<#
.SYNOPSIS
    把 docs/ 下的來源文件組裝成單一份《CTMS 系統完整說明書》。

.DESCRIPTION
    輸出 docs/系統完整說明書.md，內容依序為：
      導讀與導覽圖 → 使用者篇（KM）→ 欄位參考篇（docs/12）→ 技術篇

    組裝時會做三件事：
      1. 章節標題降兩階（來源的 # 變成 ###），避免單檔內出現多個 H1
      2. 來源檔之間的相對連結改寫成**文件內錨點**；指向非收錄檔案的連結改成相對於 docs/ 的路徑
      3. 檔尾記錄每個來源檔的 SHA256，供 Test-Docs.ps1 判斷這份產生檔是否過期

    🔴 產生檔請勿直接編輯——改來源檔後重跑本腳本。

.EXAMPLE
    pwsh scripts/Build-UserManual.ps1
#>
param(
    [string]$OutFile = "docs/系統完整說明書.md"
)

$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [Text.Encoding]::UTF8 } catch { }

$repoRoot = Split-Path -Parent $PSScriptRoot
$docsRoot = Join-Path $repoRoot 'docs'
$outPath = Join-Path $repoRoot $OutFile

function Read-Text([string]$p) { return [IO.File]::ReadAllText($p, [Text.Encoding]::UTF8) }

function Get-Slug([string]$heading) {
    $t = $heading.Trim()
    $t = [regex]::Replace($t, '\[([^\]]*)\]\([^)]*\)', '$1')
    $t = $t -replace '[`*_~]', ''
    $t = $t.ToLowerInvariant().Trim()
    $t = [regex]::Replace($t, '[^\p{L}\p{Nd}\p{Mn} \-]', '')
    return ($t -replace ' ', '-')
}

# ── 組裝順序 ────────────────────────────────────────────────────────────
$sections = @(
    @{ Title = '導讀與畫面導覽'; Files = @(
            'docs/01-系統總覽/接手指南.md'
            'docs/01-系統總覽/畫面導覽地圖.md'
        )
    }
    @{ Title = '使用者篇：怎麼操作'; Files = @(
            'docs/KM/新手入門-從收案到匯出.md'
            'docs/KM/共通訊息與錯誤訊息.md'
        ) + ((Get-ChildItem (Join-Path $docsRoot 'KM') -Filter *.md -File |
                Where-Object { $_.Name -ne 'README.md' -and $_.Name -notlike '新手入門-*' -and $_.Name -ne '共通訊息與錯誤訊息.md' } |
                Sort-Object Name | ForEach-Object { "docs/KM/$($_.Name)" }))
    }
    @{ Title = '欄位參考篇：每個頁籤的每個欄位'; Files = @(
            (Get-ChildItem (Join-Path $docsRoot '12-畫面欄位參考') -Filter *.md -File |
                Where-Object { $_.Name -ne 'README.md' } | Sort-Object Name |
                ForEach-Object { "docs/12-畫面欄位參考/$($_.Name)" })
        )
    }
    @{ Title = '功能模組篇：每個功能背後的邏輯'; Files = @(
            'docs/04-功能模組/個案管理.md'
            'docs/04-功能模組/儀表板.md'
            'docs/04-功能模組/收案進度統計.md'
            'docs/04-功能模組/臨床資訊與病史.md'
            'docs/04-功能模組/治療紀錄.md'
            'docs/04-功能模組/抽血檢驗.md'
            'docs/04-功能模組/副作用與CTCAE分級.md'
            'docs/04-功能模組/問卷收集.md'
            'docs/04-功能模組/追蹤資料與縱向影像.md'
            'docs/04-功能模組/AI風險評估與骨骼肌分析.md'
            'docs/04-功能模組/簽核確認.md'
            'docs/04-功能模組/DICOM影像處理.md'
            'docs/04-功能模組/受試者編號與隨機分組.md'
            'docs/04-功能模組/使用者與權限管理.md'
            'docs/04-功能模組/資料匯入匯出.md'
            'docs/04-功能模組/操作歷程與稽核.md'
            'docs/05-AI推論/overview.md'
            'docs/05-AI推論/flow-1-dicom-upload.md'
            'docs/05-AI推論/flow-2-manual-annotation.md'
            'docs/05-AI推論/AIAgent背景服務.md'
        )
    }
    @{ Title = '技術篇：資料怎麼被處理與紀錄'; Files = @(
            'docs/02-架構/開發慣例與限制速查.md'
            'docs/02-架構/啟動與初始化.md'
            'docs/03-資料模型/領域資料模型.md'
            'docs/03-資料模型/VisitCode-資料結構與關聯.md'
            'docs/03-資料模型/問卷與抽血資料的建立與維護.md'
            'docs/02-架構/外部服務串接.md'
            'docs/06-部署與維護/資料讀寫與運作狀態落點.md'
            'docs/11-資安/認證授權與權限機制.md'
            'docs/06-部署與維護/文件維護規範.md'
        )
    }
)

