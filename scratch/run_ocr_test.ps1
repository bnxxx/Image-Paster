$path = (Resolve-Path .).Path + "\scratch\test_ocr.png"

Add-Type -AssemblyName System.Drawing
$bmp = New-Object System.Drawing.Bitmap(400, 100)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::White)
$font = New-Object System.Drawing.Font('Arial', 24)
$g.DrawString('Hello Antigravity OCR', $font, [System.Drawing.Brushes]::Black, 20, 30)
$bmp.Save($path)
$g.Dispose()
$bmp.Dispose()

Add-Type -AssemblyName System.Runtime.WindowsRuntime
[Windows.Media.Ocr.OcrEngine, Windows.Foundation, ContentType=WindowsRuntime] | Out-Null
[Windows.Storage.StorageFile, Windows.Foundation, ContentType=WindowsRuntime] | Out-Null
[Windows.Storage.FileAccessMode, Windows.Foundation, ContentType=WindowsRuntime] | Out-Null
[Windows.Graphics.Imaging.BitmapDecoder, Windows.Foundation, ContentType=WindowsRuntime] | Out-Null

$engine = [Windows.Media.Ocr.OcrEngine]::TryCreateFromUserProfileLanguages()

$fileOp = [Windows.Storage.StorageFile]::GetFileFromPathAsync($path)
$file = [System.WindowsRuntimeSystemExtensions]::AsTask($fileOp).Result

$streamOp = $file.OpenAsync([Windows.Storage.FileAccessMode]::Read)
$stream = [System.WindowsRuntimeSystemExtensions]::AsTask($streamOp).Result

$decoderOp = [Windows.Graphics.Imaging.BitmapDecoder]::CreateAsync($stream)
$decoder = [System.WindowsRuntimeSystemExtensions]::AsTask($decoderOp).Result

$bmpOp = $decoder.GetSoftwareBitmapAsync()
$softwareBitmap = [System.WindowsRuntimeSystemExtensions]::AsTask($bmpOp).Result

$ocrOp = $engine.RecognizeAsync($softwareBitmap)
$result = [System.WindowsRuntimeSystemExtensions]::AsTask($ocrOp).Result

Write-Host ("Transcribed: " + $result.Text)
