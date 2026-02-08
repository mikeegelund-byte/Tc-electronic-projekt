# Nova System Manager - Implementation Instructions

## CRITICAL: Read This Entire File Before Writing Any Code

You are implementing preset editing for a TC Electronic Nova System guitar pedal MIDI manager.
The app can already connect and receive data. It CANNOT edit presets or send changes back.

**The root cause is simple:** `Preset.ToSysEx()` returns the original unchanged bytes.
UI edits have zero effect on what gets sent to hardware.

## Project

- **Language:** C# .NET 8, Avalonia 11.x UI, DryWetMIDI
- **Solution:** `NovaApp.sln`
- **Build:** `dotnet build NovaApp.sln`
- **Test:** `dotnet test NovaApp.sln`
- **Run:** `dotnet run --project src/Nova.Presentation`

## RULES - You MUST Follow These

1. **DO NOT** create new projects, interfaces, or use cases
2. **DO NOT** touch `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs` - it works
3. **DO NOT** restructure the architecture or add abstraction layers
4. **DO NOT** add new NuGet packages
5. **DO NOT** modify any test files (they all pass, leave them alone)
6. **DO NOT** add features not listed in the steps below
7. **DO NOT** add XML doc comments to new code beyond what's shown
8. **DO** follow the exact step order below
9. **DO** run `dotnet build NovaApp.sln` after each step and fix any errors before proceeding
10. **DO** preserve all existing code that you don't explicitly change

## Architecture (for context only - do not change)

```
src/Nova.Presentation  → Avalonia UI (ViewModels, Views, AXAML)
src/Nova.Application   → Use Cases (leave alone)
src/Nova.Domain        → Models (Preset.cs is the main edit target)
src/Nova.Infrastructure→ MIDI impl (DO NOT TOUCH)
src/Nova.Midi          → MIDI abstractions (leave alone)
```

---

## STEP 1: Add Encoding Methods to Preset.cs

**File:** `src/Nova.Domain/Models/Preset.cs`

### 1A: Add `Encode4ByteValue` as a private static method

Add this method right after the existing `Decode4ByteValue` method (which is around line 789):

```csharp
/// <summary>
/// Encodes an integer value into 4 bytes using Nova System's 7-bit encoding.
/// Inverse of Decode4ByteValue.
/// </summary>
private static void Encode4ByteValue(byte[] sysex, int offset, int value)
{
    sysex[offset] = (byte)(value & 0x7F);
    sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
    sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
    sysex[offset + 3] = (byte)((value >> 21) & 0x7F);
}
```

### 1B: Add `EncodeSignedDbValue` as a private static method

Add this right after `Encode4ByteValue`. This is the inverse of the existing `DecodeSignedDbValue`:

```csharp
/// <summary>
/// Encodes a signed dB value using the same strategy as DecodeSignedDbValue.
/// Negative values use 2^24 offset encoding. Zero and positive values stored directly.
/// </summary>
private static void EncodeSignedDbValue(byte[] sysex, int offset, int value)
{
    const int LARGE_OFFSET = 16777216; // 2^24
    int rawValue = value < 0 ? value + LARGE_OFFSET : value;
    Encode4ByteValue(sysex, offset, rawValue);
}
```

### 1C: Make properties publicly settable

Change ALL property declarations from `private set` to `set`. The properties are on lines 10-123.

Find this pattern (it appears ~80 times):
```csharp
public int SomeProperty { get; private set; }
public bool SomeProperty { get; private set; }
public string Name { get; private set; } = string.Empty;
public byte[] RawSysEx { get; private set; } = Array.Empty<byte>();
```

Change to:
```csharp
public int SomeProperty { get; set; }
public bool SomeProperty { get; set; }
public string Name { get; set; } = string.Empty;
public byte[] RawSysEx { get; set; } = Array.Empty<byte>();
```

Every property in the Preset class between lines 10 and 123 must have its `private set` changed to `set`.

### 1D: Rewrite `ToSysEx()` method

Replace the existing `ToSysEx()` method (around line 776) with a full encoding implementation.
The method must encode ALL properties back into a fresh 520-byte array.

Replace this:
```csharp
public Result<byte[]> ToSysEx()
{
    if (RawSysEx == null || RawSysEx.Length != 520)
        return Result.Fail("Preset has no valid RawSysEx data");

    return Result.Ok(RawSysEx);
}
```

