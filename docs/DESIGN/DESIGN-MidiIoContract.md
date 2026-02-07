# MIDI I/O Kontrakt (Interface)

## IMidiPort interface (definition)
```csharp
public sealed record MidiPortSelection(string InputPortName, string OutputPortName);

public interface IMidiPort
{
    /// Connect to named input/output ports
    Task<Result> ConnectAsync(MidiPortSelection selection);
    
    /// Disconnect
    Task<Result> DisconnectAsync();
    
    /// Send SysEx, get result
    Task<Result> SendSysExAsync(byte[] data);
    
    /// Receive SysEx as stream (buffered)
    IAsyncEnumerable<byte[]> ReceiveSysExAsync();

    /// Receive CC messages (3 bytes)
    IAsyncEnumerable<byte[]> ReceiveCCAsync();
    
    /// List available ports
    IReadOnlyList<string> GetAvailableInputPorts();
    IReadOnlyList<string> GetAvailableOutputPorts();
    
    /// Connection state
    bool IsConnected { get; }
    string? InputPortName { get; }
    string? OutputPortName { get; }
    string Name { get; } // "IN: X / OUT: Y"
}
```

---

## Input contract (Hardware → App)

### Modtag User Bank Dump
**Message:** F0 00 20 1F [ID] 63 20 03 [60 × Preset...] [Checksum] F7

**Behavior:**
1. Hardware sender 60 presets, nogle gange splittet i chunks
2. `ReceiveSysExAsync()` buffers bytes
3. Returns complete F0...F7 når F7 modtaget
4. Each preset is 520 bytes

**Timeout:** Max 30 sekunder for helt dump (60 × 520 bytes)

**Fejl:**
- Hvis F7 ikke kommer: timeout error efter 30s
- Hvis checksum fail: log warning, return raw data (app validates)

### Modtag System Dump
**Message:** F0 00 20 1F [ID] 63 20 02 [System data...] [Checksum] F7

**Behavior:** Samme som Bank Dump, men mindre datamængde (~500 bytes)

---

## Output contract (App → Hardware)

### Send User Bank Dump
**Action:** App calls `SendSysExAsync(byte[] bankDump)`

**Krav:**
1. Data must be valid SysEx (start with F0, end with F7)
2. Checksum must be pre-calculated by app
3. Send atomically (no chunking on sender side)

**Timeout:** 5 secondsmax til hardware confirm

**Fejl:**
- Port not connected: return `Result.Failure("Not connected")`
- Port busy: retry once, then return `Result.Failure("Port busy")`

### Send Program Change (Modul 2+)
**Action:** `SendProgramChangeAsync(byte programNumber)`

**Opbygning:** 0xC0 [Program] (2 bytes)

---

## Buffering strategy (intern)

### Incoming SysEx
```csharp
// Pseudocode
var buffer = new List<byte>();
foreach byte in incomingData:
    buffer.Add(byte);
    if byte == 0xF7:  // End marker
        yield return buffer.ToArray();
        buffer.Clear();
```

**Why:** USB-MIDI kan splitte store messages (>64 bytes) i chunks.

### Timeout strategy
```csharp
// If no F7 within 30s, yield error
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
try {
    await ReceiveWithTimeoutAsync(cts.Token);
} catch (OperationCanceledException) {
    yield Error("SysEx timeout - Nova System not responding");
}
```

---

## Port enumeration

### Windows 11 specifics
- Input ports and output ports are separate device lists
- Port names can be: "MIDI 0" / "MIDI 1", "Nova System MIDI Out", or generic "USB MIDI device"
- App must show **two** dropdowns:
  - MIDI OUT → pedalens MIDI IN
  - MIDI IN → pedalens MIDI OUT

**App must handle:** Display friendly names, allow manual pairing, and avoid swapping IN/OUT.

---

## Error handling matrix

| Scenario | Action | UI message |
|----------|--------|------------|
| Port not found | Return Failure | "Nova System not detected" |
| SysEx checksum fail | Log + return raw | Warning badge (user approves) |
| Timeout | Retry 3x | "Waiting..." → "Timeout, retry?" |
| Port already open | Return Failure | "Another app has MIDI port" |
| Send on closed port | Return Failure | "Not connected" |
| Malformed SysEx | Return Failure | "Invalid SysEx format" |

---

## Mock implementation (for tests)

```csharp
public class MockMidiPort : IMidiPort
{
    private readonly byte[][] _responseData;
    
    public async IAsyncEnumerable<byte[]> ReceiveSysExAsync()
    {
        foreach (var data in _responseData)
        {
            await Task.Delay(10); // Simulate network delay
            yield return data;
        }
    }
    
    public Task<Result<Unit>> SendSysExAsync(byte[] data)
    {
        // Validate format
        if (data[0] != 0xF0 || data[^1] != 0xF7)
            return Task.FromResult(Result<Unit>.Failure("Invalid SysEx"));
        
        return Task.FromResult(Result<Unit>.Success());
    }
}
```

---

## Real implementation (DryWetMIDI wrapper)

**Class:** `MidiPortImpl`

```csharp
public class MidiPortImpl : IMidiPort
{
    private readonly DryWetMidiAdapter _adapter;
    private IMidiInDevice? _inputDevice;
    private IMidiOutDevice? _outputDevice;
    
    public async Task<Result<Unit>> ConnectAsync(string portName)
    {
        try {
            var inputName = _adapter.EnumerateInputs()
                .FirstOrDefault(p => p.Contains("Nova") && p.Contains(portName));
            
            _inputDevice = _adapter.OpenInput(inputName);
            _outputDevice = _adapter.OpenOutput(inputName);
            
            return Result<Unit>.Success();
        } catch (Exception ex) {
            return Result<Unit>.Failure($"Connect failed: {ex.Message}");
        }
    }
}
```

---

## Performance requirements
- Latency: <100ms round-trip (soft, nice-to-have)
- Throughput: 60 presets in <15 seconds (acceptable)
- No UI freeze during MIDI ops (all async)

---

## Compatibility
- **USB-MIDI converters:** tested with common brands (FH-MIDI, Behringer UMO202)
- **Direct ports:** if Nova has USB native MIDI (future hardware)
- **Windows drivers:** assume installed + working
