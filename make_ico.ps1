Add-Type -AssemblyName System.Drawing
$img = [System.Drawing.Image]::FromFile('c:\C sharp\ScreenRecorder\app_icon.png')
$bmp = New-Object System.Drawing.Bitmap($img, 256, 256)
$hIcon = $bmp.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($hIcon)
$fs = [System.IO.File]::Create('c:\C sharp\ScreenRecorder\app.ico')
$icon.Save($fs)
$fs.Close()
$icon.Dispose()
$bmp.Dispose()
$img.Dispose()
Write-Host "Successfully created app.ico"
