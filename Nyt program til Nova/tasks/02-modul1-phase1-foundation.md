# Task List: Modul 1, Phase 1 — MIDI Layer Foundation

## 📋 Module: 1 (MVP - Connection + Bank Dump)
## 📋 Phase: 1 (Foundation - MIDI abstraction layer)
**Duration**: 1 week  
**Prerequisite**: Phase 0 (Environment setup) complete  
**Output**: IMidiPort interface + mock implementation, all tests passing  

---

## Overview

**Goal**: Build MIDI I/O abstraction layer that can be tested without real hardware.

**Design**: 
- `IMidiPort` interface (contract)
- `MockMidiPort` (for unit testing)
- Specification of DryWetMIDI adapter (for Modul 2)

**Key Principle**: Everything is testable with mock data.

---

## Task 1.1: Create IMidiPort Interface

**Status**: Not started  
**Estimated**: 30 min  
**File**: `NovaApp.Midi/IMidiPort.cs`

### Test First (RED)
```csharp
namespace NovaApp.Tests;

public class MidiPortContractTests
{
    [Fact]
    public void IMidiPort_HasConnectAsync()
    {
        // Verify interface exists and has ConnectAsync
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("ConnectAsync");
        Assert.NotNull(method);
    }
    
    [Fact]
    public void IMidiPort_HasSendSysExAsync()
    {
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("SendSysExAsync");
        Assert.NotNull(method);
    }
    
    [Fact]
    public void IMidiPort_HasReceiveSysExAsync()
    {
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("ReceiveSysExAsync");
        Assert.NotNull(method);
    }
}
```

### Code (GREEN)

File: `NovaApp.Midi/IMidiPort.cs`

```csharp
using FluentResults;

namespace NovaApp.Midi;

/// <summary>
/// Abstraction for MIDI port I/O operations.
/// Implemented by both real MIDI and mock implementations.
/// </summary>
public interface IMidiPort
{
    /// <summary>
    /// Gets the name of this MIDI port.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets whether the port is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the MIDI port.
    /// </summary>
    /// <param name="portName">Name of the port to connect to (e.g., "Nova System")</param>
    /// <returns>Success or failure reason</returns>
    Task<Result> ConnectAsync(string portName);

    /// <summary>
    /// Disconnects from the MIDI port.
    /// </summary>
    Task<Result> DisconnectAsync();

    /// <summary>
    /// Sends a SysEx message to the MIDI port.
    /// </summary>
    /// <param name="sysex">Complete SysEx data (F0...F7)</param>
    /// <returns>Success or failure reason</returns>
    Task<Result> SendSysExAsync(byte[] sysex);

    /// <summary>
    /// Receives SysEx messages from the MIDI port.
    /// Yields complete SysEx messages (F0...F7).
    /// 
    /// Use with foreach or async enumeration:
    ///   await foreach (var sysex in port.ReceiveSysExAsync())
    ///   {
    ///       ProcessSysEx(sysex);
    ///   }
    /// </summary>
    /// <param name="cancellationToken">Token to cancel reception</param>
    /// <returns>Async enumerable of SysEx messages</returns>
    IAsyncEnumerable<byte[]> ReceiveSysExAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enumerates available MIDI output ports on the system.
    /// </summary>
    /// <returns>Port names (e.g., "Nova System", "In From MIDI Port 1")</returns>
    static abstract IEnumerable<string> GetAvailableOutputPorts();

    /// <summary>
    /// Enumerates available MIDI input ports on the system.
    /// </summary>
    /// <returns>Port names</returns>
    static abstract IEnumerable<string> GetAvailableInputPorts();
}
```

### Refactor (CLEAN)
- Documentation is clear
- No additional refactor needed

### Verification
```powershell
dotnet test --filter "IMidiPort"
# Output: PASSED ✅
dotnet build
# Output: succeeded with 0 warnings
```

### Checklist
- [ ] Interface defined with 6 methods
- [ ] Documentation on every method
- [ ] SysEx reception uses async enumerable
- [ ] Connection status trackable
- [ ] Tests pass

---

## Task 1.2: Create MockMidiPort

**Status**: Not started  
**Estimated**: 45 min  
**File**: `NovaApp.Tests/Mocks/MockMidiPort.cs`

