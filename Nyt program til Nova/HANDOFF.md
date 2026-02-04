# Handoff - Nova System Manager (2026-02-03)

## Status
- Appen blev ikke synlig for bruger efter launch, og der meldes om ingen synlige ændringer i UI endnu.
- Jeg rettede en crash ved startup: DI manglede registrering af Program Map In/Out ViewModels og use cases.
- Program Map In/Out-funktionalitet findes i repoet (views, viewmodels, tests).
- Ingen tests er kørt i denne handoff.

## Observed Issue
- Appen crashede tidligere på startup pga. manglende DI for ProgramMapInViewModel/ProgramMapOutViewModel. Det er rettet i `src/Nova.Presentation/App.axaml.cs`.
- Efter fix gav `dotnet run` ingen fejl i konsollen, men bruger så stadig ikke vinduet.

## Hvor du skal kigge
- Logs: `%LOCALAPPDATA%\NovaSystemManager\logs\NovaSystemManager-<dato>.log`
- UI: System Settings-tabben indeholder knappen til at hente System Dump. MIDI Mapping-tabben har Program Map In/Out.
- Relevante docs: `MIDI_PROTOCOL.md`, `EFFECT_REFERENCE.md`, `docs/SYSEX_MAP_TABLES.md`

## Næste skridt
1. Relaunch appen og bekræft vinduet:
   `dotnet run --project src\Nova.Presentation\Nova.Presentation.csproj`
2. Hardware test flow:
   - Sæt MIDI in/out korrekt og SysEx ID = 0 på pedalen.
   - I app: Connection-tab -> Connect. System Settings-tab -> Download System Settings.
   - Hvis intet sker, trig på pedalen: `UTILITY -> MIDI -> Send Dump -> System`.
3. Hvis der stadig ikke kommer data:
   - Tjek logfilen.
   - Verificer portvalg i `Nova.Infrastructure\Midi\DryWetMidiPort.cs`.
   - Overvej at øge timeout i `RequestSystemDumpUseCase`.

## Ændrede filer
- `src/Nova.Presentation/App.axaml.cs` (tilføjet DI-registrering for Program Map In/Out)
