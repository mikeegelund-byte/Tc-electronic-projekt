# Modul 1 Teknisk detalje — Connection + Bank Dump roundtrip

## Overview
Modul 1 er MVP. Efter denne modul, ved vi at MIDI kommunikation virker stabilt, og vi har et arbejdende fundament for resten.

---

## Use case flow (detaljeret)

### Flow 1: User clicks "Connect"
```
User: Click "Connect" button
  ↓
MainViewModel.ConnectCommand.Execute()
  ↓
ConnectUseCase.ExecuteAsync(portName)
  ↓
IMidiPort.ConnectAsync(portName)
  ↓
MidiPortImpl calls DryWetMIDI:
  - DryWetMidiAdapter.OpenInputAsync(portName)
  - DryWetMidiAdapter.OpenOutputAsync(portName)
  ↓
If success:
  - Set IsConnected = true
  - UI shows "Connected" status
  ↓
If failure:
  - Return Result.Failure(reason)
  - UI shows red error message
  - User can retry
```

### Flow 2: User clicks "Download Bank"
```
User: Click "Download Bank" button
  ↓
DownloadBankUseCase.ExecuteAsync()
  ↓
Build SysEx request:
  F0 00 20 1F 00 63 45 03 F7
  (Request User Bank Dump, Message ID 45, Type 03)
  ↓
Send via IMidiPort.SendSysExAsync(sysex)
  ↓
IMidiPort.ReceiveSysExAsync() starts listening
  ↓
Hardware sends 60 presets (~31KB in chunks)
  ↓
IMidiPort buffers chunks:
  - Accumulate bytes until F7 received
  - Returns complete SysEx (520 bytes × 60)
  ↓
Domain layer: Parse UserBankDump
  - For each of 60 presets:
    - Extract name (bytes 10-33)
    - Extract parameters (bytes 34-517)
    - Validate checksum
  ↓
If all valid:
  - Return Result.Success(bank)
  ↓
If any invalid:
  - Return Result.Failure("Corrupt preset #5")
  ↓
Update ViewModel:
  - Set PresetList = [ "00-1: ...", "00-2: ...", ... ]
  - Status = "Downloaded 60 presets"
  ↓
User sees list of preset names
```

### Flow 3: User clicks "Upload Bank" (send back to Nova)
```
User: Click "Upload Bank" button
  ↓
UploadBankUseCase.ExecuteAsync(bankDump)
  ↓
Build complete SysEx:
  F0 00 20 1F 00 63 20 03 [Preset1..Preset60] [Checksum] F7
  ↓
IMidiPort.SendSysExAsync(completeBank)
  ↓
Hardware receives and writes to flash memory
  ↓
User sees: "Bank uploaded successfully"
```

---

## Data structures (C#)

### Domain models
```csharp
// Single preset
public class Preset
{
    public required int Number { get; init; }     // 1-90
    public required string Name { get; init; }    // 24 chars
    public required byte[] RawSysEx { get; init; } // 520 bytes
    public required byte Checksum { get; init; }
    
    public static Result<Preset> FromSysEx(byte[] data);
    public byte[] ToSysEx();
    public bool IsValid => ValidateChecksum();
}

// 60 presets
public class UserBankDump
{
    public required Preset[] Presets { get; init; } // [0..59]
    
    public static Result<UserBankDump> FromSysEx(byte[] data);
    public byte[] ToSysEx();
}
```

### Application layer
```csharp
public class DownloadBankUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly IUserBankParser _parser;
    
    public async Task<Result<UserBankDump>> ExecuteAsync()
    {
        // 1. Send request
        var requestSysEx = new byte[] { 0xF0, 0x00, 0x20, 0x1F, 0x00, 0x63, 0x45, 0x03, 0xF7 };
        var sendResult = await _midiPort.SendSysExAsync(requestSysEx);
        if (!sendResult.IsSuccess) return Result<UserBankDump>.Failure(sendResult.Error);
        
        // 2. Receive bank (may come in chunks)
        var receivedData = new List<byte[]>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        try
        {
            await foreach (var sysex in _midiPort.ReceiveSysExAsync())
            {
                receivedData.Add(sysex);
                if (receivedData.Sum(d => d.Length) >= 31200) break; // 60 × 520
            }
        }
        catch (OperationCanceledException)
        {
            return Result<UserBankDump>.Failure("Timeout waiting for bank dump");
        }
        
        // 3. Combine chunks into single buffer
        var buffer = CombineChunks(receivedData);
        
        // 4. Parse
        return _parser.Parse(buffer);
    }
}
```