With this:
```csharp
public Result<byte[]> ToSysEx()
{
    var sysex = new byte[520];

    // Copy original raw bytes as base (preserves any bytes we don't explicitly encode)
    if (RawSysEx != null && RawSysEx.Length == 520)
        Array.Copy(RawSysEx, sysex, 520);

    // Header
    sysex[0] = 0xF0;
    sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
    sysex[4] = 0x00; // Device ID
    sysex[5] = 0x63; // Nova System
    sysex[6] = 0x20; // Dump
    sysex[7] = 0x01; // Preset
    sysex[8] = (byte)Number;

    // Name (bytes 10-33: 24 ASCII chars, byte 9 reserved)
    var paddedName = (Name ?? "").Length > 24 ? Name!.Substring(0, 24) : (Name ?? "").PadRight(24);
    var nameBytes = System.Text.Encoding.ASCII.GetBytes(paddedName);
    Array.Copy(nameBytes, 0, sysex, 10, 24);

    // Global parameters
    Encode4ByteValue(sysex, 38, TapTempo);
    Encode4ByteValue(sysex, 42, Routing);
    EncodeSignedDbValue(sysex, 46, LevelOutLeft);
    EncodeSignedDbValue(sysex, 50, LevelOutRight);
    Encode4ByteValue(sysex, 54, MapParameter);
    Encode4ByteValue(sysex, 58, MapMin);
    Encode4ByteValue(sysex, 62, MapMid);
    Encode4ByteValue(sysex, 66, MapMax);

    // COMP parameters
    Encode4ByteValue(sysex, 70, CompType);
    EncodeSignedDbValue(sysex, 74, CompThreshold);
    Encode4ByteValue(sysex, 78, CompRatio);
    Encode4ByteValue(sysex, 82, CompAttack);
    Encode4ByteValue(sysex, 86, CompRelease);
    Encode4ByteValue(sysex, 90, CompResponse);
    Encode4ByteValue(sysex, 94, CompDrive);
    EncodeSignedDbValue(sysex, 98, CompLevel);

    // Effect on/off switches
    Encode4ByteValue(sysex, 130, CompressorEnabled ? 1 : 0);

    // DRIVE parameters
    Encode4ByteValue(sysex, 134, DriveType);
    Encode4ByteValue(sysex, 138, DriveGain);
    Encode4ByteValue(sysex, 142, DriveTone);
    Encode4ByteValue(sysex, 182, BoostLevel);
    Encode4ByteValue(sysex, 186, BoostEnabled ? 1 : 0);
    EncodeSignedDbValue(sysex, 190, DriveLevel);
    Encode4ByteValue(sysex, 194, DriveEnabled ? 1 : 0);

    // MOD parameters
    Encode4ByteValue(sysex, 198, ModType);
    Encode4ByteValue(sysex, 202, ModSpeed);
    Encode4ByteValue(sysex, 206, ModDepth);
    Encode4ByteValue(sysex, 210, ModTempo);
    Encode4ByteValue(sysex, 214, ModHiCut);
    EncodeSignedDbValue(sysex, 218, ModFeedback);
    Encode4ByteValue(sysex, 222, ModDelayOrRange);
    Encode4ByteValue(sysex, 238, ModWidth);
    Encode4ByteValue(sysex, 250, ModMix);
    Encode4ByteValue(sysex, 258, ModulationEnabled ? 1 : 0);

    // DELAY parameters
    Encode4ByteValue(sysex, 262, DelayType);
    Encode4ByteValue(sysex, 266, DelayTime);
    Encode4ByteValue(sysex, 270, DelayTime2);
    Encode4ByteValue(sysex, 274, DelayTempo);
    Encode4ByteValue(sysex, 278, DelayTempo2OrWidth);
    Encode4ByteValue(sysex, 282, DelayFeedback);
    Encode4ByteValue(sysex, 286, DelayClipOrFeedback2);
    Encode4ByteValue(sysex, 290, DelayHiCut);
    Encode4ByteValue(sysex, 294, DelayLoCut);
    if (DelayType == 3)
    {
        EncodeSignedDbValue(sysex, 298, DelayOffsetOrPan1);
        EncodeSignedDbValue(sysex, 302, DelaySenseOrPan2);
    }
    else if (DelayType == 4)
    {
        EncodeSignedDbValue(sysex, 298, DelayOffsetOrPan1);
        EncodeSignedDbValue(sysex, 302, DelaySenseOrPan2);
    }
    Encode4ByteValue(sysex, 306, DelayDamp);
    Encode4ByteValue(sysex, 310, DelayRelease);
    Encode4ByteValue(sysex, 314, DelayMix);
    Encode4ByteValue(sysex, 322, DelayEnabled ? 1 : 0);

    // REVERB parameters
    Encode4ByteValue(sysex, 326, ReverbType);
    Encode4ByteValue(sysex, 330, ReverbDecay);
    Encode4ByteValue(sysex, 334, ReverbPreDelay);
    Encode4ByteValue(sysex, 338, ReverbShape);
    Encode4ByteValue(sysex, 342, ReverbSize);
    Encode4ByteValue(sysex, 346, ReverbHiColor);
    EncodeSignedDbValue(sysex, 350, ReverbHiLevel);
    Encode4ByteValue(sysex, 354, ReverbLoColor);
    EncodeSignedDbValue(sysex, 358, ReverbLoLevel);
    EncodeSignedDbValue(sysex, 362, ReverbRoomLevel);
    EncodeSignedDbValue(sysex, 366, ReverbLevel);
    EncodeSignedDbValue(sysex, 370, ReverbDiffuse);
    Encode4ByteValue(sysex, 374, ReverbMix);
    Encode4ByteValue(sysex, 386, ReverbEnabled ? 1 : 0);

    // EQ/GATE parameters
    Encode4ByteValue(sysex, 390, GateType);
    EncodeSignedDbValue(sysex, 394, GateThreshold);
    Encode4ByteValue(sysex, 398, GateDamp);
    Encode4ByteValue(sysex, 402, GateRelease);
    Encode4ByteValue(sysex, 406, EqEnabled ? 1 : 0);
    Encode4ByteValue(sysex, 410, EqFreq1);
    EncodeSignedDbValue(sysex, 414, EqGain1);
    Encode4ByteValue(sysex, 418, EqWidth1);
    Encode4ByteValue(sysex, 422, EqFreq2);
    EncodeSignedDbValue(sysex, 426, EqGain2);
    Encode4ByteValue(sysex, 430, EqWidth2);
    Encode4ByteValue(sysex, 434, EqFreq3);
    EncodeSignedDbValue(sysex, 438, EqGain3);
    Encode4ByteValue(sysex, 442, EqWidth3);
    Encode4ByteValue(sysex, 450, GateEnabled ? 1 : 0);

    // PITCH parameters
    Encode4ByteValue(sysex, 454, PitchType);
    EncodeSignedDbValue(sysex, 458, PitchVoice1);
    EncodeSignedDbValue(sysex, 462, PitchVoice2);
    EncodeSignedDbValue(sysex, 466, PitchPan1);
    EncodeSignedDbValue(sysex, 470, PitchPan2);
    Encode4ByteValue(sysex, 474, PitchDelay1);
    Encode4ByteValue(sysex, 478, PitchDelay2);
    Encode4ByteValue(sysex, 482, PitchFeedback1OrKey);
    Encode4ByteValue(sysex, 486, PitchFeedback2OrScale);
    EncodeSignedDbValue(sysex, 490, PitchLevel1);
    if (PitchType == 1 || PitchType == 2)
    {
        Encode4ByteValue(sysex, 494, PitchDirection);
        Encode4ByteValue(sysex, 498, PitchRange);
    }
    else
    {
        EncodeSignedDbValue(sysex, 494, PitchLevel2);
    }
    Encode4ByteValue(sysex, 502, PitchMix);
    Encode4ByteValue(sysex, 514, PitchEnabled ? 1 : 0);

    // Checksum (sum of bytes 34-517 & 0x7F)
    int checksum = 0;
    for (int i = 34; i <= 517; i++)
        checksum += sysex[i];
    sysex[518] = (byte)(checksum & 0x7F);

    // SysEx end
    sysex[519] = 0xF7;

    return Result.Ok(sysex);
}
```