# ── 收集來源檔並建立「檔案 → 錨點」對照 ─────────────────────────────────
$entries = New-Object System.Collections.Generic.List[object]
foreach ($sec in $sections) {
    foreach ($rel in $sec.Files) {
        $full = Join-Path $repoRoot ($rel -replace '/', [IO.Path]::DirectorySeparatorChar)
        if (-not (Test-Path $full)) { Write-Host "!! 找不到來源檔：$rel" -ForegroundColor Red; continue }
        $text = Read-Text $full
        $h1 = ([regex]::Match($text, '(?m)^#\s+(.+)$')).Groups[1].Value.Trim()
        if (-not $h1) { $h1 = [IO.Path]::GetFileNameWithoutExtension($full) }
        $entries.Add([pscustomobject]@{
                Section = $sec.Title
                Rel     = $rel
                Full    = (Resolve-Path $full).Path
                Title   = $h1
                Anchor  = (Get-Slug $h1)
                Text    = $text
            })
    }
}

$anchorByPath = @{}
foreach ($e in $entries) { $anchorByPath[$e.Full] = $e.Anchor }

# ── 產生內容 ────────────────────────────────────────────────────────────
$version = ([regex]::Match((Read-Text (Join-Path $repoRoot 'src/CTMS/CTMS/appsettings.json')),
        '"SystemVersion"\s*:\s*"([^"]+)"')).Groups[1].Value
$today = (Get-Item $PSCommandPath).LastWriteTime.ToString('yyyy/MM/dd')

$out = New-Object System.Collections.Generic.List[string]
$out.Add('# CTMS 系統完整說明書')
$out.Add('')
$out.Add('- 文件用途：把使用者操作、欄位參考與技術說明組裝成單一檔案，供交付、稽核與離線查閱')
$out.Add('- 主要讀者：使用者、開發者、維運人員與稽核')
$out.Add("- 對應系統版本：$version")
$out.Add("- 最後核對日期：見各章節標頭（本檔為組裝產物，不另行對版）")
$out.Add('- 編碼：UTF-8（繁體中文，含 BOM）')
$out.Add('')
$out.Add('---')
$out.Add('')
$out.Add('> 🔴 **本檔由 `scripts/Build-UserManual.ps1` 產生，請勿直接編輯。** 要改內容請改對應的來源檔，再重跑腳本；`scripts/Test-Docs.ps1` 會比對檔尾的來源雜湊，來源改了卻沒重跑就會檢查失敗。')
$out.Add('')

# 目錄
$out.Add('## 目錄')
$out.Add('')
foreach ($sec in $sections) {
    $out.Add("### $($sec.Title)")
    $out.Add('')
    foreach ($e in ($entries | Where-Object Section -eq $sec.Title)) {
        $out.Add("- [$($e.Title)](#$($e.Anchor))")
    }
    $out.Add('')
}
$out.Add('---')
$out.Add('')

