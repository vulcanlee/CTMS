<#
.SYNOPSIS
    docs/ 文件體系的機械守門檢查（彙總入口）。

.DESCRIPTION
    六項檢查：
      1. 編碼        —— 委派 Test-DocsEncoding.ps1（UTF-8 含 BOM、無 U+FFFD）
      2. 連結與錨點  —— 相對連結的檔案存在、#錨點對得上標題、目錄連結有 README.md
      3. 索引完整性  —— 每份文件至少被某個 README.md 連到一次
      4. 文件標頭    —— 非豁免的 .md 必須有五欄標頭
      5. 易腐數字    —— 禁止在文件內硬寫醫院家數（唯一出處是 HospitalRegistry）
      6. KM 章節     —— 畫面手冊必須有四個必備章節

    這些檢查只驗結構，驗不出「敘述與程式碼是否相符」——那要靠
    docs/06-部署與維護/文件維護規範.md 的紀律與人工校對。

.EXAMPLE
    pwsh scripts/Test-Docs.ps1
#>
param(
    [string]$DocsPath = "docs"
)

$ErrorActionPreference = "Stop"

# ⚠️ 檔名含中文時，Windows cp950 主控台會讓輸出丟 UnicodeEncodeError 而整支腳本掛掉，
# 結束碼非 0 看起來像有死連結，其實只是印不出來。
try { [Console]::OutputEncoding = [Text.Encoding]::UTF8 } catch { }

$repoRoot = Split-Path -Parent $PSScriptRoot
$docsRoot = (Resolve-Path (Join-Path $repoRoot $DocsPath)).Path
$failures = New-Object System.Collections.Generic.List[string]

function Add-Failure([string]$check, [string]$file, [string]$reason) {
    $rel = $file.Replace($repoRoot + [IO.Path]::DirectorySeparatorChar, '')
    $failures.Add("[$check] $rel`n         $reason")
}

function Read-Text([string]$path) {
    return [IO.File]::ReadAllText($path, [Text.Encoding]::UTF8)
}

