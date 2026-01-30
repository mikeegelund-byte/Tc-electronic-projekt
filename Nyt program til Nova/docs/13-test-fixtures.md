# Test Fixtures — Real SysEx Data

## Overview
This document defines all test fixtures (real SysEx data) needed for Modul 1 testing. These are used to:
1. Parse real hardware responses
2. Validate checksum calculations
3. Mock MIDI I/O in unit tests
4. Simulate error scenarios

---

## Test Data Organization

```
Nova.Tests/
├── Fixtures/
│   ├── BankDumps/
│   │   ├── nova-bank-all-defaults.syx        (60 empty presets)
│   │   ├── nova-bank-with-artist-presets.syx (artist bank from hardware)
│   │   └── nova-bank-corrupted-preset5.syx   (for error testing)
│   ├── PresetResponses/
│   │   ├── preset-001-clean.bin               (single 520-byte preset)
│   │   └── preset-001-bad-checksum.bin
│   └── README.md                              (hex dumps for documentation)
```

---

## Fixture 1: User Bank Dump Request (Outgoing)

### What is it
The MIDI message sent BY the app TO the Nova System requesting user bank dump.

### Hex dump
```
F0 00 20 1F 00 63 45 03 F7
```

### Breakdown
```
F0        = SysEx start
00 20 1F  = TC Electronic manufacturer ID
00        = Device ID (0 = any device)
63        = Nova System model ID
45        = Request message ID
03        = Request user bank dump
F7        = SysEx end
```

### Usage in tests
```csharp
[Fact]
public void BuildBankDumpRequest_Returns9Bytes()
{
    var expected = new byte[] { 0xF0, 0x00, 0x20, 0x1F, 0x00, 0x63, 0x45, 0x03, 0xF7 };
    var request = SysExBuilder.BuildBankDumpRequest();
    
    Assert.Equal(expected, request);
}
```

### File
```
Nova.Tests/Fixtures/BankDumpRequest.bin
(actual file: 9 bytes)
```

---

## Fixture 2: Single Preset (520 bytes)

### What is it
A single preset response from Nova System. When user bank dump is requested, 60 of these come back.

### Structure
```
Byte offset   | Purpose
──────────────┼────────────────────────────────
0             | 0xF0 (SysEx start)
1-2           | 0x00 0x20
3             | 0x1F
4             | 0x00
5             | 0x63 (Nova System)
6             | 0x20 (Bank dump response ID)
7             | Preset number (0-59 = user presets, or 0-89 for program banks)
8             | Bank (0=user, 1=A, 2=B, 3=C, 4=D)
9-32          | 24-byte preset name (ASCII, space-padded)
33-516        | 484 bytes of parameters (nibble-encoded)
              | - Drive block: 100+ bytes
              | - Reverb block: 100+ bytes
              | - etc.
517           | Checksum (7 LSBs of sum of bytes 8-516)
518-519       | Spare / padding
520           | 0xF7 (SysEx end)
```

**Total: 520 bytes**

### Example hex dump (first 50 bytes + structure)
```
F0 00 20 1F 00 63 20 03   00 00 41 6E 64 72 65 61   |....c. ...Andrea|
6E 20 50 72 65 73 65 74   00 00 00 00 00 00 00 00   | Preset ........|
06 05 00 02 06 12 03 00   ...
```

### Breakdown
- `F0 00 20 1F 00 63` = Header
- `20` = User bank dump response
- `03` = Preset number 3
- `00` = User bank
- `41 6E 64 72 65 61 20 50 72 65 73 65 74 00 ...` = "Andrea Preset" (name)
- `06 05 00 02 06 12 03 00 ...` = Parameter data (nibble-encoded)
- Last byte before F7 = Checksum

### Usage in tests
```csharp
[Fact]
public void ParsePreset_ValidSysEx_ReturnsCorrectName()
{
    var fixture = File.ReadAllBytes("Fixtures/PresetResponses/preset-001-clean.bin");
    var preset = Preset.FromSysEx(fixture);
    
    Assert.True(preset.IsSuccess);
    Assert.Equal("Andrea Preset", preset.Value.Name);
}
```

### File
```
Nova.Tests/Fixtures/PresetResponses/preset-001-clean.bin
(actual file: 520 bytes)

Hex representation (for documentation):
F0 00 20 1F 00 63 20 03 00 00 41 6E 64 72 65 61
20 50 72 65 73 65 74 00 00 00 00 00 00 00 00 00
[... 470+ bytes of parameter data ...]
3C F7
```

---

## Fixture 3: User Bank Dump Response (~31KB)

### What is it
Complete response when user bank dump is requested: 60 presets × 520 bytes = 31,200 bytes.

