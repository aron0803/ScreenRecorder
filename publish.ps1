# ScreenRecorder Publishing Script
param(
    [string]$Configuration = "Debug",
    [string]$Platform = "x64"
)

Write-Host "========================================"
Write-Host "ScreenRecorder Release Tool"
Write-Host "========================================"
Write-Host ""

$ProjectRoot = $PSScriptRoot
$BinPath = Join-Path $ProjectRoot "bin\$Platform\$Configuration\net48"
$ReleasePath = Join-Path $ProjectRoot "Release"
$Version = (Get-Date -Format "yyyyMMdd")
$ReleaseFolder = Join-Path $ReleasePath "ScreenRecorder_$Version"

if (-not (Test-Path $BinPath)) {
    Write-Host "Error: Build output not found: $BinPath" -ForegroundColor Red
    Write-Host "Please build the project first!" -ForegroundColor Yellow
    exit 1
}

$ExePath = Join-Path $BinPath "ScreenRecorder.exe"
if (-not (Test-Path $ExePath)) {
    Write-Host "Error: ScreenRecorder.exe not found" -ForegroundColor Red
    exit 1
}

Write-Host "Creating release folder: $ReleaseFolder" -ForegroundColor Green
if (Test-Path $ReleaseFolder) {
    Remove-Item $ReleaseFolder -Recurse -Force
}
New-Item -ItemType Directory -Path $ReleaseFolder -Force | Out-Null

$RequiredFiles = @(
    "ScreenRecorder.exe",
    "ScreenRecorderLib.dll"
)

$OptionalFiles = @(
    "ScreenRecorder.exe.config"
)

Write-Host ""
Write-Host "Copying required files..." -ForegroundColor Yellow
foreach ($file in $RequiredFiles) {
    $sourcePath = Join-Path $BinPath $file
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath $ReleaseFolder -Force
        Write-Host "  [OK] $file" -ForegroundColor Green
    } else {
        Write-Host "  [FAIL] Missing required file: $file" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "Copying optional files..." -ForegroundColor Yellow
foreach ($file in $OptionalFiles) {
    $sourcePath = Join-Path $BinPath $file
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath $ReleaseFolder -Force
        Write-Host "  [OK] $file" -ForegroundColor Green
    } else {
        Write-Host "  [SKIP] $file (not found)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "Copying documentation..." -ForegroundColor Yellow
$ManualPath = Join-Path $ProjectRoot "UserManual_ZH-TW.md"
if (Test-Path $ManualPath) {
    Copy-Item $ManualPath $ReleaseFolder -Force
    Write-Host "  [OK] UserManual_ZH-TW.md" -ForegroundColor Green
}

$ReadmePath = Join-Path $ProjectRoot "README.md"
if (Test-Path $ReadmePath) {
    Copy-Item $ReadmePath $ReleaseFolder -Force
    Write-Host "  [OK] README.md" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================"
Write-Host "Release Complete!" -ForegroundColor Green
Write-Host "========================================"
Write-Host ""
Write-Host "Release folder: $ReleaseFolder" -ForegroundColor Cyan
Write-Host ""

$files = Get-ChildItem $ReleaseFolder
$totalSize = ($files | Measure-Object -Property Length -Sum).Sum
Write-Host "Files:" -ForegroundColor Yellow
foreach ($file in $files) {
    $sizeMB = [math]::Round($file.Length / 1MB, 2)
    Write-Host "  - $($file.Name) ($sizeMB MB)"
}
Write-Host ""
$totalSizeMB = [math]::Round($totalSize / 1MB, 2)
Write-Host "Total size: $totalSizeMB MB" -ForegroundColor Cyan
Write-Host ""

$CreateZip = Read-Host "Create ZIP archive? (Y/N)"
if ($CreateZip -eq "Y" -or $CreateZip -eq "y") {
    $ZipPath = "$ReleaseFolder.zip"
    if (Test-Path $ZipPath) {
        Remove-Item $ZipPath -Force
    }
    
    Write-Host "Creating archive..." -ForegroundColor Yellow
    Compress-Archive -Path "$ReleaseFolder\*" -DestinationPath $ZipPath -Force
    
    if (Test-Path $ZipPath) {
        $zipSize = [math]::Round((Get-Item $ZipPath).Length / 1MB, 2)
        Write-Host "[OK] Archive created: $ZipPath ($zipSize MB)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
