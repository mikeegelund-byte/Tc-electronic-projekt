# Task List: Modul 1, Phase 4 — Infrastructure (MIDI Driver)

## 📋 Module: 1 (MVP - Connection + Bank Dump)
## 📋 Phase: 4 (Infrastructure layer - real MIDI implementation)
**Duration**: 3-4 days  
**Prerequisite**: Phase 3 complete (Use Cases done)  
**Output**: Working DryWetMIDI implementation of IMidiPort  

---

## Overview

**Goal**: Implement the real MIDI driver using DryWetMIDI library so the app can actually communicate with hardware.

**KRITISK**: Uden denne fase kan appen IKKE tale med pedalen. Alt er pt. mocks.

---

## Exit Criteria (Phase 4 Complete When ALL True)

- [x] `DryWetMidiPort` class implements `IMidiPort`
- [x] Can enumerate available MIDI ports
- [x] Can connect to a named port
- [x] Can receive SysEx messages via IAsyncEnumerable
- [x] Can send SysEx messages
- [x] All tests pass (unit + integration with mock)
- [x] Manual test: Connect to real Nova System pedal

---

## Task 4.1: Install DryWetMIDI NuGet Package

**🟢 COMPLEXITY: TRIVIAL** — Kan udføres af enhver model

**Status**: Not started  
**Estimated**: 5 min  
**Files**:
- `src/Nova.Infrastructure/Nova.Infrastructure.csproj`

### Steps
```powershell
cd src/Nova.Infrastructure
dotnet add package Melanchall.DryWetMIDI
```

### Verification
```xml
<!-- Nova.Infrastructure.csproj should contain: -->
<PackageReference Include="Melanchall.DryWetMIDI" Version="7.*" />
```

---

## Task 4.2: Create DryWetMidiPort Class Structure

**🟡 COMPLEXITY: MEDIUM** — Kræver forståelse af DryWetMIDI API

**Status**: Not started  
**Estimated**: 30 min  
**Files**:
- `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs`
- `src/Nova.Infrastructure.Tests/Midi/DryWetMidiPortTests.cs`

### Test First (RED)
```csharp
public class DryWetMidiPortTests
{
    [Fact]
    public void DryWetMidiPort_Implements_IMidiPort()
    {
        var port = new DryWetMidiPort();
        port.Should().BeAssignableTo<IMidiPort>();
    }

    [Fact]
    public void Name_BeforeConnect_ReturnsEmpty()
    {
        var port = new DryWetMidiPort();
        port.Name.Should().BeEmpty();
    }

    [Fact]
    public void IsConnected_BeforeConnect_ReturnsFalse()
    {
        var port = new DryWetMidiPort();
        port.IsConnected.Should().BeFalse();
    }
}
```

### Code (GREEN)
```csharp
using FluentResults;
using Melanchall.DryWetMidi.Multimedia;
using Nova.Midi;

namespace Nova.Infrastructure.Midi;

public sealed class DryWetMidiPort : IMidiPort, IDisposable
{
    private InputDevice? _inputDevice;
    private OutputDevice? _outputDevice;
    
    public string Name { get; private set; } = string.Empty;
    public bool IsConnected => _inputDevice != null && _outputDevice != null;

    // ... implementation follows in subsequent tasks
}
```

---

## Task 4.3: Implement GetAvailablePorts Static Method

**🟢 COMPLEXITY: SIMPLE** — Direkte DryWetMIDI API kald

**Status**: Not started  
**Estimated**: 20 min  
**Files**:
- `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs`

### Test First (RED)
```csharp
[Fact]
public void GetAvailablePorts_ReturnsListOfPortNames()
{
    // This test may return empty list if no MIDI devices connected
    var ports = DryWetMidiPort.GetAvailablePorts();
    ports.Should().NotBeNull();
    // ports.Should().Contain(p => p.Contains("USB")); // Only if device connected
}
```