### UI layer (ViewModel)
```csharp
public class MainViewModel : INotifyPropertyChanged
{
    private readonly ConnectUseCase _connectUseCase;
    private readonly DownloadBankUseCase _downloadUseCase;
    
    private bool _isConnected;
    private string _statusMessage = "Disconnected";
    private ObservableCollection<PresetItemViewModel> _presets = new();
    
    public bool IsConnected
    {
        get => _isConnected;
        set => SetProperty(ref _isConnected, value);
    }
    
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }
    
    public ObservableCollection<PresetItemViewModel> Presets => _presets;
    
    public ICommand ConnectCommand { get; }
    public ICommand DownloadBankCommand { get; }
    
    public MainViewModel(ConnectUseCase connectUseCase, DownloadBankUseCase downloadUseCase)
    {
        _connectUseCase = connectUseCase;
        _downloadUseCase = downloadUseCase;
        
        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
        DownloadBankCommand = new AsyncRelayCommand(DownloadBankAsync, () => IsConnected);
    }
    
    private async Task ConnectAsync()
    {
        StatusMessage = "Connecting...";
        var result = await _connectUseCase.ExecuteAsync("Nova System");
        if (result.IsSuccess)
        {
            IsConnected = true;
            StatusMessage = "Connected";
        }
        else
        {
            StatusMessage = $"Connection failed: {result.Error}";
        }
    }
    
    private async Task DownloadBankAsync()
    {
        StatusMessage = "Downloading...";
        var result = await _downloadUseCase.ExecuteAsync();
        if (result.IsSuccess)
        {
            var bank = result.Value;
            Presets.Clear();
            foreach (var preset in bank.Presets)
            {
                Presets.Add(new PresetItemViewModel
                {
                    Number = preset.Number,
                    Name = preset.Name
                });
            }
            StatusMessage = $"Downloaded {bank.Presets.Length} presets";
        }
        else
        {
            StatusMessage = $"Download failed: {result.Error}";
        }
    }
}
```

---

## MIDI protocol detaljer (Modul 1)

### Request: User Bank Dump
```
F0 00 20 1F 00 63 45 03 F7
│  │  │  │  │  │  │  │  │
│  │  │  │  │  │  │  │  └─ F7: End marker
│  │  │  │  │  │  │  └────── 03: Request User Bank
│  │  │  │  │  │  └───────── 45: Request message ID
│  │  │  │  │  └──────────── 63: Model ID (Nova System)
│  │  │  │  └─────────────── 00: Device ID (0 = any, or 1-126)
│  │  │  └────────────────── 1F: TC Electronic ID part 3
│  │  └───────────────────── 20: TC Electronic ID part 2
│  └────────────────────────── 00: TC Electronic ID part 1
└─────────────────────────────── F0: Start marker
```

### Response: User Bank Dump (~31KB)
```
F0 00 20 1F 00 63 20 03 [Preset1] [Preset2] ... [Preset60] [Checksum] F7
```

Each preset: 520 bytes (F0 ... F7 not repeated)

---

## Test plan (Modul 1)

### Unit tests
```
SysExParsingTests.cs
├── ParseUserBankDump_ValidData_Returns60Presets
├── ParseUserBankDump_InvalidChecksum_ReturnsFail
├── ParsePreset_ExtractName_IsCorrect
├── CalculateChecksum_KnownData_MatchesExpected
└── ValidateChecksum_BadData_ReturnsFalse

ConnectUseCaseTests.cs
├── ExecuteAsync_ValidPort_ReturnsSuccess
└── ExecuteAsync_InvalidPort_ReturnsFailure

DownloadBankUseCaseTests.cs
├── ExecuteAsync_MockMidiReturnsBank_ParsesSuccessfully
├── ExecuteAsync_Timeout_ReturnsFailure
└── ExecuteAsync_BadChecksum_ReturnsFailure
```

### Integration tests
```
MidiRoundtripTests.cs
├── DownloadAndUpload_MockData_Roundtrips
└── HandleChunkedData_SplitAcrossPackets_BuffersCorrectly
```

### Manual tests
```
✓ Real pedal responds to request
✓ 60 presets downloaded without corruption
✓ Checksum validation catches real corruption
✓ Upload roundtrip works (compare before/after)
✓ Timeout triggers after 30s with Nova unplugged
✓ UI remains responsive during long download
```

---

## Success criteria (acceptance)
1. App connects to Nova System without errors
2. Download bank completes in <15 seconds
3. All 60 preset names are correct (no corruption)
4. Upload bank to pedal succeeds
5. Pedal confirms written data matches input (checksums correct)
6. If USB unplugged: timeout error (not crash)
7. All unit + integration tests pass
8. Manual test on real Nova System passes

---

## Deliverables (end of Modul 1)
- [ ] Working application (`.exe`)
- [ ] Main window with port picker + connect/download/upload buttons
- [ ] Preset list (60 names shown)
- [ ] Status bar with messages
- [ ] Full test suite (unit + integration)
- [ ] Basic documentation (setup, usage)
- [ ] GitHub repo with clean history
