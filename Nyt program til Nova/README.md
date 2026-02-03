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

## Installation

### End Users - Windows Installer (.msi)

Download and run the latest `NovaSystemManager.msi` installer from the releases page.

The installer will:
- Install the application to `C:\Program Files\Nova System Manager\`
- Create Start Menu and Desktop shortcuts
- Enable easy uninstall via Windows Settings

### Developers - Build from Source

See Quick Start section above.

### Building the Installer

To create the Windows installer package:

```powershell
# Install WiX Toolset (one-time setup)
dotnet tool install --global wix

# Build the MSI installer
.\installer\build-installer.ps1
```

The installer will be created at `installer\output\NovaSystemManager.msi`.

For more details, see [installer/README.md](installer/README.md).

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
