# Stack & Tooling

## Valgt stack
| Komponent | Valg | Version |
|-----------|------|---------|
| **Sprog** | C# | .NET 8 LTS |
| **UI Framework** | Avalonia | 11.x |
| **MIDI Lib** | DryWetMIDI | 7.x |
| **Test Framework** | xUnit | 2.6.x |
| **Mocking** | Moq | 4.18.x |
| **Logger** | Serilog | 3.x |
| **OS (Primary)** | Windows 11 | Build 22621+ |
| **OS (Future)** | macOS | 11.0+ (forberedt) |

---

## Hvorfor denne stack

### C#
- Mature, moderne sproget
- Stærk typing + LINQ
- Excellente debugging tools
- God performance (JIT compiled)

### Avalonia
- Cross-platform (Win/Mac/Linux native)
- XAML-baseret UI
- Tema/styling muligheder (Apple-agtig UI mulig)
- Aktiv community

### DryWetMIDI
- Fuldt out-of-the-box support for SysEx
- Robust parsing og validering
- Godt dokumenteret
- Maintenant aktiv

### xUnit + Moq
- Standard i .NET øko
- Nem mockingDem MIDI interface
- Good CI/CD integration

---

## Udviklingsmiljø setup (forventet)
1. **Visual Studio Community 2022** (gratis)
   - .NET desktop development workload
   - C# extension

2. **.NET 8 SDK** (Windows installer)

3. **Git** (Windows installer)

4. **NuGet packages** (automatisk download via Visual Studio)

---

## Projektstruktur (forventet)
```
NovaApp/
├── NovaApp.UI/                  # Avalonia UI, XAML, ViewModels
├── NovaApp.Application/         # Use cases, commands
├── NovaApp.Domain/              # Presets, params, validering
├── NovaApp.Midi/                # DryWetMIDI wrapper, I/O
├── NovaApp.Infrastructure/      # File I/O, config
├── NovaApp.Tests/               # xUnit + Moq test suite
└── NovaApp.sln                  # Solution file
```

---

## Kodestil & Konventioner

### Arkitektur (lagdeling)
```
UI Layer (Avalonia)
    ↕ ViewModels (observerable)
Application Layer (Use cases)
    ↕ (interfaces)
Domain Layer (Presets, params)
    ↕ (interfaces)
MIDI Layer (I/O)
    ↕
Hardware
```

**Regel:** Ingen MIDI-logik i UI. Alt I/O går gennem interfaces.

### Naming
- Classes: `PascalCase` (fx `UserBankDump`)
- Methods: `PascalCase` (fx `ParseSysEx()`)
- Private fields: `_camelCase`
- Constants: `UPPER_SNAKE_CASE`
- Async methods: suffix `Async` (fx `ConnectAsync()`)

### Error handling
- Exceptions for exceptional cases (hardware failure, checksum error)
- Result<T> pattern for expected failures (invalid port name)

---

## Pakker (NuGet) - initial liste
- Avalonia
- DryWetMIDI
- xUnit
- Moq
- Serilog (logging)
- FluentAssertions (test assertions)

---

## Build & test commands
```bash
# Build
dotnet build

# Run tests
dotnet test

# Run app
dotnet run --project NovaApp.UI

# Package for release
dotnet publish -c Release -r win-x64 --self-contained
```

---

## CI/CD pipeline (senere)
- GitHub Actions
- Auto-run tests on PR
- Build matrix (Win x64)

---

## Versionering
- SemVer (MAJOR.MINOR.PATCH)
- Tagged releases på GitHub
- AssemblyVersion i .csproj
