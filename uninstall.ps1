# PowerShell One-Line Uninstaller for Clipboard Image Paster
# Usage: irm https://raw.githubusercontent.com/bnxxx/Image-Paster/main/uninstall.ps1 | iex

$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$AppName = "ClipboardPaster"
$InstallDir = "$env:LOCALAPPDATA\Programs\ImagePaster"
$ExePath = Join-Path $InstallDir "$AppName.exe"

Write-Host "===================================================" -ForegroundColor Cyan
Write-Host "Uninstalling Clipboard Image Paster..." -ForegroundColor Cyan
Write-Host "===================================================" -ForegroundColor Cyan

if (Test-Path $ExePath) {
    Write-Host "Removing context menu entries from registry..." -ForegroundColor Yellow
    try {
        & $ExePath --uninstall
    } catch {
        Write-Host "[WARNING] Could not execute --uninstall: $_" -ForegroundColor DarkYellow
    }
} else {
    # If executable isn't found, check if local bin copy exists just to run --uninstall
    $LocalBinPath = Join-Path $PSScriptRoot "bin\$AppName.exe"
    if (Test-Path $LocalBinPath) {
        & $LocalBinPath --uninstall
    }
}

if (Test-Path $InstallDir) {
    Write-Host "Removing installation directory ($InstallDir)..." -ForegroundColor Yellow
    Remove-Item -Path $InstallDir -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "`n[SUCCESS] Clipboard Image Paster has been uninstalled successfully.`n" -ForegroundColor Green
