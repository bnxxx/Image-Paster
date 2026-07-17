# PowerShell Build Script for ClipboardPaster
Write-Host "===================================================" -ForegroundColor Cyan
Write-Host "Building ClipboardPaster.exe using pre-installed csc.exe..." -ForegroundColor Cyan
Write-Host "===================================================" -ForegroundColor Cyan

$cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if (-not (Test-Path $cscPath)) {
    Write-Host "[ERROR] csc.exe not found at $cscPath" -ForegroundColor Red
    Write-Host "Please make sure Microsoft .NET Framework 4.0/4.8 is installed on Windows." -ForegroundColor Red
    exit 1
}

if (-not (Test-Path "bin")) {
    New-Item -ItemType Directory -Path "bin" | Out-Null
}

& $cscPath /nologo /target:winexe /optimize+ /out:bin\ClipboardPaster.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll src\*.cs

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Compilation failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`n[SUCCESS] Compiled successfully to: bin\ClipboardPaster.exe`n" -ForegroundColor Green
Write-Host "To run the GUI setup:" -ForegroundColor Yellow
Write-Host "  .\bin\ClipboardPaster.exe`n"
Write-Host "To install directly from command line:" -ForegroundColor Yellow
Write-Host "  .\bin\ClipboardPaster.exe --install`n"