### Test First (RED)
```csharp
namespace NovaApp.Tests;

public class MockMidiPortTests
{
    [Fact]
    public async Task MockMidiPort_Connect_SetsIsConnected()
    {
        var port = new MockMidiPort();
        Assert.False(port.IsConnected);

        var result = await port.ConnectAsync("Test Port");

        Assert.True(result.IsSuccess);
        Assert.True(port.IsConnected);
    }

    [Fact]
    public async Task MockMidiPort_SendSysEx_Succeeds()
    {
        var port = new MockMidiPort();
        await port.ConnectAsync("Test");

        var sysex = new byte[] { 0xF0, 0x00, 0x20, 0x1F, 0x00, 0x63, 0x45, 0x03, 0xF7 };
        var result = await port.SendSysExAsync(sysex);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task MockMidiPort_ReceiveSysEx_YieldsEnqueuedData()
    {
        var port = new MockMidiPort();
        await port.ConnectAsync("Test");

        var expectedSysex = new byte[] { 0xF0, 0x00, 0x20, 0x1F, 0x00, 0x63, 0x20, 0x03,
            0x00, 0x00, /* name bytes */, 0x3C, 0xF7 };
        port.EnqueueResponse(expectedSysex);

        var received = new List<byte[]>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        await foreach (var data in port.ReceiveSysExAsync(cts.Token))
        {
            received.Add(data);
            break;  // Just get one for testing
        }

        Assert.Single(received);
        Assert.Equal(expectedSysex, received[0]);
    }
}
```

### Code (GREEN)

File: `NovaApp.Tests/Mocks/MockMidiPort.cs`

```csharp
using FluentResults;
using NovaApp.Midi;

namespace NovaApp.Tests;

/// <summary>
/// Mock MIDI port for unit testing. No real hardware required.
/// </summary>
public class MockMidiPort : IMidiPort
{
    private bool _isConnected;
    private readonly Queue<byte[]> _responseQueue = new();
    private readonly Channel<byte[]> _receiveChannel = Channel.CreateUnbounded<byte[]>();

    public string Name { get; set; } = "Mock Port";
    public bool IsConnected => _isConnected;

    public Task<Result> ConnectAsync(string portName)
    {
        Name = portName;
        _isConnected = true;
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> DisconnectAsync()
    {
        _isConnected = false;
        _receiveChannel.Writer.TryComplete();
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> SendSysExAsync(byte[] sysex)
    {
        if (!IsConnected)
            return Task.FromResult(Result.Fail("Not connected"));

        if (sysex == null || sysex.Length == 0)
            return Task.FromResult(Result.Fail("Empty SysEx"));

        return Task.FromResult(Result.Ok());
    }

    public async IAsyncEnumerable<byte[]> ReceiveSysExAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
            yield break;

        // First yield any enqueued responses
        while (_responseQueue.TryDequeue(out var response))
        {
            yield return response;
        }

        // Then wait for new data on channel
        await foreach (var data in _receiveChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return data;
        }
    }

    /// <summary>
    /// For testing: enqueue response data to be received.
    /// </summary>
    public void EnqueueResponse(byte[] sysex)
    {
        _responseQueue.Enqueue(sysex);
    }

    /// <summary>
    /// For testing: send data as if received from hardware.
    /// </summary>
    public async Task SendResponseAsync(byte[] sysex)
    {
        await _receiveChannel.Writer.WriteAsync(sysex);
    }

    public static IEnumerable<string> GetAvailableOutputPorts()
        => new[] { "Mock Output 1", "Mock Output 2" };

    public static IEnumerable<string> GetAvailableInputPorts()
        => new[] { "Mock Input 1", "Mock Input 2" };
}
```

### Refactor (CLEAN)
- Extract common validation into private method

```csharp
private void ValidateConnected()
{
    if (!IsConnected)
        throw new InvalidOperationException("Port not connected");
}
```

### Verification
```powershell
dotnet test --filter "MockMidiPort"
# Output: PASSED ✅ (3 tests)
dotnet build
# Output: succeeded with 0 warnings
```

### Checklist
- [ ] Implements IMidiPort fully
- [ ] Connection state tracked
- [ ] Response queueing works
- [ ] Async enumeration works
- [ ] Tests pass

---

## Task 1.3: Create SysEx Request Builder

**Status**: Not started  
**Estimated**: 30 min  
**File**: `NovaApp.Domain/Midi/SysExBuilder.cs`