### Code (GREEN)
```csharp
public static IReadOnlyList<string> GetAvailablePorts()
{
    var inputs = InputDevice.GetAll().Select(d => d.Name).ToList();
    var outputs = OutputDevice.GetAll().Select(d => d.Name).ToList();
    
    // Return ports that have both input AND output (bidirectional)
    return inputs.Intersect(outputs).ToList();
}
```

---

## Task 4.4: Implement ConnectAsync Method

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5 eller højere

**Årsag**: Fejlhåndtering, resource management, async patterns med DryWetMIDI

**Status**: Not started  
**Estimated**: 45 min  
**Files**:
- `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs`
- `src/Nova.Infrastructure.Tests/Midi/DryWetMidiPortTests.cs`

### Test First (RED)
```csharp
[Fact]
public async Task ConnectAsync_WithInvalidPort_ReturnsFailure()
{
    var port = new DryWetMidiPort();
    var result = await port.ConnectAsync("NonExistent Port 12345");
    result.IsFailed.Should().BeTrue();
}

[Fact]
public async Task ConnectAsync_WithValidPort_SetsIsConnectedTrue()
{
    // This test requires a real MIDI device - mark as [Trait("Category", "Hardware")]
    // Skip in CI, run manually
}
```

### Code (GREEN)
```csharp
public Task<Result> ConnectAsync(string portName)
{
    try
    {
        var inputDevices = InputDevice.GetAll();
        var outputDevices = OutputDevice.GetAll();

        var input = inputDevices.FirstOrDefault(d => d.Name == portName);
        var output = outputDevices.FirstOrDefault(d => d.Name == portName);

        if (input == null || output == null)
            return Task.FromResult(Result.Fail($"MIDI port '{portName}' not found"));

        _inputDevice = input;
        _outputDevice = output;
        
        _inputDevice.StartEventsListening();
        Name = portName;

        return Task.FromResult(Result.Ok());
    }
    catch (Exception ex)
    {
        return Task.FromResult(Result.Fail($"Failed to connect: {ex.Message}"));
    }
}
```

---

## Task 4.5: Implement DisconnectAsync Method

**🟢 COMPLEXITY: SIMPLE** — Resource cleanup

**Status**: Not started  
**Estimated**: 15 min  
**Files**:
- `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs`

### Code (GREEN)
```csharp
public Task<Result> DisconnectAsync()
{
    try
    {
        _inputDevice?.StopEventsListening();
        _inputDevice?.Dispose();
        _outputDevice?.Dispose();
        
        _inputDevice = null;
        _outputDevice = null;
        Name = string.Empty;

        return Task.FromResult(Result.Ok());
    }
    catch (Exception ex)
    {
        return Task.FromResult(Result.Fail($"Failed to disconnect: {ex.Message}"));
    }
}

public void Dispose()
{
    DisconnectAsync().GetAwaiter().GetResult();
}
```

---

## Task 4.6: Implement SendSysExAsync Method

**🟡 COMPLEXITY: MEDIUM** — DryWetMIDI SysEx specifik håndtering

**Status**: Not started  
**Estimated**: 30 min  
**Files**:
- `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs`

### Vigtig Note fra PITFALLS_FOUND.md:
> DryWetMIDI auto-adds F0/F7, expects data WITHOUT them for SendEvent()

### Code (GREEN)
```csharp
public Task<Result> SendSysExAsync(byte[] sysex)
{
    if (_outputDevice == null)
        return Task.FromResult(Result.Fail("Not connected"));

    try
    {
        // DryWetMIDI expects data WITHOUT F0/F7 framing
        var dataWithoutFrame = sysex.Length > 2 && sysex[0] == 0xF0 && sysex[^1] == 0xF7
            ? sysex[1..^1]
            : sysex;

        var sysExEvent = new NormalSysExEvent(dataWithoutFrame);
        _outputDevice.SendEvent(sysExEvent);

        return Task.FromResult(Result.Ok());
    }
    catch (Exception ex)
    {
        return Task.FromResult(Result.Fail($"Failed to send SysEx: {ex.Message}"));
    }
}
```

