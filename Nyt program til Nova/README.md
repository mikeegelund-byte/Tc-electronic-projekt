# Nova System Controller

Modern replacement for TC Electronic Nova System manager.

## Quick Start

```powershell
# 1. Clone and enter
cd "Nyt program til Nova"

# 2. Build
dotnet build NovaApp.sln

# 3. Test
dotnet test NovaApp.sln

# 4. Run
dotnet run --project src/Nova.Presentation
```

## Project Structure
```
src/
├── Nova.Common/          # Shared utilities, logging
├── Nova.Domain/          # Business logic, entities
├── Nova.Midi/            # MIDI I/O abstraction
├── Nova.Infrastructure/  # DryWetMIDI implementation
├── Nova.Application/     # Use cases, commands
└── Nova.Presentation/    # Avalonia UI
```

## Development
- Follow RED→GREEN→REFACTOR discipline
- All code requires tests first
- See `tasks/` folder for current work items
