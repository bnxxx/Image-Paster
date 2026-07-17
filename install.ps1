# PowerShell One-Line Installer for Clipboard Image Paster
# Usage: irm https://raw.githubusercontent.com/bnxxx/Image-Paster/main/install.ps1 | iex

param(
    [switch]$Uninstall
)

$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$AppName = "ClipboardPaster"
$InstallDir = "$env:LOCALAPPDATA\Programs\ImagePaster"
$ExePath = Join-Path $InstallDir "$AppName.exe"
$RepoUrl = "https://github.com/bnxxx/Image-Paster"
$DownloadUrl = "$RepoUrl/releases/latest/download/$AppName.exe"

# Handle Uninstall
if ($Uninstall) {
    Write-Host "===================================================" -ForegroundColor Cyan
    Write-Host "Uninstalling Clipboard Image Paster..." -ForegroundColor Cyan
    Write-Host "===================================================" -ForegroundColor Cyan

    if (Test-Path $ExePath) {
        Write-Host "Removing context menu entries..." -ForegroundColor Yellow
        try {
            & $ExePath --uninstall
        } catch {
            Write-Host "[WARNING] Could not execute --uninstall: $_" -ForegroundColor DarkYellow
        }
    }

    if (Test-Path $InstallDir) {
        Write-Host "Removing installation directory ($InstallDir)..." -ForegroundColor Yellow
        Remove-Item -Path $InstallDir -Recurse -Force -ErrorAction SilentlyContinue
    }

    Write-Host "`n[SUCCESS] Clipboard Image Paster has been uninstalled successfully.`n" -ForegroundColor Green
    return
}

Write-Host "===================================================" -ForegroundColor Cyan
Write-Host "Installing Clipboard Image Paster..." -ForegroundColor Cyan
Write-Host "===================================================" -ForegroundColor Cyan

# Create install directory if it doesn't exist
if (-not (Test-Path $InstallDir)) {
    Write-Host "Creating installation directory: $InstallDir" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null
}

# Check if installing from local directory (e.g. cloned repo or local build)
$ScriptDir = if (![string]::IsNullOrEmpty($PSScriptRoot)) { $PSScriptRoot } else { (Get-Location).Path }
$LocalBinPath = Join-Path $ScriptDir "bin\$AppName.exe"
$LocalRootPath = Join-Path $ScriptDir "$AppName.exe"

if (Test-Path $LocalBinPath) {
    Write-Host "Found local build at $LocalBinPath. Copying..." -ForegroundColor Green
    Copy-Item -Path $LocalBinPath -Destination $ExePath -Force
} elseif (Test-Path $LocalRootPath) {
    Write-Host "Found local executable at $LocalRootPath. Copying..." -ForegroundColor Green
    Copy-Item -Path $LocalRootPath -Destination $ExePath -Force
} else {
    Write-Host "Downloading latest $AppName.exe from GitHub Releases..." -ForegroundColor Yellow
    Write-Host "URL: $DownloadUrl" -ForegroundColor DarkGray
    try {
        Invoke-WebRequest -Uri $DownloadUrl -OutFile $ExePath -UseBasicParsing
    } catch {
        Write-Host "`n[ERROR] Failed to download $AppName.exe from GitHub Releases." -ForegroundColor Red
        Write-Host "Details: $_" -ForegroundColor Red
        Write-Host "Make sure a release exists on GitHub: $RepoUrl/releases" -ForegroundColor DarkYellow
        exit 1
    }
}

# Verify binary exists
if (-not (Test-Path $ExePath)) {
    Write-Host "[ERROR] Installation file not found at $ExePath" -ForegroundColor Red
    exit 1
}

# Register right-click context menu options silently
Write-Host "Registering Windows right-click context menu entries..." -ForegroundColor Yellow
try {
    & $ExePath --install
} catch {
    Write-Host "[ERROR] Failed to register context menu entries: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n===================================================" -ForegroundColor Green
Write-Host "[SUCCESS] Clipboard Image Paster installed successfully!" -ForegroundColor Green
Write-Host "===================================================" -ForegroundColor Green
Write-Host "Installed location: $ExePath" -ForegroundColor Cyan
Write-Host "`nYou can now right-click any folder icon or open folder background in Windows Explorer" -ForegroundColor White
Write-Host "and select:" -ForegroundColor White
Write-Host "  - Paste clipboard image" -ForegroundColor Yellow
Write-Host "  - Paste image & transcribe text (OCR)" -ForegroundColor Yellow
Write-Host "`nTo uninstall anytime, run:" -ForegroundColor DarkGray
Write-Host "  irm https://raw.githubusercontent.com/bnxxx/Image-Paster/main/uninstall.ps1 | iex" -ForegroundColor DarkGray
Write-Host "  (or run '$ExePath --uninstall')" -ForegroundColor DarkGray
Write-Host ""