### Structure
```
Header:
F0 00 20 1F 00 63 20 03

Presets 0-59:
[Preset 0: 520 bytes]
[Preset 1: 520 bytes]
...
[Preset 59: 520 bytes]
```

**Total: F0 + header + (60 × 520) + F7 = ~31,206 bytes**

### Checksum calculation
For each preset, checksum = sum of bytes 8-516 MOD 128 (7 LSBs)

### Usage in tests
```csharp
[Fact]
public void ParseBankDump_60Presets_AllValid()
{
    var fixture = File.ReadAllBytes("Fixtures/BankDumps/nova-bank-all-defaults.syx");
    var bank = UserBankDump.FromSysEx(fixture);
    
    Assert.True(bank.IsSuccess);
    Assert.Equal(60, bank.Value.Presets.Length);
    
    for (int i = 0; i < 60; i++)
    {
        Assert.True(bank.Value.Presets[i].IsValid);
    }
}
```

### Files
```
Nova.Tests/Fixtures/BankDumps/nova-bank-all-defaults.syx (~31 KB)
- All 60 presets with default empty settings

Nova.Tests/Fixtures/BankDumps/nova-bank-with-artist-presets.syx (~31 KB)
- Artist presets from Nova-System-LTD_Artists-Presets-for-User-Bank.syx
- Real data from hardware
```

---

## Fixture 4: Corrupted Preset (Checksum Mismatch)

### What is it
A preset with intentionally wrong checksum, for testing error detection.

### How to create
```csharp
var validPreset = new byte[520];
Array.Copy(validFixture, validPreset, 520);
validPreset[517] = (byte)((validPreset[517] + 1) % 128); // Flip checksum
File.WriteAllBytes("preset-corrupted.bin", validPreset);
```

### Usage in tests
```csharp
[Fact]
public void ParsePreset_BadChecksum_ReturnsFail()
{
    var fixture = File.ReadAllBytes("Fixtures/PresetResponses/preset-001-bad-checksum.bin");
    var preset = Preset.FromSysEx(fixture);
    
    Assert.False(preset.IsSuccess);
    Assert.Contains("checksum", preset.Error, StringComparison.OrdinalIgnoreCase);
}
```

### File
```
Nova.Tests/Fixtures/PresetResponses/preset-001-bad-checksum.bin (520 bytes)
```

---

## Fixture 5: Incomplete SysEx (for timeout testing)

### What is it
A partial preset (e.g., 300 bytes) without final F7, to simulate timeout or disconnection.

### Usage in tests
```csharp
[Fact]
public async Task ReceiveSysExAsync_TimeoutAfter30s_ReturnsFailure()
{
    var incompleteSysEx = new byte[300]; // Incomplete, no F7
    var mockPort = CreateMockPort();
    mockPort.SetupSysExResponse(incompleteSysEx, delayMs: 35000);
    
    var result = await mockPort.ReceiveSysExAsync();
    
    Assert.False(result.IsSuccess);
    Assert.Contains("timeout", result.Error, StringComparison.OrdinalIgnoreCase);
}
```

### File
```
Nova.Tests/Fixtures/PresetResponses/preset-001-incomplete.bin (300 bytes)
(No F7 at end)
```

---

## Fixture 6: Chunked Response (for buffering testing)

### What is it
A bank dump split across multiple USB packets (simulating real hardware behavior).

### Scenario
```
Packet 1: [Header + Preset 0: 520 bytes]
Packet 2: [Preset 1: 520 bytes]
Packet 3: [Preset 2: partial, 100 bytes] + [Rest of Preset 2, 420 bytes + Preset 3, 520 bytes]
...
Packet 60: [Last presets + F7]
```

### Usage in tests
```csharp
[Theory]
[InlineData(1)]     // One packet
[InlineData(10)]    // 10 packets
[InlineData(60)]    // One per preset
public async Task ReceiveSysExAsync_ChunkedData_BuffersCorrectly(int numChunks)
{
    var fullBank = File.ReadAllBytes("Fixtures/BankDumps/nova-bank-all-defaults.syx");
    var chunks = ChunkData(fullBank, numChunks);
    
    var mockPort = CreateMockPort();
    foreach (var chunk in chunks)
    {
        mockPort.EnqueueChunk(chunk);
    }
    
    var received = new List<byte>();
    await foreach (var data in mockPort.ReceiveChunksAsync())
    {
        received.AddRange(data);
    }
    
    Assert.Equal(fullBank, received.ToArray());
}
```

### Files
```
Nova.Tests/Fixtures/PresetResponses/bank-dump-chunked-10.bin
(Bank dump split into 10 chunks, stored as: [len1][chunk1][len2][chunk2]...)

Nova.Tests/Fixtures/PresetResponses/bank-dump-chunked-60.bin
(Bank dump split into 60 chunks)
```