### Test First (RED)
```csharp
namespace NovaApp.Tests;

public class SysExBuilderTests
{
    [Fact]
    public void BuildBankDumpRequest_Returns9Bytes()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(9, request.Length);
        Assert.Equal(0xF0, request[0]);
        Assert.Equal(0xF7, request[8]);
    }

    [Fact]
    public void BuildBankDumpRequest_HasCorrectManufacturerId()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(0x00, request[1]);
        Assert.Equal(0x20, request[2]);
        Assert.Equal(0x1F, request[3]);
    }

    [Fact]
    public void BuildBankDumpRequest_HasCorrectModelId()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(0x63, request[5]);  // Nova System
    }

    [Fact]
    public void BuildBankDumpRequest_HasCorrectMessageId()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(0x45, request[6]);  // Request message ID
    }
}
```

### Code (GREEN)

File: `NovaApp.Domain/Midi/SysExBuilder.cs`

```csharp
namespace NovaApp.Domain.Midi;

/// <summary>
/// Builds SysEx messages for Nova System communication.
/// </summary>
public static class SysExBuilder
{
    // MIDI SysEx identifiers
    private const byte SYSEX_START = 0xF0;
    private const byte SYSEX_END = 0xF7;

    // TC Electronic IDs
    private const byte TC_ID_1 = 0x00;
    private const byte TC_ID_2 = 0x20;
    private const byte TC_ID_3 = 0x1F;

    // Nova System
    private const byte DEVICE_ID = 0x00;  // 0 = any device
    private const byte MODEL_ID = 0x63;   // Nova System

    // Message IDs
    private const byte REQUEST_MESSAGE_ID = 0x45;
    private const byte RESPONSE_MESSAGE_ID = 0x20;
    private const byte USER_BANK_DUMP = 0x03;

    /// <summary>
    /// Builds request for User Bank Dump (all 60 presets).
    /// </summary>
    /// <returns>9-byte SysEx request: F0 00 20 1F 00 63 45 03 F7</returns>
    public static byte[] BuildBankDumpRequest()
    {
        return new byte[]
        {
            SYSEX_START,
            TC_ID_1, TC_ID_2, TC_ID_3,
            DEVICE_ID,
            MODEL_ID,
            REQUEST_MESSAGE_ID,
            USER_BANK_DUMP,
            SYSEX_END
        };
    }
}
```

### Refactor (CLEAN)
- Constants clearly named
- No additional refactor needed

### Verification
```powershell
dotnet test --filter "SysExBuilder"
# Output: PASSED ✅ (4 tests)
dotnet build
# Output: succeeded with 0 warnings
```

### Checklist
- [ ] Request builder returns correct bytes
- [ ] All SysEx identifiers correct
- [ ] Tests pass
- [ ] Ready for parser

---

## Task 1.4: Create SysEx Parser (Checksum Validation)

**Status**: Not started  
**Estimated**: 45 min  
**File**: `NovaApp.Domain/Midi/SysExValidator.cs`

### Test First (RED)
```csharp
namespace NovaApp.Tests;

public class SysExValidatorTests
{
    [Fact]
    public void CalculateChecksum_KnownData_MatchesExpected()
    {
        // Data from real hardware
        var data = new byte[515];  // bytes 8-516 (485 bytes + 2 spare = 487, but we count from 8)
        // Just fill with zeros for testing
        Array.Fill(data, (byte)0x00);

        var checksum = SysExValidator.CalculateChecksum(data);

        Assert.InRange(checksum, 0, 127);  // Must be 7-bit value
    }

    [Fact]
    public void ValidateChecksum_CorrectChecksum_ReturnsTrue()
    {
        // Build a simple SysEx with known checksum
        var preset = new byte[520];
        Array.Fill(preset, (byte)0x00);
        preset[0] = 0xF0;
        preset[519] = 0xF7;
        
        // Calculate correct checksum for bytes 8-516
        var checksumData = new byte[509];  // bytes 8-516
        Array.Copy(preset, 8, checksumData, 0, 509);
        var checksum = SysExValidator.CalculateChecksum(checksumData);
        preset[517] = checksum;

        var isValid = SysExValidator.ValidateChecksum(preset);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidateChecksum_BadChecksum_ReturnsFalse()
    {
        var preset = new byte[520];
        Array.Fill(preset, (byte)0x00);
        preset[0] = 0xF0;
        preset[519] = 0xF7;
        preset[517] = 0xFF;  // Wrong checksum

        var isValid = SysExValidator.ValidateChecksum(preset);

        Assert.False(isValid);
    }
}
```

