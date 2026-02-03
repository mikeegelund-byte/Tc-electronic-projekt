$appFolder = Join-Path $env:LOCALAPPDATA 'NovaSystemManager'
$exe = Join-Path $appFolder 'Nova.Presentation.exe'
$shortcutUser = Join-Path $env:USERPROFILE 'Desktop\Nova System Manager.lnk'
$shortcutPublic = 'C:\Users\Public\Desktop\Nova System Manager.lnk'
"AppFolder=$appFolder"
"ExeExists=$([bool](Test-Path $exe))"
"UserDesktopShortcut=$([bool](Test-Path $shortcutUser))"
"PublicDesktopShortcut=$([bool](Test-Path $shortcutPublic))"
if (Test-Path $exe) { (Get-Item $exe).FullName }