---

## Fixture 7: Real Hardware Data (Artist Presets)

### What is it
The actual `.syx` file from Nova-System-LTD_Artists-Presets-for-User-Bank.syx

### Location
```
Tc originalt materiale/Nova-System-LTD_Artists-Presets-for-User-Bank.syx
```

### How to use in tests
```csharp
[Fact]
public void ParseBankDump_RealArtistPresets_AllValid()
{
    // Copy to fixtures first
    var fixture = File.ReadAllBytes("Fixtures/BankDumps/nova-bank-with-artist-presets.syx");
    var bank = UserBankDump.FromSysEx(fixture);
    
    Assert.True(bank.IsSuccess);
    Assert.Equal(60, bank.Value.Presets.Length);
    
    // Spot-check a few presets
    Assert.False(string.IsNullOrWhiteSpace(bank.Value.Presets[0].Name));
}
```

---

## Fixture Preparation Script

Create a PowerShell script to prepare all fixtures:

```powershell
# prepare-fixtures.ps1

$fixtureDir = "Nova.Tests/Fixtures"

# Create directories
@("BankDumps", "PresetResponses") | ForEach-Object {
    New-Item -Path "$fixtureDir/$_" -ItemType Directory -Force | Out-Null
}

# Copy real data from materials
Copy-Item `
    "d:\Tc electronic projekt\Tc originalt materiale\Nova-System-LTD_Artists-Presets-for-User-Bank.syx" `
    "$fixtureDir/BankDumps/nova-bank-with-artist-presets.syx"

# Generate corrupted fixtures (run via C# tool)
Write-Host "Run GenerateCorruptedFixtures tool to create error-case fixtures"

Write-Host "Fixtures prepared at: $fixtureDir"
```

---

## Test Data Validation

### How to verify fixtures are correct
1. Load with parsing code
2. Validate checksums
3. Spot-check preset names and parameters
4. Compare roundtrip (parse → serialize → parse) matches

```csharp
[Fact]
public void AllFixtures_ParseAndRoundtrip_MatchOriginal()
{
    var fixtureDir = "Fixtures/BankDumps";
    foreach (var file in Directory.EnumerateFiles(fixtureDir, "*.syx"))
    {
        var original = File.ReadAllBytes(file);
        var parsed = UserBankDump.FromSysEx(original);
        
        Assert.True(parsed.IsSuccess, $"Failed to parse {file}");
        
        var serialized = parsed.Value.ToSysEx();
        Assert.Equal(original, serialized);
    }
}
```

---

## Summary of Fixtures

| Fixture | File | Size | Purpose |
|---------|------|------|---------|
| Bank dump request | BankDumpRequest.bin | 9 bytes | Outgoing MIDI message |
| Clean preset | preset-001-clean.bin | 520 bytes | Valid parsing test |
| Bad checksum | preset-001-bad-checksum.bin | 520 bytes | Error detection |
| Incomplete preset | preset-001-incomplete.bin | 300 bytes | Timeout handling |
| Default bank | nova-bank-all-defaults.syx | 31 KB | Full bank parsing |
| Artist bank | nova-bank-with-artist-presets.syx | 31 KB | Real hardware data |
| Chunked data | bank-dump-chunked-*.bin | variable | Buffering tests |

**Total fixture size: ~65 KB**

---

## Fixture Setup in Test Class

```csharp
public class MidiParsingTests : IAsyncLifetime
{
    private static readonly string FixturePath = Path.Combine(
        AppContext.BaseDirectory, 
        "Fixtures"
    );
    
    private byte[] _cleanPreset;
    private byte[] _corruptedPreset;
    private byte[] _defaultBank;
    private byte[] _artistBank;
    
    public async Task InitializeAsync()
    {
        _cleanPreset = File.ReadAllBytes(
            Path.Combine(FixturePath, "PresetResponses/preset-001-clean.bin")
        );
        _corruptedPreset = File.ReadAllBytes(
            Path.Combine(FixturePath, "PresetResponses/preset-001-bad-checksum.bin")
        );
        _defaultBank = File.ReadAllBytes(
            Path.Combine(FixturePath, "BankDumps/nova-bank-all-defaults.syx")
        );
        _artistBank = File.ReadAllBytes(
            Path.Combine(FixturePath, "BankDumps/nova-bank-with-artist-presets.syx")
        );
        
        await Task.CompletedTask;
    }
    
    public Task DisposeAsync() => Task.CompletedTask;
    
    [Fact]
    public void ParseCleanPreset_Succeeds() => ...
}
```

**Status: Ready for test development**
