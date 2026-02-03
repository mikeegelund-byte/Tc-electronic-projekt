$msi = 'C:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova\installer\output\NovaSystemManager.msi'
$args = @('/i', $msi, '/qn')
$p = Start-Process -FilePath 'msiexec.exe' -ArgumentList $args -Wait -PassThru
Write-Host ('ExitCode=' + $p.ExitCode)