# 未收錄清單：讓「哪些內容不在本檔」變成明寫的事實，而不是靜默的缺口
$includedPaths = New-Object System.Collections.Generic.HashSet[string]
foreach ($e in $entries) { [void]$includedPaths.Add($e.Full) }
$notIncluded = @()
foreach ($f in (Get-ChildItem $docsRoot -Filter *.md -File -Recurse | Sort-Object FullName)) {
    if ($f.Name -eq 'README.md') { continue }
    if ($f.FullName -eq $outPath) { continue }
    if ($includedPaths.Contains($f.FullName)) { continue }
    $rel = [IO.Path]::GetRelativePath($docsRoot, $f.FullName) -replace '\\', '/'
    if ($rel -like 'Prompts/*' -or $rel -like 'superpowers/*') { continue }
    $notIncluded += $rel
}
if ($notIncluded) {
    $out.Add('## 本檔未收錄的文件')
    $out.Add('')
    $out.Add('下列文件留在 `docs/` 各分類內，**沒有**組裝進本檔（多為歷史紀錄、測試報告與擴充指南）。這份清單由腳本自動產生，不會漏列。')
    $out.Add('')
    foreach ($rel in $notIncluded) { $out.Add("- [$rel]($rel)") }
    $out.Add('')
    $out.Add('---')
    $out.Add('')
}

$currentSection = ''
foreach ($e in $entries) {
    if ($e.Section -ne $currentSection) {
        $currentSection = $e.Section
        $out.Add("## $currentSection")
        $out.Add('')
    }

    $lines = $e.Text -split "`r?`n"
    $inFence = $false
    foreach ($line in $lines) {
        if ($line -match '^\s*```') { $inFence = -not $inFence; $out.Add($line); continue }
        if ($inFence) { $out.Add($line); continue }

        # 標題降兩階
        if ($line -match '^(#{1,4})\s+(.*)$') {
            $line = ('#' * ($Matches[1].Length + 2)) + ' ' + $Matches[2]
        }

        # 連結改寫
        $line = [regex]::Replace($line, '\[([^\]]*)\]\(([^)\s]+)\)', {
                param($m)
                $label = $m.Groups[1].Value
                $target = $m.Groups[2].Value
                if ($target -match '^(https?:|mailto:|tel:|#)') { return $m.Value }
                $parts = $target -split '#', 2
                $pathPart = [Uri]::UnescapeDataString($parts[0])
                if ($pathPart -eq '') { return $m.Value }
                $resolved = [IO.Path]::GetFullPath([IO.Path]::Combine((Split-Path $e.Full), $pathPart))
                if ($anchorByPath.ContainsKey($resolved)) {
                    return "[$label](#$($anchorByPath[$resolved]))"
                }
                # 未收錄：改成相對於 docs/ 的路徑（產生檔就放在 docs/ 底下）
                $relToDocs = [IO.Path]::GetRelativePath($docsRoot, $resolved) -replace '\\', '/'
                return "[$label]($relToDocs)"
            })

        $out.Add($line)
    }
    $out.Add('')
    $out.Add('---')
    $out.Add('')
}

# 檔尾：來源雜湊
$out.Add('## 來源檔與雜湊')
$out.Add('')
$out.Add('下表由組裝腳本產生，供 `Test-Docs.ps1` 判斷本檔是否過期。**請勿手動修改。**')
$out.Add('')
$out.Add('<!-- BUILD-SOURCES-START -->')
$out.Add('| 來源檔 | SHA256 |')
$out.Add('| --- | --- |')
foreach ($e in $entries) {
    $hash = (Get-FileHash -Path $e.Full -Algorithm SHA256).Hash
    $out.Add("| ``$($e.Rel)`` | ``$hash`` |")
}
$out.Add('<!-- BUILD-SOURCES-END -->')
$out.Add('')

$utf8Bom = New-Object System.Text.UTF8Encoding $true
[IO.File]::WriteAllText($outPath, ($out -join "`r`n"), $utf8Bom)

Write-Host "已產生：$OutFile" -ForegroundColor Green
Write-Host "  組裝來源 $($entries.Count) 份、共 $($out.Count) 行" -ForegroundColor Green
foreach ($sec in $sections) {
    $n = @($entries | Where-Object Section -eq $sec.Title).Count
    Write-Host "  - $($sec.Title)：$n 份"
}
