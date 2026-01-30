# Arkitektur

## Lagdeling (clean architecture)
```
┌─────────────────────────────────────┐
│   UI Layer (Avalonia)               │
│   - Views (XAML)                    │
│   - ViewModels (INotifyPropertyChanged) │
└───────────────┬─────────────────────┘
                │
┌───────────────▼─────────────────────┐
│   Application Layer                 │
│   - Use Cases / Commands             │
│   - Coordinaters                    │
└───────────────┬─────────────────────┘
                │
┌───────────────▼─────────────────────┐
│   Domain Layer                      │
│   - Preset (ValueObject)            │
│   - EffectBlock (ValueObject)       │
│   - Parameter validation             │
│   - SysEx parsing logic             │
└───────────────┬─────────────────────┘
                │
┌───────────────▼─────────────────────┐
│   MIDI Layer (Infrastructure)       │
│   - IMidiPort (interface)           │
│   - MidiPortImpl (DryWetMIDI wrapper)│
│   - SysEx buffering                 │
│   - Port enumeration                │
└─────────────────────────────────────┘
```

---

## Nøgleprincipper

### 1. Dependency Injection
- All services injected via constructor
- Makes testing easy (swap real MIDI with mock)

### 2. Immutability
- Presets, Parameters are immutable
- Changes create new instances
- Makes undo/redo trivial later

### 3. Result pattern
```csharp
public Result<UserBankDump> ParseBank(byte[] data)
{
    if (!ValidateChecksum(data))
        return Result<UserBankDump>.Failure("Bad checksum");
    
    return Result<UserBankDump>.Success(new UserBankDump(...));
}
```

### 4. Async I/O
- All MIDI operations are `async/await`
- UI never blocks
- Long operations show progress

### 5. Event-driven updates
- MIDI input triggers domain events
- Domain events update UI via ViewModels
- Example: `PresetReceivedEvent`

---

## Key types (Domain)

### Preset
```csharp
public class Preset
{
    public int Number { get; }           // 1-90
    public string Name { get; }          // 24 chars max
    public byte[] RawSysEx { get; }     // 520 bytes
    public EffectBlock[] Blocks { get; } // 15 blocks
    public byte Checksum { get; }
    
    public static Preset FromSysEx(byte[] data);
    public byte[] ToSysEx();
}
```

### UserBankDump
```csharp
public class UserBankDump
{
    public Preset[] Presets { get; } // [0..59]
    
    public static UserBankDump FromSysEx(byte[] data);
    public byte[] ToSysEx();
}
```

### IMidiPort (interface)
```csharp
public interface IMidiPort : IAsyncDisposable
{
    IAsyncEnumerable<byte[]> ReceiveSysExAsync();
    Task SendSysExAsync(byte[] data);
    Task<bool> ConnectAsync(string portName);
    IEnumerable<string> EnumeratePorts();
}
```

---

## Dataflow eksempel (Modul 1: Bank Download)

```
1. User: "Click Download Bank"
   ↓
2. UI → DownloadBankUseCase
   ↓
3. UseCase: 
   - Build "Request Bank Dump" SysEx
   - Send via IMidiPort
   ↓
4. IMidiPort:
   - Buffers incoming SysEx chunks
   - Waits for F7 (end byte)
   ↓
5. Hardware: Sends 60 presets
   ↓
6. IMidiPort: Buffers complete dump
   ↓
7. Domain: Parse UserBankDump
   - Validate each preset checksum
   - Extract names + block data
   ↓
8. Application: Create PresetReceivedEvent
   ↓
9. UI: Update ViewModel
   - Bind to ListView of preset names
   ↓
10. User: Sees "Bank downloaded: 60 presets"
```

---

## Service setup (dependency container)

```csharp
var services = new ServiceCollection();

// Infrastructure
services.AddSingleton<IMidiPort>(sp => 
    new MidiPortImpl(new DryWetMidiAdapter()));

// Application
services.AddScoped<DownloadBankUseCase>();
services.AddScoped<UploadBankUseCase>();

// UI
services.AddScoped<MainViewModel>();

var provider = services.BuildServiceProvider();
```

---

## Error handling strategy
- **SysEx parse error** → User: "Corrupted data, retry"
- **Port not found** → User: "Nova System not detected, check USB"
- **Timeout** → Auto-retry 3x, then: "Nova System not responding"
- **Bad checksum** → Request again (never corrupt user's preset)

---

## Testing layer integration
- `IMidiPort` mocked in all tests
- Fixture: real SysEx dumps from `..\docs\fixtures\`
- Unit tests: parsing + validation
- Integration tests: roundtrip (mock → real data)