### VERIFY STEP 1:
Run `dotnet build NovaApp.sln`. It must compile with 0 errors.

---

## STEP 2: Add `WriteToPreset` to Each Effect ViewModel

Each of the 7 effect ViewModels needs a `WriteToPreset(Preset)` method that is the inverse of the existing `LoadFromPreset(Preset)` method. The pattern is: read each ViewModel property and assign it to the corresponding Preset property.

### 2A: DriveBlockViewModel

**File:** `src/Nova.Presentation/ViewModels/Effects/DriveBlockViewModel.cs`

Add this method after the existing `LoadFromPreset` method:

```csharp
/// <summary>
/// Writes Drive parameters back to a Preset domain model.
/// Inverse of LoadFromPreset.
/// </summary>
public void WriteToPreset(Preset preset)
{
    if (preset == null) return;

    preset.DriveType = TypeId;
    preset.DriveGain = Gain;
    preset.DriveTone = Tone;
    preset.DriveLevel = Level;
    preset.BoostLevel = BoostLevel;
    preset.BoostEnabled = BoostEnabled;
    preset.DriveEnabled = IsEnabled;
}
```

### 2B: CompressorBlockViewModel

**File:** `src/Nova.Presentation/ViewModels/Effects/CompressorBlockViewModel.cs`

