$procs = Get-Process msiexec -ErrorAction SilentlyContinue
foreach ($p in $procs) {
  Write-Host ('msiexec pid=' + $p.Id + ' start=' + $p.StartTime)
}
