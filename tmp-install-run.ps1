$ErrorActionPreference = 'Stop'
$msi = 'C:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova\installer\output\NovaSystemManager.msi'
$log = 'C:\Temp\nova_install.log'
if (-not (Test-Path 'C:\Temp')) { New-Item -ItemType Directory -Path 'C:\Temp' | Out-Null }
$arg = "/i `"$msi`" /qn /l*v `"$log`""
$p = Start-Process -FilePath 'msiexec.exe' -ArgumentList $arg -Wait -PassThru
"EXIT=$($p.ExitCode)"
