<#
  Manual backup script for local-first workflow.
  Creates a `backups/` folder and writes a git bundle containing all refs.
  Usage: Open PowerShell in repo root and run: .\scripts\backup-git.ps1
#>
Param(
    [string]$OutputDir = "backups",
    [string]$NamePrefix = "repo-backup"
)

$now = Get-Date -Format yyyyMMdd_HHmmss
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$outDir = Join-Path $repoRoot $OutputDir
If (-Not (Test-Path $outDir)) { New-Item -ItemType Directory -Path $outDir | Out-Null }

$bundleName = "$NamePrefix-$now.bundle"
$bundlePath = Join-Path $outDir $bundleName

Write-Host "Creating git bundle: $bundlePath"
git bundle create "$bundlePath" --all

If ($LASTEXITCODE -eq 0) {
    Write-Host "Bundle created: $bundlePath"
    Write-Host "(You can copy this file offsite for safe backup.)"
} else {
    Write-Error "Failed to create git bundle. Exit code: $LASTEXITCODE"
}

# Optional: show instructions to push to remote backup if wanted
Write-Host "To push to the GitHub backup manually, run:"
Write-Host "  git remote set-url --push backup https://github.com/your/repo.git"
Write-Host "  git push backup main"
