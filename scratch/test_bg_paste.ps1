Add-Type -AssemblyName System.Windows.Forms,System.Drawing

Write-Host "Setting green test image on clipboard..."
$bmp = New-Object System.Drawing.Bitmap(100, 100)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::Green)
$g.Dispose()

[System.Windows.Forms.Clipboard]::SetImage($bmp)

if (-not (Test-Path "test_bg_folder")) {
    New-Item -ItemType Directory -Path "test_bg_folder" | Out-Null
}

Push-Location "test_bg_folder"
Write-Host "Simulating right-click inside open folder background (passing %V literal or .)..."
& ..\bin\ClipboardPaster.exe "%V"
Pop-Location

Write-Host "Files generated inside test_bg_folder:"
Get-ChildItem -Path "test_bg_folder"
