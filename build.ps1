# ScreenRecorder Build Script
# Auto compile and package ScreenRecorder application
# Updated for single release folder

param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [switch]$SkipPackage
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ScreenRecorder Build Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$ProjectRoot = $PSScriptRoot
$ProjectFile = Join-Path $ProjectRoot "ScreenRecorder.csproj"
$BinPath = Join-Path $ProjectRoot "bin\$Platform\$Configuration\net48"
$ReleasePath = Join-Path $ProjectRoot "Release"
# $Version = (Get-Date -Format "yyyyMMdd")
$ReleaseFolder = Join-Path $ReleasePath "ScreenRecorder"

# Check project file exists
if (-not (Test-Path $ProjectFile)) {
    Write-Host "Error: Project file not found: $ProjectFile" -ForegroundColor Red
    exit 1
}

# Find MSBuild
Write-Host "Finding MSBuild..." -ForegroundColor Yellow
$msbuildPaths = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
)

$msbuild = $null
foreach ($path in $msbuildPaths) {
    if (Test-Path $path) {
        $msbuild = $path
        break
    }
}

# If MSBuild not found, try dotnet
if (-not $msbuild) {
    Write-Host "MSBuild not found, trying dotnet build..." -ForegroundColor Yellow
    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($dotnet) {
        $useDotnet = $true
        Write-Host "  [OK] Using dotnet CLI" -ForegroundColor Green
    }
    else {
        Write-Host "Error: Neither MSBuild nor dotnet CLI found" -ForegroundColor Red
        Write-Host "Please install Visual Studio or .NET SDK" -ForegroundColor Yellow
        exit 1
    }
}
else {
    Write-Host "  [OK] Found MSBuild: $msbuild" -ForegroundColor Green
    $useDotnet = $false
}

Write-Host ""

# Clean old build output
Write-Host "Cleaning old build output..." -ForegroundColor Yellow
if (Test-Path $BinPath) {
    Remove-Item $BinPath -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "  [OK] Cleaned" -ForegroundColor Green
}

Write-Host ""

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
if ($useDotnet) {
    & dotnet restore $ProjectFile /p:Platform=$Platform
}
else {
    & $msbuild $ProjectFile /t:Restore /p:Configuration=$Configuration /p:Platform=$Platform /v:minimal
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: NuGet restore failed" -ForegroundColor Red
    exit 1
}
Write-Host "  [OK] Restore completed" -ForegroundColor Green
Write-Host ""

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
Write-Host "  Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "  Platform: $Platform" -ForegroundColor Cyan
Write-Host ""

if ($useDotnet) {
    & dotnet build $ProjectFile --configuration $Configuration --no-restore /p:Platform=$Platform
}
else {
    & $msbuild $ProjectFile /p:Configuration=$Configuration /p:Platform=$Platform /v:minimal /nologo
}

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""

# Check output file
$ExePath = Join-Path $BinPath "ScreenRecorder.exe"
if (-not (Test-Path $ExePath)) {
    Write-Host "Error: Build output not found: $ExePath" -ForegroundColor Red
    exit 1
}

$exeSize = [math]::Round((Get-Item $ExePath).Length / 1KB, 2)
Write-Host "Output location: $BinPath" -ForegroundColor Cyan
Write-Host "Executable size: $exeSize KB" -ForegroundColor Cyan
Write-Host ""

# Skip packaging if requested
if ($SkipPackage) {
    Write-Host "Skipping packaging (SkipPackage parameter used)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Done!" -ForegroundColor Green
    exit 0
}

# Start packaging
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Packaging Release" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Creating release folder: $ReleaseFolder" -ForegroundColor Yellow
if (Test-Path $ReleaseFolder) {
    Remove-Item $ReleaseFolder -Recurse -Force
}
New-Item -ItemType Directory -Path $ReleaseFolder -Force | Out-Null
Write-Host "  [OK] Folder created" -ForegroundColor Green
Write-Host ""

# Required files list
$RequiredFiles = @(
    "ScreenRecorder.exe",
    "ScreenRecorderLib.dll"
)

# Optional files list
$OptionalFiles = @(
    "ScreenRecorder.exe.config"
)

Write-Host "Copying required files..." -ForegroundColor Yellow
foreach ($file in $RequiredFiles) {
    $sourcePath = Join-Path $BinPath $file
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath $ReleaseFolder -Force
        $fileSize = [math]::Round((Get-Item $sourcePath).Length / 1KB, 2)
        $sizeText = "$fileSize KB"
        Write-Host "  [OK] $file ($sizeText)" -ForegroundColor Green
    }
    else {
        Write-Host "  [ERROR] Missing required file: $file" -ForegroundColor Red
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
    }
    else {
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
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Packaging Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Show file list
$files = Get-ChildItem $ReleaseFolder
$totalSize = ($files | Measure-Object -Property Length -Sum).Sum
Write-Host "Release folder: $ReleaseFolder" -ForegroundColor Cyan
Write-Host ""
Write-Host "Files:" -ForegroundColor Yellow
foreach ($file in $files) {
    $sizeMB = [math]::Round($file.Length / 1MB, 2)
    Write-Host "  - $($file.Name) ($sizeMB MB)"
}
Write-Host ""
$totalSizeMB = [math]::Round($totalSize / 1MB, 2)
Write-Host "Total size: $totalSizeMB MB" -ForegroundColor Cyan
Write-Host ""

# Auto create ZIP
$ZipPath = "$ReleaseFolder.zip"
if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
}

Write-Host "Creating archive..." -ForegroundColor Yellow
Compress-Archive -Path "$ReleaseFolder\*" -DestinationPath $ZipPath -Force

if (Test-Path $ZipPath) {
    $zipSize = [math]::Round((Get-Item $ZipPath).Length / 1MB, 2)
    Write-Host "  [OK] Archive created: $ZipPath ($zipSize MB)" -ForegroundColor Green
}

Write-Host ""
Write-Host "All done!" -ForegroundColor Green
Write-Host ""
