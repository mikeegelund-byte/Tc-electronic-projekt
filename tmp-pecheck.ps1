$paths = @(
  'C:\Program Files\NovaSystemManager\Nova.Presentation.exe',
  'C:\Program Files\NovaSystemManager\libSkiaSharp.dll',
  'C:\Program Files\NovaSystemManager\libHarfBuzzSharp.dll',
  'C:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova\publish\Nova.Presentation.exe',
  'C:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova\publish\libSkiaSharp.dll',
  'C:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova\publish\libHarfBuzzSharp.dll'
)

function Get-PeMachine([string]$Path) {
  $fs = [IO.File]::OpenRead($Path)
  $br = New-Object IO.BinaryReader($fs)
  $fs.Seek(0x3C, [IO.SeekOrigin]::Begin) | Out-Null
  $peOffset = $br.ReadInt32()
  $fs.Seek($peOffset + 4, [IO.SeekOrigin]::Begin) | Out-Null
  $machine = $br.ReadUInt16()
  $br.Close()
  $fs.Close()
  switch ($machine) {
    0x14c { 'x86' }
    0x8664 { 'x64' }
    0xaa64 { 'ARM64' }
    default { ('0x{0:X}' -f $machine) }
  }
}

foreach ($p in $paths) {
  if (Test-Path $p) {
    $arch = Get-PeMachine $p
    Write-Host ($p + ' => ' + $arch)
  } else {
    Write-Host ($p + ' => missing')
  }
}