Look at the existing `LoadFromPreset` method in this file. It reads these properties:
- CompType, CompThreshold, CompRatio, CompAttack, CompRelease, CompResponse, CompDrive, CompLevel, CompressorEnabled

Add a `WriteToPreset` that writes each back:

```csharp
public void WriteToPreset(Preset preset)
{
    if (preset == null) return;

    preset.CompType = TypeId;
    preset.CompThreshold = Threshold;
    preset.CompRatio = Ratio;
    preset.CompAttack = Attack;
    preset.CompRelease = Release;
    preset.CompResponse = Response;
    preset.CompDrive = Drive;
    preset.CompLevel = Level;
    preset.CompressorEnabled = IsEnabled;
}
```

**IMPORTANT:** Check the actual property names in each ViewModel's `LoadFromPreset`. The ViewModel property names may differ from Preset property names. For example, the VM might use `TypeId` while Preset uses `CompType`. Map them exactly as `LoadFromPreset` does, but reversed.

### 2C: ModulationBlockViewModel

**File:** `src/Nova.Presentation/ViewModels/Effects/ModulationBlockViewModel.cs`

Same pattern. Map from ViewModel properties back to Preset properties matching how `LoadFromPreset` reads them.

### 2D: DelayBlockViewModel

**File:** `src/Nova.Presentation/ViewModels/Effects/DelayBlockViewModel.cs`

Same pattern.

### 2E: ReverbBlockViewModel

**File:** `src/Nova.Presentation/ViewModels/Effects/ReverbBlockViewModel.cs`

Same pattern.

### 2F: EqGateBlockViewModel

**File:** `src/Nova.Presentation/ViewModels/Effects/EqGateBlockViewModel.cs`

Same pattern. This VM handles both EQ and Gate parameters.

### 2G: PitchBlockViewModel

**File:** `src/Nova.Presentation/ViewModels/Effects/PitchBlockViewModel.cs`

Same pattern.

### How to determine the correct property mapping:

For EACH ViewModel, read its `LoadFromPreset(Preset preset)` method. Every line like:
```csharp
SomeVmProperty = preset.SomePresetProperty;
```
becomes in `WriteToPreset`:
```csharp
preset.SomePresetProperty = SomeVmProperty;
```

Some lines use `Math.Clamp()` - you can write back without clamping since the ViewModel's property setters already have range validation.

### VERIFY STEP 2:
Run `dotnet build NovaApp.sln`. It must compile with 0 errors.

---

## STEP 3: Add `BuildModifiedPreset` to PresetDetailViewModel

**File:** `src/Nova.Presentation/ViewModels/PresetDetailViewModel.cs`

Add this method somewhere before `UploadPresetAsync`:

