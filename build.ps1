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

$winMeta = "C:\Windows\System32\WinMetadata"
$sysRuntime = "C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Runtime\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Runtime.dll"
$ocrRefs = @()

if ((Test-Path "$winMeta\Windows.Media.winmd") -and (Test-Path $sysRuntime)) {
    $ocrRefs = @(
        "/reference:$sysRuntime",
        "/reference:$winMeta\Windows.Media.winmd",
        "/reference:$winMeta\Windows.Graphics.winmd",
        "/reference:$winMeta\Windows.Foundation.winmd",
        "/reference:$winMeta\Windows.Storage.winmd",
        "/reference:$winMeta\Windows.Globalization.winmd"
    )
}

& $cscPath /nologo /target:winexe /optimize+ /out:bin\ClipboardPaster.exe /reference:System.Windows.Forms.dll /reference:System.Drawing.dll $ocrRefs src\*.cs

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Compilation failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`n[SUCCESS] Compiled successfully to: bin\ClipboardPaster.exe`n" -ForegroundColor Green
Write-Host "To run the GUI setup:" -ForegroundColor Yellow
Write-Host "  .\bin\ClipboardPaster.exe`n"
Write-Host "To install directly from command line:" -ForegroundColor Yellow
Write-Host "  .\bin\ClipboardPaster.exe --install`n"