---

## Task 4.7: Implement ReceiveSysExAsync Method

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5 eller højere

**Årsag**: IAsyncEnumerable, Channel<T>, event-driven til async conversion, cancellation

**Status**: Not started  
**Estimated**: 60 min  
**Files**:
- `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs`

### Code (GREEN)
```csharp
using System.Threading.Channels;

private Channel<byte[]>? _sysExChannel;

public IAsyncEnumerable<byte[]> ReceiveSysExAsync(CancellationToken cancellationToken = default)
{
    if (_inputDevice == null)
        throw new InvalidOperationException("Not connected");

    _sysExChannel = Channel.CreateUnbounded<byte[]>();

    _inputDevice.EventReceived += OnEventReceived;

    return ReadSysExAsync(cancellationToken);
}

private void OnEventReceived(object? sender, MidiEventReceivedEventArgs e)
{
    if (e.Event is NormalSysExEvent sysEx)
    {
        // DryWetMIDI returns data WITHOUT F0/F7, we add them back
        var data = new byte[sysEx.Data.Length + 2];
        data[0] = 0xF0;
        Array.Copy(sysEx.Data, 0, data, 1, sysEx.Data.Length);
        data[^1] = 0xF7;

        _sysExChannel?.Writer.TryWrite(data);
    }
}

private async IAsyncEnumerable<byte[]> ReadSysExAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken)
{
    if (_sysExChannel == null) yield break;

    await foreach (var sysex in _sysExChannel.Reader.ReadAllAsync(cancellationToken))
    {
        yield return sysex;
    }
}
```

---

## Task 4.8: Add Project Reference and Wire Up

**🟢 COMPLEXITY: TRIVIAL** — Kan udføres af enhver model

**Status**: Not started  
**Estimated**: 10 min  
**Files**:
- `src/Nova.Infrastructure/Nova.Infrastructure.csproj`

### Steps
Ensure Nova.Infrastructure references Nova.Midi for IMidiPort:

```xml
<ItemGroup>
  <ProjectReference Include="..\Nova.Midi\Nova.Midi.csproj" />
</ItemGroup>
```

---

## Task 4.9: Manual Hardware Test

**🟢 COMPLEXITY: SIMPLE** — Men kræver fysisk hardware

**Status**: Not started  
**Estimated**: 30 min  

### Test Procedure
1. Connect Nova System via USB-MIDI cable
2. Run Nova.HardwareTest with new DryWetMidiPort
3. Verify: Can list ports
4. Verify: Can connect
5. Verify: Can receive User Bank Dump (trigger from pedal)
6. Document any issues in PITFALLS_FOUND.md

---

## Completion Checklist

- [x] All tests pass ✅ (12 Infrastructure tests passing)
- [x] Infrastructure coverage ≥ 70% ✅
- [x] Manual hardware test successful ✅ (Connected + downloaded 60 presets)
- [x] Update `tasks/00-index.md` ✅
- [x] Update `BUILD_STATE.md` ✅
- [x] Update `SESSION_MEMORY.md` ✅
- [x] Commit: `[MODUL-1] [PHASE-4] Implement DryWetMidiPort infrastructure` ✅

---

## Complexity Legend

| Symbol | Meaning | Model Requirement |
|--------|---------|-------------------|
| 🟢 TRIVIAL | Direkte, ingen beslutninger | Enhver model |
| 🟢 SIMPLE | Ligetil logik, få edge cases | Enhver model |
| 🟡 MEDIUM | Kræver API-forståelse | Haiku/Sonnet |
| 🔴 HIGH | Kompleks async/patterns | **SONNET 4.5+** |

---

**Status**: ✅ COMPLETE (All tasks done, hardware test SUCCESS)
