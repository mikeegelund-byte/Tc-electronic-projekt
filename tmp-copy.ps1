$src = 'C:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova\publish'
$dst = 'C:\Users\mike_\AppData\Local\NovaSystemManager'

if (-not (Test-Path $dst)) {
  New-Item -ItemType Directory -Path $dst -Force | Out-Null
}

robocopy $src $dst /E /NFL /NDL /NJH /NJS /NC /NS /NP
