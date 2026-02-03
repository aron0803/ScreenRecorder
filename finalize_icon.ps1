Add-Type -AssemblyName System.Drawing
$inputPath = "C:/Users/ron08/.gemini/antigravity/brain/324cbd46-4e19-4fb4-a3f2-e8946c5a80f8/app_icon_v9_refinement_v2_1770084168568.png"
$outputPath = "c:/C sharp/ScreenRecorder/app_icon.png"
$icoPath = "c:/C sharp/ScreenRecorder/app.ico"

$src = [System.Drawing.Image]::FromFile($inputPath)
$size = 512 # Working size
$bmp = New-Object System.Drawing.Bitmap($size, $size)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)

# Create highly rounded rectangle path (radius increased for V8 feel)
$radius = 160
$rect = New-Object System.Drawing.Rectangle(0, 0, $size, $size)
$path = New-Object System.Drawing.Drawing2D.GraphicsPath
$path.AddArc(0, 0, $radius*2, $radius*2, 180, 90)
$path.AddArc($size - $radius*2, 0, $radius*2, $radius*2, 270, 90)
$path.AddArc($size - $radius*2, $size - $radius*2, $radius*2, $radius*2, 0, 90)
$path.AddArc(0, $size - $radius*2, $radius*2, $radius*2, 90, 90)
$path.CloseAllFigures()

$g.SetClip($path)
$g.DrawImage($src, 0, 0, $size, $size)

$bmp.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)

# Also save as ICO
$icoBmp = New-Object System.Drawing.Bitmap($bmp, 256, 256)
$hIcon = $icoBmp.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($hIcon)
$fs = [System.IO.File]::Create($icoPath)
$icon.Save($fs)
$fs.Close()

$icon.Dispose()
$icoBmp.Dispose()
$path.Dispose()
$g.Dispose()
$bmp.Dispose()
$src.Dispose()

Write-Host "Final Icon with Extra Rounded Corners Created!"
