# Handover for Codex 5.2 — deeper test & coverage analysis

Dette dokument giver Codex 5.2 (eller en anden agent) alt nødvendigt for at grave dybere i test- og coverage-resultaterne og fortsætte arbejdet i denne VS Code-workspace.

## Kort opsummering af hvad jeg allerede gjorde
- Rensede repo-historikken for store binære filer (JDK zip) og opdaterede `.gitignore`.
- Kørte hele løsningen med coverage (`--collect:"XPlat Code Coverage"`).
- Genererede HTML- og tekst-sammendrag via `reportgenerator`.
- Push af de rensede commits er udført (force push). OBS: historikken blev omskrevet — alle samarbejdspartnere skal re-klone.

## Viktige artefakter og filer
- Coverage HTML summary: `coverage-report/summary.html`
- Coverage text summary: `coverage-report-txt/Summary.txt`
- Cobertura coverage filer (én per testprojekt) findes under hvert testprojekts `TestResults` mappe, f.eks.:
  - `Nyt program til Nova/src/Nova.Presentation.Tests/TestResults/<GUID>/coverage.cobertura.xml`
  - `Nyt program til Nova/src/Nova.Infrastructure.Tests/TestResults/<GUID>/coverage.cobertura.xml`
  - osv.
- Opdateret `.gitignore`: `Nyt program til Nova/.gitignore`

## Miljø (på denne maskine)
- OS: Windows
- .NET SDK: .NET 8 (kørte `dotnet test` succesfuldt)
- ReportGenerator: `dotnet-reportgenerator-globaltool` (installeret globalt)

## Kommandoer til reproduktion (kør i workspace root)
Kør hele løsningen med coverage:
```powershell
dotnet test "Nyt program til Nova\NovaApp.sln" --collect:"XPlat Code Coverage"
```
Generer HTML-rapport fra alle Cobertura-filer:
```powershell
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coverage-report -reporttypes:HtmlSummary
```
Generer tekst-sammendrag (praktisk hvis du vil parse i CI):
```powershell
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:coverage-report-txt -reporttypes:TextSummary
```
Åbn HTML-rapporten lokalt (eks.):
- Fil: `coverage-report/summary.html` — åbn i browser eller i VS Code.

## Højdepunkter fra rapporten (hurtig analyse)
- Total tests: `358` (358 succeeded, 0 failed) — kørsel var succesfuld.
- Line coverage: `45.5%` (1813 / 3981)
- Branch coverage: `51.8%` (460 / 887)
- Method coverage: `69.8%` (278 / 398)

Top hotspots (lav coverage / opmærksomhed):
- `Nova.Presentation` — **19.9%** coverage: mange views/services på `0%` (fx `FileManagerView`, `PresetListView`, `App`, `FileDialogService`) — UI-kode mangler tests eller er svær at teste direkte.
- `Nova.Infrastructure` — **33%** coverage: infrastruktur-adaptere mangler tests.
- `Nova.Midi.Tests` (og Midi-adaptere) — lav coverage i nogle autogenererede/mock-filer.

Stærkt dækkede områder:
- `Nova.Domain` — **91.4%**: domain-logikken er solidt dækket.
- `Nova.Application` — **75.5%** med mange use-cases godt dækket; nogle use-cases (fx `RequestPresetUseCase`) er 0% og bør undersøges.

## Forslag til næste dybere analyser (prioriteret)
1. Fokusér på `Nova.Presentation`: list de viewmodels med lav coverage og skriv unit tests for dem — test viewmodels i stedet for views. Starterforslag:
   - `ProgramMapInViewModel` / `ProgramMapOutViewModel` — der er nye viewmodels og tests i repo; udbyg dem.
   - `FileDialogService` skal mockes og testes for edge-cases.
2. Infrastruktur: tilføj mocks/interfaces for hardware- og port-abstraktioner (`Nova.Infrastructure.Midi.*`) så du kan unit-teste adapters.
3. Identificér filer med mange ubeskrevne linjer ved at åbne `coverage-report/summary.html` og sortere/filtrere for "Uncovered lines".
4. Ekskludér auto-generated XAML/resource-filer fra coverage (kan konfigureres i reportgenerator eller ved at ignorere filer) for at få mere meningsfuld procent.
5. Opret en GitHub Actions workflow som kører `dotnet test --collect:"XPlat Code Coverage"` og bruger `reportgenerator` til at publicere coverage HTML som artifacts.

## Task-for Codex 5.2 — konkrete trin
1. Klon repo frisk (pga. historik-ændring):
```powershell
git clone https://github.com/mikeegelund-byte/Tc-electronic-projekt.git
cd "Tc electronic projekt\Nyt program til Nova"
```
2. Reproducer coverage-run for målprojekt (f.eks. `Nova.Presentation.Tests` hvis det er tungest):
```powershell
dotnet test "src\Nova.Presentation.Tests\Nova.Presentation.Tests.csproj" --collect:"XPlat Code Coverage"
reportgenerator -reports:"src\**\TestResults\**\coverage.cobertura.xml" -targetdir:coverage-report-presentation -reporttypes:Html
```
3. Åbn `coverage-report-presentation/index.htm` i browser og noter hvilke klasser/metoder der har mest ubeskyttet kode.
4. For en valgt viewmodel (fx `ProgramMapInViewModel`): skriv en unit-test der mocker afhængigheder (brug `Moq` eller indbyggede mocks), kør `dotnet test` lokalt og se coverage-ændring.

## Praktiske noter / advarsler
- Repo-historik er omskrevet (jeg force-pushed). Alle andre udviklere SKAL re-klone eller køre `git fetch` + hard reset mod ny `origin/main` for at undgå divergens.
- De store JDK-artefakter blev fjernet fra historikken og er nu undtaget i `.gitignore`. Hvis projektet stadig skal bruge en JDK-distribution i `tools/`, overvej at hoste zip-filen på en artefakt-server eller bruge Git LFS.

## Filer / steder jeg brugte
- Coverage HTML: `coverage-report/summary.html`
- Coverage TXT: `coverage-report-txt/Summary.txt`
- Cobertura filer: `src/*/TestResults/*/coverage.cobertura.xml`
- `.gitignore`: `Nyt program til Nova/.gitignore`

---
Hvis du vil, kan jeg nu:
- Oprette en PR med et eksempel unit-test for `ProgramMapInViewModel` og vise coverage-forbedring.
- Oprette en GitHub Actions workflow YAML til test+coverage.
- Eller levere en prioriteret liste (top 20) af lav-dækkede klasser fra `Summary.txt` for at guide Codex 5.2.

Sig hvilken af de næste handlinger du vil have mig til at gøre — så implementerer jeg den og opdaterer handover hvis nødvendigt.
