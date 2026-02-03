$paths = @(
  'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*',
  'HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*'
)

$items = foreach ($p in $paths) {
  Get-ItemProperty $p -ErrorAction SilentlyContinue
}

$items | Where-Object { $_.DisplayName -eq 'Nova System Manager' } |
  Select-Object DisplayName, DisplayVersion, PSChildName, UninstallString
