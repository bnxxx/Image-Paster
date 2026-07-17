Add-Type -AssemblyName System.Windows.Forms,System.Drawing
$bmp = New-Object System.Drawing.Bitmap(100, 100)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::Red)
$g.Dispose()

[System.Windows.Forms.Clipboard]::SetImage($bmp)
$bmp.Dispose()

if (-not (Test-Path "test_folder")) {
    New-Item -ItemType Directory -Path "test_folder" | Out-Null
}

& .\bin\ClipboardPaster.exe "C:\proyectos\paster\test_folder"

Write-Host "Files in test_folder after pasting:" -ForegroundColor Cyan
Get-ChildItem -Path "test_folder"