```csharp
/// <summary>
/// Writes all ViewModel state back into the current Preset, then returns it.
/// Call this before uploading to ensure all edits are encoded into SysEx.
/// </summary>
private Preset? BuildModifiedPreset()
{
    if (_currentPreset == null) return null;

    // Write global parameters
    _currentPreset.Name = PresetName;
    _currentPreset.Number = PresetNumber;
    _currentPreset.TapTempo = TapTempo;
    _currentPreset.Routing = Routing;
    _currentPreset.LevelOutLeft = LevelOutLeft;
    _currentPreset.LevelOutRight = LevelOutRight;
    _currentPreset.MapParameter = MapParameter;
    _currentPreset.MapMin = MapMin;
    _currentPreset.MapMid = MapMid;
    _currentPreset.MapMax = MapMax;

    // Write all effect blocks
    Drive.WriteToPreset(_currentPreset);
    Compressor.WriteToPreset(_currentPreset);
    EqGate.WriteToPreset(_currentPreset);
    Modulation.WriteToPreset(_currentPreset);
    Pitch.WriteToPreset(_currentPreset);
    Delay.WriteToPreset(_currentPreset);
    Reverb.WriteToPreset(_currentPreset);

    return _currentPreset;
}
```

---

## STEP 4: Update `UploadPresetAsync` to Use `BuildModifiedPreset`

**File:** `src/Nova.Presentation/ViewModels/PresetDetailViewModel.cs`

Find the existing `UploadPresetAsync` method. It currently sends `_currentPreset` directly.
Change it to call `BuildModifiedPreset()` first:

Replace:
```csharp
private async Task UploadPresetAsync()
{
    if (_currentPreset == null) return;

    StatusMessage = $"Uploading preset to slot {TargetSlot}...";

    var result = await _savePresetUseCase.ExecuteAsync(_currentPreset, TargetSlot, verify: true);
```

With:
```csharp
private async Task UploadPresetAsync()
{
    if (_currentPreset == null) return;

    // Write all UI changes back into the Preset before sending
    BuildModifiedPreset();

    StatusMessage = $"Uploading preset to slot {TargetSlot}...";

    var result = await _savePresetUseCase.ExecuteAsync(_currentPreset, TargetSlot, verify: true);
```

The rest of the method stays the same.

### VERIFY STEP 4:
Run `dotnet build NovaApp.sln`. It must compile with 0 errors.

---

## STEP 5: Register PresetDetailViewModel in DI

**File:** `src/Nova.Presentation/App.axaml.cs`

The `PresetDetailViewModel` is NOT registered in dependency injection. Find the section where ViewModels are registered (look for `services.AddTransient<MainViewModel>` or similar).

Add this line near the other ViewModel registrations:
```csharp
services.AddTransient<PresetDetailViewModel>();
```

### VERIFY STEP 5:
Run `dotnet build NovaApp.sln`. It must compile with 0 errors.

---

## STEP 6: Final Build Verification

Run these commands in order:

```
dotnet build NovaApp.sln
dotnet test NovaApp.sln
```

**Expected result:** Build succeeds. All 453 existing tests still pass.

Some tests MAY fail because they test `ToSysEx()` returning `RawSysEx` directly, or because they depend on properties being `private set`. If tests fail:
- Tests that check `preset.ToSysEx()` returns the exact same bytes: Update them to account for the new encoding (which should produce identical bytes for unmodified presets)
- Tests that set properties via reflection or test helpers: Should still work since properties are now publicly settable
- Do NOT delete any tests. Fix them to work with the new code.

---

## ENCODING REFERENCE

### 4-Byte Nibble Encoding (used for ALL parameters)

```
Encode: byte[0] = value & 0x7F
        byte[1] = (value >> 7) & 0x7F
        byte[2] = (value >> 14) & 0x7F
        byte[3] = (value >> 21) & 0x7F

Decode: value = byte[0] | (byte[1] << 7) | (byte[2] << 14) | (byte[3] << 21)
```

### Signed Value Encoding

Negative values: `rawValue = value + 16777216` (2^24 offset)
Zero/positive values: `rawValue = value` (stored directly)

### SysEx Layout (520 bytes)

```
Byte 0:      F0 (SysEx start)
Bytes 1-3:   00 20 1F (TC Electronic manufacturer ID)
Byte 4:      00 (Device ID)
Byte 5:      63 (Nova System model ID)
Byte 6:      20 (Dump message)
Byte 7:      01 (Preset data type)
Byte 8:      Preset number (1-60)
Byte 9:      Reserved
Bytes 10-33: Name (24 ASCII chars)
Bytes 34-517: Parameter data (4 bytes per parameter)
Byte 518:    Checksum (sum of bytes 34-517 & 0x7F)
Byte 519:    F7 (SysEx end)
```