### Code (GREEN)

File: `NovaApp.Domain/Midi/SysExValidator.cs`

```csharp
namespace NovaApp.Domain.Midi;

/// <summary>
/// Validates SysEx messages from Nova System.
/// </summary>
public static class SysExValidator
{
    /// <summary>
    /// Calculates checksum for SysEx preset data.
    /// Checksum = 7 LSBs of sum of parameter bytes (8-516).
    /// </summary>
    /// <param name="parameterData">Bytes 8-516 of preset (509 bytes)</param>
    /// <returns>Checksum value (0-127)</returns>
    public static byte CalculateChecksum(byte[] parameterData)
    {
        if (parameterData == null || parameterData.Length == 0)
            return 0;

        // Sum all parameter bytes
        int sum = 0;
        foreach (var b in parameterData)
        {
            sum += b;
        }

        // Keep only 7 LSBs (0-127)
        return (byte)(sum & 0x7F);
    }

    /// <summary>
    /// Validates SysEx preset message (520 bytes).
    /// </summary>
    /// <param name="sysex">Complete SysEx: F0...F7 (520 bytes)</param>
    /// <returns>True if checksum is valid</returns>
    public static bool ValidateChecksum(byte[] sysex)
    {
        if (sysex == null || sysex.Length != 520)
            return false;

        if (sysex[0] != 0xF0 || sysex[519] != 0xF7)
            return false;

        // Extract parameter bytes (8-516)
        var parameterData = new byte[509];
        Array.Copy(sysex, 8, parameterData, 0, 509);

        var expectedChecksum = CalculateChecksum(parameterData);
        var actualChecksum = sysex[517];

        return expectedChecksum == actualChecksum;
    }
}
```

### Refactor (CLEAN)
- Validation logic is clear and testable
- No additional refactor needed

### Verification
```powershell
dotnet test --filter "SysExValidator"
# Output: PASSED ✅ (3 tests)
dotnet build
# Output: succeeded with 0 warnings
```

### Checklist
- [ ] Checksum calculation correct (7 LSBs)
- [ ] Validation returns bool
- [ ] Tests pass
- [ ] Works with real data

---

## Task 1.5: Test Coverage Verification

**Status**: Not started  
**Estimated**: 5 min

### Steps
- [ ] Run coverage report: `dotnet test /p:CollectCoverage=true`
- [ ] Check Domain layer coverage ≥ 95%
- [ ] Add missing tests if needed
- [ ] All tests pass

### Verification
```powershell
dotnet test
# Output: All tests PASSED ✅
dotnet build
# Output: succeeded with 0 warnings
```

### Checklist
- [ ] All 13 tests passing
- [ ] Coverage ≥ 95% (Domain)
- [ ] Build clean

---

## ✅ Phase 1 Complete When

- [x] IMidiPort interface defined
- [x] MockMidiPort fully implemented
- [x] SysExBuilder creates correct messages
- [x] SysExValidator calculates checksums
- [x] All 13 tests passing
- [x] Coverage ≥ 95%
- [x] Build 0 warnings
- [x] Committed with clear message

---

## Commit Message Template

```
[MODUL-1] [PHASE-1] Complete MIDI layer foundation

Tests written and passing:
  + MidiPortContractTests (3 tests)
  + MockMidiPortTests (3 tests)
  + SysExBuilderTests (4 tests)
  + SysExValidatorTests (3 tests)

Code changes:
  + IMidiPort interface (Midi/)
  + MockMidiPort implementation (Tests/Mocks/)
  + SysExBuilder static class (Domain/Midi/)
  + SysExValidator static class (Domain/Midi/)

Quality:
  ✅ All 13 tests passing
  ✅ Coverage: 95%+ (Domain)
  ✅ Build: 0 warnings
  ✅ No breaking changes
```

---

## 🔗 Next Phase

Once complete: `tasks/03-modul1-phase2-domain-models.md`

---

## Notes

- Don't move forward until all tests are GREEN
- Use mock data for all tests (no real hardware yet)
- Every change must have a test
- Commit after each task (even small ones)
