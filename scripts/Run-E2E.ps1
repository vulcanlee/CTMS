<#
.SYNOPSIS
    Run the "add three hospitals" Playwright E2E verification, protecting dev data.

.DESCRIPTION
    The app's connection string and Data\ paths are hard-coded and cannot be
    redirected to a test database via environment variables. This script therefore
    protects the developer's data with a backup -> test -> restore flow:
      1. Back up BackendDB.db, Data\SubjectNoGenerator.json, Data\RandomListRuntime.json
      2. Start dotnet run (http profile, http://localhost:5272) and poll until ready
      3. Run "npx playwright test" under tests\e2e
      4. Stop dotnet FIRST (to release the SQLite file lock), then restore the 3 files
      5. Open the HTML report

    Whether the tests pass or fail, the app is stopped and the data is restored
    (finally block).

.PARAMETER Headed
    Run in headed mode so you can watch the browser. Default: on.

.PARAMETER ShowReport
    Open the HTML report after the run. Default: on.

.PARAMETER KeepData
    Skip the restore step so the patients created by the tests REMAIN in
    BackendDB.db (visible after you restart the app). Default: off (data restored).

.PARAMETER Spec
    Run only the test files whose name matches this keyword (Playwright test
    filename filter), e.g. -Spec random-workflow. When set, the HTML report is
    written to playwright-report-spec\ instead of playwright-report\, so the
    canonical playwright-report\index.html (last full-suite run) is never
    overwritten by a filtered run. Default: unset (run all tests).

.EXAMPLE
    pwsh scripts\Run-E2E.ps1
    pwsh scripts\Run-E2E.ps1 -Headed:$false   # headless (faster, no window)
    pwsh scripts\Run-E2E.ps1 -KeepData        # keep test patients in the database
#>
[CmdletBinding()]
param(
    [switch]$Headed = $true,
    [switch]$ShowReport = $true,
    [switch]$KeepData,
    # 只跑符合此關鍵字的測試檔（Playwright test 檔名過濾），例如 -Spec random-workflow
    [string]$Spec
)

$ErrorActionPreference = 'Stop'

# --- paths ---
$RepoRoot = Split-Path -Parent $PSScriptRoot
$AppDir   = Join-Path $RepoRoot 'src\CTMS\CTMS'
$E2EDir   = Join-Path $RepoRoot 'tests\e2e'
$DataDir  = Join-Path $AppDir 'Data'
$BaseUrl  = 'http://localhost:5272'
$BackupDir = Join-Path $E2EDir '.data-backup'

# Files to back up / restore.
# NOTE: SQLite uses side-car files BackendDB.db-wal / BackendDB.db-shm. A hard kill
# leaves an un-checkpointed WAL; if you restore only the .db but keep a stale WAL,
# SQLite reports "database disk image is malformed". So the whole db family is
# treated as one unit: on restore we delete all live db-family files first, then
# copy back whatever was backed up (a consistent snapshot).
$DbBase   = Join-Path $AppDir 'BackendDB.db'
$DbFamily = @($DbBase, "$DbBase-wal", "$DbBase-shm")
$JsonFiles = @(
    (Join-Path $DataDir 'SubjectNoGenerator.json'),
    (Join-Path $DataDir 'RandomListRuntime.json')
)
$ProtectedFiles = $DbFamily + $JsonFiles

function Backup-Data {
    if (Test-Path $BackupDir) { Remove-Item $BackupDir -Recurse -Force }
    New-Item -ItemType Directory -Path $BackupDir | Out-Null
    foreach ($f in $ProtectedFiles) {
        $name = Split-Path $f -Leaf
        if (Test-Path $f) {
            Copy-Item $f -Destination (Join-Path $BackupDir $name) -Force
            Write-Host "  backed up $name"
        }
    }
}

function Restore-Data {
    # Remove all live SQLite db-family files so no stale WAL/SHM mismatches the .db
    foreach ($f in $DbFamily) { if (Test-Path $f) { Remove-Item $f -Force } }
    foreach ($f in $ProtectedFiles) {
        $name = Split-Path $f -Leaf
        $b = Join-Path $BackupDir $name
        if (Test-Path $b) {
            Copy-Item $b -Destination $f -Force
            Write-Host "  restored $name"
        }
    }
}

$dotnet = $null
$testExit = 1
try {
    Write-Host '=== 1/4 Back up dev data ===' -ForegroundColor Cyan
    Backup-Data

    Write-Host '=== 2/4 Build and start CTMS app ===' -ForegroundColor Cyan
    # Build first so "dotnet run --no-build" serves quickly and deterministically
    Write-Host '  building...'
    & dotnet build (Join-Path $AppDir 'CTMS.csproj') -c Debug --nologo -v quiet
    if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }

    $appLog = Join-Path $E2EDir 'app-run.log'
    $appErr = Join-Path $E2EDir 'app-run.err.log'
    $dotnet = Start-Process -FilePath 'dotnet' `
        -ArgumentList 'run', '--no-build', '--launch-profile', 'http' `
        -WorkingDirectory $AppDir -PassThru -WindowStyle Minimized `
        -RedirectStandardOutput $appLog -RedirectStandardError $appErr

    $ready = $false
    for ($i = 0; $i -lt 120; $i++) {
        if ($dotnet.HasExited) { throw "dotnet run exited early (code $($dotnet.ExitCode)); see $appErr" }
        try {
            $resp = Invoke-WebRequest -Uri "$BaseUrl/Auths/Login" -UseBasicParsing -TimeoutSec 3
            if ($resp.StatusCode -eq 200) { $ready = $true; break }
        } catch {
            Start-Sleep -Seconds 1
        }
    }
    if (-not $ready) { throw "App not ready in time ($BaseUrl)" }
    Write-Host "  app ready: $BaseUrl"

    Write-Host '=== 3/4 Run Playwright tests ===' -ForegroundColor Cyan
    Push-Location $E2EDir
    try {
        $pwArgs = @('playwright', 'test')
        if ($Headed) { $pwArgs += '--headed' }
        if ($Spec) {
            # 篩選跑：把 HTML 報告導到另一個資料夾，避免覆蓋正式的全套報告
            $env:PLAYWRIGHT_HTML_OUTPUT_DIR = 'playwright-report-spec'
            $pwArgs += $Spec
        }
        & npx @pwArgs
        $testExit = $LASTEXITCODE
    } finally {
        Pop-Location
    }
}
finally {
    Write-Host '=== 4/4 Stop app and restore data ===' -ForegroundColor Cyan
    if ($dotnet -and -not $dotnet.HasExited) {
        # Kill dotnet run and its child processes (Kestrel runs in a child)
        taskkill /PID $dotnet.Id /T /F 2>$null | Out-Null
        Start-Sleep -Seconds 2
    }
    if ($KeepData) {
        Write-Host '  -KeepData: skipping restore; test-created patients remain in BackendDB.db'
    } else {
        Restore-Data
    }
}

if ($testExit -eq 0) {
    Write-Host "`n[PASS] all tests passed" -ForegroundColor Green
} else {
    Write-Host "`n[FAIL] some tests failed (exit $testExit); see the HTML report" -ForegroundColor Yellow
}

if ($ShowReport) {
    Push-Location $E2EDir
    try {
        if ($Spec) {
            & npx playwright show-report playwright-report-spec
        } else {
            & npx playwright show-report
        }
    } finally { Pop-Location }
}

exit $testExit
