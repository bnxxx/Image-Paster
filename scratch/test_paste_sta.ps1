Add-Type -AssemblyName System.Windows.Forms,System.Drawing

Write-Host "Setting image on clipboard..."
$bmp = New-Object System.Drawing.Bitmap(100, 100)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::Blue)
$g.Dispose()

[System.Windows.Forms.Clipboard]::SetImage($bmp)

$hasImg = [System.Windows.Forms.Clipboard]::ContainsImage()
Write-Host "Clipboard.ContainsImage() in PowerShell: $hasImg"

if (-not (Test-Path "test_folder")) {
    New-Item -ItemType Directory -Path "test_folder" | Out-Null
}

Write-Host "Running ClipboardPaster.exe..."
& .\bin\ClipboardPaster.exe "C:\proyectos\paster\test_folder"

Write-Host "Files in test_folder:"
Get-ChildItem -Path "test_folder"
