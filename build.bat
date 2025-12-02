@echo off
REM ScreenRecorder Quick Build Script
REM 快速編譯腳本 - 呼叫 PowerShell 編譯腳本

echo ========================================
echo   ScreenRecorder 快速編譯
echo ========================================
echo.

REM Ask for release
set /p publish="Publish to GitHub (Bump Version & Push)? (Y/N): "
if /i "%publish%"=="Y" (
    powershell.exe -ExecutionPolicy Bypass -File "%~dp0build.ps1" -Publish %*
) else (
    powershell.exe -ExecutionPolicy Bypass -File "%~dp0build.ps1" %*
)

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo 編譯失敗!
    pause
    exit /b %ERRORLEVEL%
)

echo.
pause
