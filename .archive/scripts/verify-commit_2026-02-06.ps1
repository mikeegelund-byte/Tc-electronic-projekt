param()

$ErrorActionPreference = "Stop"

function Write-Info($msg) { Write-Host $msg -ForegroundColor Cyan }
function Write-Ok($msg) { Write-Host $msg -ForegroundColor Green }
function Write-Err($msg) { Write-Host $msg -ForegroundColor Red }

Write-Info "Running verification gates..."

$solutionPath = "NovaApp.sln"
$csprojCount = Get-ChildItem -Recurse -Filter "*.csproj" -ErrorAction SilentlyContinue | Measure-Object | Select-Object -ExpandProperty Count

if (-not (Test-Path $solutionPath) -and $csprojCount -eq 0) {
    Write-Info "No solution/projects found yet. Skipping build/test gates."
    exit 0
}

# Build gate
Write-Info "Gate: dotnet build"
& dotnet build $solutionPath --no-incremental --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Err "Build failed."
    exit 1
}
Write-Ok "Build passed."

# Coverage thresholds per test project
$coverageThresholds = @{
    "Nova.Domain.Tests" = 95
    "Nova.Application.Tests" = 80
    "Nova.Infrastructure.Tests" = 70
    "Nova.Presentation.Tests" = 50
}

$testProjects = Get-ChildItem -Recurse -Filter "*.csproj" -Path "tests" -ErrorAction SilentlyContinue
if (-not $testProjects) {
    Write-Info "No test projects found. Skipping test gate."
    exit 0
}

foreach ($proj in $testProjects) {
    $name = $proj.BaseName
    $threshold = $coverageThresholds[$name]
    if (-not $threshold) { $threshold = 70 }

    Write-Info "Gate: dotnet test $name (coverage >= $threshold%)"
    & dotnet test $proj.FullName --no-build --verbosity quiet `
        /p:CollectCoverage=true `
        /p:CoverletOutputFormat=opencover `
        /p:ThresholdType=line `
        /p:Threshold=$threshold

    if ($LASTEXITCODE -ne 0) {
        Write-Err "Tests or coverage failed for $name."
        exit 1
    }

    Write-Ok "Tests passed for $name."
}

Write-Ok "All verification gates passed."
exit 0
