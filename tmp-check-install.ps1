$paths = @(
  'C:\Program Files\NovaSystemManager\Nova.Presentation.exe',
  'C:\Program Files (x86)\NovaSystemManager\Nova.Presentation.exe'
)

foreach ($p in $paths) {
  if (Test-Path $p) {
    $item = Get-Item $p
    Write-Host ($p + ' exists, size=' + $item.Length)
  } else {
    Write-Host ($p + ' missing')
  }
}