function Test-PathMatch([string]$fullPath, [string[]]$patterns) {
    $rel = $fullPath.Replace($repoRoot + [IO.Path]::DirectorySeparatorChar, '').Replace('\', '/')
    foreach ($p in $patterns) { if ($rel -like $p) { return $true } }
    return $false
}

# GitHub 相容的標題 slug：小寫、去掉標記符號、空白轉連字號、保留中日韓文字
function Get-HeadingSlug([string]$heading) {
    $t = $heading.Trim()
    $t = [regex]::Replace($t, '\[([^\]]*)\]\([^)]*\)', '$1')
    $t = $t -replace '[`*_~]', ''
    $t = $t.ToLowerInvariant().Trim()
    $t = [regex]::Replace($t, '[^\p{L}\p{Nd}\p{Mn} \-]', '')
    return ($t -replace ' ', '-')
}

# 取出檔案內的連結，略過程式碼區塊與外部連結
function Get-DocLinks([string]$path) {
    $links = @()
    $inFence = $false
    foreach ($line in (Read-Text $path) -split "`r?`n") {
        if ($line -match '^\s*```') { $inFence = -not $inFence; continue }
        if ($inFence) { continue }
        foreach ($m in [regex]::Matches($line, '\[[^\]]*\]\(([^)\s]+)\)')) {
            $target = $m.Groups[1].Value
            if ($target -match '^(https?:|mailto:|tel:)') { continue }
            $links += $target
        }
    }
    return $links
}

$allMd = Get-ChildItem -Path $docsRoot -Filter *.md -File -Recurse
$allDocFiles = Get-ChildItem -Path $docsRoot -File -Recurse | Where-Object { $_.Extension -in '.md', '.html' }

Write-Host "檢查對象：$($allMd.Count) 份 .md（$docsRoot）" -ForegroundColor Cyan
Write-Host ""

# ── 1. 編碼檢查（委派既有腳本，只回報失敗） ─────────────────────────────
Write-Host "1/8 編碼（UTF-8 含 BOM、無亂碼）" -ForegroundColor Cyan
$encScript = Join-Path $PSScriptRoot 'Test-DocsEncoding.ps1'
try {
    # *>&1 一併攔下 Write-Host 的輸出，否則 83 行 OK 會蓋掉真正的失敗訊息
    $encOutput = & $encScript -DocsPath $docsRoot *>&1
    $encFailed = $encOutput | Where-Object { "$_" -like 'FAILED*' }
    foreach ($line in $encFailed) { $failures.Add("[編碼] $line") }
}
catch {
    $encFailed = $encOutput | Where-Object { "$_" -like 'FAILED*' }
    if ($encFailed) { foreach ($line in $encFailed) { $failures.Add("[編碼] $line") } }
    else { $failures.Add("[編碼] Test-DocsEncoding.ps1 執行失敗：$($_.Exception.Message)") }
}

# ── 2. 連結與錨點 ───────────────────────────────────────────────────────
Write-Host "2/8 連結與錨點" -ForegroundColor Cyan
$headingCache = @{}
function Get-Slugs([string]$path) {
    if ($headingCache.ContainsKey($path)) { return $headingCache[$path] }
    $slugs = @()
    if ([IO.Path]::GetExtension($path) -eq '.md' -and (Test-Path $path)) {
        foreach ($line in (Read-Text $path) -split "`r?`n") {
            if ($line -match '^#{1,6}\s+(.*)$') { $slugs += (Get-HeadingSlug $Matches[1]) }
        }
    }
    $headingCache[$path] = $slugs
    return $slugs
}

foreach ($file in $allMd) {
    $dir = $file.DirectoryName
    foreach ($target in (Get-DocLinks $file.FullName)) {
        $parts = $target -split '#', 2
        $pathPart = [Uri]::UnescapeDataString($parts[0])
        $anchor = if ($parts.Count -eq 2) { [Uri]::UnescapeDataString($parts[1]).ToLowerInvariant() } else { $null }

        $resolved = if ($pathPart -eq '') { $file.FullName }
        else { [IO.Path]::GetFullPath([IO.Path]::Combine($dir, $pathPart)) }

        if (-not (Test-Path $resolved)) {
            Add-Failure '連結' $file.FullName "找不到目標：$target"
            continue
        }
        if ((Get-Item $resolved).PSIsContainer) {
            if (-not (Test-Path (Join-Path $resolved 'README.md'))) {
                Add-Failure '連結' $file.FullName "目錄連結但該目錄沒有 README.md：$target"
            }
            continue
        }
        if ($anchor -and ([IO.Path]::GetExtension($resolved) -eq '.md')) {
            if ((Get-Slugs $resolved) -notcontains $anchor) {
                Add-Failure '錨點' $file.FullName "目標文件沒有這個標題：$target"
            }
        }
    }
}

# ── 3. 索引完整性 ───────────────────────────────────────────────────────
Write-Host "3/8 索引完整性（每份文件都要被 README 連到）" -ForegroundColor Cyan
$indexed = New-Object System.Collections.Generic.HashSet[string]
foreach ($readme in ($allMd | Where-Object Name -eq 'README.md')) {
    foreach ($target in (Get-DocLinks $readme.FullName)) {
        $pathPart = [Uri]::UnescapeDataString(($target -split '#', 2)[0])
        if ($pathPart -eq '') { continue }
        $resolved = [IO.Path]::GetFullPath([IO.Path]::Combine($readme.DirectoryName, $pathPart))
        [void]$indexed.Add($resolved)
    }
}
$manualPath = Join-Path $docsRoot '系統完整說明書.md'
foreach ($doc in ($allDocFiles | Where-Object Name -ne 'README.md')) {
    # 說明書是組裝產物，它的連結指向自身錨點，不適用索引規則
    if ($doc.FullName -eq $manualPath) { continue }
    if (-not $indexed.Contains($doc.FullName)) {
        Add-Failure '索引' $doc.FullName "沒有任何 README.md 連到它——這份文件等於不存在"
    }
}

# ── 4. 文件標頭 ─────────────────────────────────────────────────────────
Write-Host "4/8 文件標頭區塊" -ForegroundColor Cyan
# 豁免：提示詞原稿與歷史紀錄保留原貌；Marp 簡報的 front matter 必須在第一行；README 另有格式
$headerExempt = @(
    'docs/Prompts/*',
    'docs/superpowers/*',
    'docs/08-測試/AI自動化測試-成果簡報（GM）.md'
)
$headerKeys = @('文件用途', '主要讀者', '對應系統版本', '最後核對日期', '編碼')
# 變更紀錄有自己的標頭欄位（記錄的是一次異動，不是一份會被對版的說明文件）
$changelogKeys = @('變更日期', '版本', '性質', '起因', '編碼')
foreach ($file in $allMd) {
    if ($file.Name -eq 'README.md') { continue }
    if (Test-PathMatch $file.FullName $headerExempt) { continue }
    $keys = if (Test-PathMatch $file.FullName @('docs/09-變更紀錄/*')) { $changelogKeys } else { $headerKeys }
    $head = ((Read-Text $file.FullName) -split "`r?`n" | Select-Object -First 15) -join "`n"
    $missing = $keys | Where-Object { $head -notmatch "(?m)^- $_：" }
    if ($missing) {
        Add-Failure '標頭' $file.FullName "缺少標頭欄位：$($missing -join '、')"
    }
}

# ── 5. 易腐數字（醫院家數） ─────────────────────────────────────────────
Write-Host "5/8 易腐數字（醫院家數不得寫死）" -ForegroundColor Cyan
# 豁免：特定時間點的歷史報告與決策紀錄，改寫它們會抹除當時的判斷依據
$numberExempt = @(
    'docs/Prompts/*',
    'docs/superpowers/*',
    'docs/07-擴充指南/新增三家醫院-*.md',
    'docs/08-測試/*',
    'docs/09-變更紀錄/*',
    'docs/10-規劃待辦/*'
)
foreach ($file in $allMd) {
    if (Test-PathMatch $file.FullName $numberExempt) { continue }
    $lineNo = 0
    foreach ($line in (Read-Text $file.FullName) -split "`r?`n") {
        $lineNo++
        # 「新增三家醫院」是專案名稱，不是現況宣稱，故排除
        if ($line -match '(?<!新增)[三四五六七八九十]家醫院') {
            Add-Failure '易腐數字' $file.FullName "第 $lineNo 行寫死醫院家數：$($line.Trim())`n         醫院清單的唯一出處是 HospitalRegistry.All，請改寫成不變量並連到 07-擴充指南/新增醫院.md"
        }
    }
}

# ── 6. KM 章節 ──────────────────────────────────────────────────────────
Write-Host "6/8 KM 畫面手冊必備章節" -ForegroundColor Cyan
# 「畫面欄位說明」「按鈕與操作說明」「命名與填寫建議」刻意不強制：
# 有些畫面真的沒有可填欄位或可按按鈕（例如收案進度只讀、資料匯出無欄位），
# 強制它們存在等於逼人編一段出來，比缺那段更糟。
$kmRequired = @('這個畫面是做什麼的', '怎麼進到這個畫面', '狀態與訊息', '常見問答')
foreach ($file in (Get-ChildItem -Path (Join-Path $docsRoot 'KM') -Filter *.md -File)) {
    # README、流程書與共通訊息表不是畫面手冊，不適用八段式
    if ($file.Name -eq 'README.md' -or $file.Name -like '新手入門-*' -or $file.Name -eq '共通訊息與錯誤訊息.md') { continue }
    $text = Read-Text $file.FullName
    $missing = $kmRequired | Where-Object { $text -notmatch "(?m)^#{2,3}\s.*$_" }
    if ($missing) {
        Add-Failure 'KM章節' $file.FullName "缺少必備章節：$($missing -join '、')"
    }
}

# ── 7. docs/12 必備章節 ─────────────────────────────────────────────────
Write-Host "7/8 畫面欄位參考必備章節" -ForegroundColor Cyan
$fieldRefDir = Join-Path $docsRoot '12-畫面欄位參考'
if (Test-Path $fieldRefDir) {
    foreach ($file in (Get-ChildItem $fieldRefDir -Filter *.md -File)) {
        if ($file.Name -eq 'README.md') { continue }
        $text = Read-Text $file.FullName
        # 章節標題只要求「含欄位」與「含條件顯示規則」，不強制字面叫「欄位表」——
        # 「查詢條件欄位」「編輯表單欄位」「顯示欄位」都比統一字面更貼合該畫面。
        $missing = @()
        if ($text -notmatch '(?m)^#{2,4}\s.*欄位') { $missing += '欄位（章節標題需含「欄位」）' }
        if ($text -notmatch '(?m)^#{2,4}\s.*條件顯示規則') { $missing += '條件顯示規則' }
        if ($missing) {
            Add-Failure '欄位參考' $file.FullName "缺少必備章節：$($missing -join '、')。沒有條件顯示的畫面請寫一節「條件顯示規則」內容為「無」，不要略過章節"
        }
    }
}

# ── 8. 說明書是否過期 ───────────────────────────────────────────────────
Write-Host "8/8 系統完整說明書是否為最新" -ForegroundColor Cyan
if (Test-Path $manualPath) {
    $manualText = Read-Text $manualPath
    $block = [regex]::Match($manualText, '(?s)<!-- BUILD-SOURCES-START -->(.*?)<!-- BUILD-SOURCES-END -->')
    if (-not $block.Success) {
        Add-Failure '說明書' $manualPath "找不到來源雜湊區塊——這份檔案不是由 scripts/Build-UserManual.ps1 產生的"
    }
    else {
        $stale = @()
        foreach ($m in [regex]::Matches($block.Groups[1].Value, '\|\s*`([^`]+)`\s*\|\s*`([0-9A-Fa-f]{64})`\s*\|')) {
            $srcRel = $m.Groups[1].Value
            $recorded = $m.Groups[2].Value
            $srcFull = Join-Path $repoRoot ($srcRel -replace '/', [IO.Path]::DirectorySeparatorChar)
            if (-not (Test-Path $srcFull)) { $stale += "$srcRel（來源檔已不存在）"; continue }
            $now = (Get-FileHash -Path $srcFull -Algorithm SHA256).Hash
            if ($now -ne $recorded) { $stale += $srcRel }
        }
        if ($stale) {
            Add-Failure '說明書' $manualPath "下列來源檔已變更，請重跑 ``pwsh scripts/Build-UserManual.ps1``：`n         $($stale -join "`n         ")"
        }
    }
}
else {
    Add-Failure '說明書' $manualPath "檔案不存在——請執行 pwsh scripts/Build-UserManual.ps1"
}

# ── 結果 ────────────────────────────────────────────────────────────────
Write-Host ""
if ($failures.Count -gt 0) {
    Write-Host "文件檢查失敗，共 $($failures.Count) 項：" -ForegroundColor Red
    foreach ($f in $failures) { Write-Host "  $f" -ForegroundColor Red }
    Write-Host ""
    exit 1
}

Write-Host "全部通過：編碼、連結與錨點、索引完整性、標頭、易腐數字、KM 章節、欄位參考章節、說明書為最新。" -ForegroundColor Green
exit 0
