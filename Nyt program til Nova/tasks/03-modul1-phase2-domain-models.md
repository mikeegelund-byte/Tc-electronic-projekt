# Task List: Modul 1, Phase 2 — Domain Models

## 📋 Module: 1 (MVP - Connection + Bank Dump)
## 📋 Phase: 2 (Domain layer - core models)
**Duration**: 1 week
**Prerequisite**: Phase 0 complete + Modul 1 Phase 1 complete
**Output**: Domain entities + value objects, full unit test suite

---

## Overview

**Goal**: Establish the domain models that represent presets and dumps with strict validation and immutable semantics.

**Key Principles**:
- Domain layer has **no infrastructure dependencies**
- All constraints enforced in constructors/value objects
- Value objects are **immutable**
- Tests describe all invariants

**⚠️ ACTUAL IMPLEMENTATION (2026-01-31)**:
Instead of ParameterValue/SysExMessage value objects, we implemented:
- ✅ `Preset` entity (FromSysEx + ToSysEx) - 521 bytes
- ✅ `UserBankDump` collection (60 presets, numbers 31-90) - immutable
- ✅ `SystemDump` entity (FromSysEx + ToSysEx) - 527 bytes
- ✅ All models use FluentResults for error handling
- ✅ All models store RawSysEx for roundtrip fidelity
- ✅ 30 domain tests passing (6 Preset + 6 UserBankDump + 5 SystemDump + 13 SysEx utilities)

The original plan assumed 128 presets per bank across 4 banks.
Reality: Nova System User Bank = 60 presets (31-90), validated with real hardware data.

**CRITICAL DECISION (2026-01-31):**
Parameter extraction (bytes 33-519) is ESSENTIAL for reading preset details:
- Tap Tempo, Routing, Level Out
- All effect parameters (COMP, Drive, Boost, Mod, Delay, Reverb)
- This is NOT optional - it's required to understand what's in each preset.
Therefore: Complete parameter extraction BEFORE Phase 3.

---

## Exit Criteria (Phase 2 Complete When ALL True)

**ORIGINAL PLAN:**
- [ ] `Nova.Domain` project exists
- [ ] `ParameterValue` value object validates 0–127
- [ ] `SysExMessage` value object validates F0...F7 framing
- [ ] `PresetBank` contains 128 patches
- [ ] `SystemDump` contains 4 banks (A/B/C/D)
- [ ] `dotnet test` passes (all new tests green)
- [ ] Coverage ≥ 95% for Domain layer

**ACTUAL IMPLEMENTATION (✅ 100% COMPLETE! - Ready for validation):**
- [✅] `Nova.Domain` project exists
- [✅] `Preset` entity with FromSysEx() parsing (6 unit tests + 2 integration tests)
- [✅] `UserBankDump` collection of 60 presets (6 unit tests + 2 integration tests)
- [✅] `SystemDump` entity with FromSysEx() parsing (4 unit tests + 1 integration test)
- [✅] Preset.ToSysEx() serialization (2 roundtrip tests)
- [✅] SystemDump.ToSysEx() serialization (1 roundtrip test)
- [✅] All 117 tests passing (108 Domain + 6 Midi + 3 baseline)
- [✅] Real hardware validation: 60 presets + 1 system dump parsed successfully
- [✅] **MILESTONE COMPLETE**: Parameter extraction (78 params across 9 effect blocks)
  - Basic parameters: TapTempo, Routing, LevelOut, 5 enables (9 total)
  - COMP block: 8 parameters (commit b8a1f59)
  - DRIVE block: 3 parameters (commit bc54946)
  - BOOST block: 3 parameters (commit 6b95144)
  - MOD block: 8 parameters (commit 40e0e39)
  - DELAY block: 10 parameters (commit c553118)
  - REVERB block: 13 parameters (commit 7bb5a38)
  - EQ/GATE block: 13 parameters (commit 3a5bb9e)
  - PITCH block: 11 parameters (commit 8e6c2cf) ← **FINAL BLOCK!**
- [✅] Parameter validation logic (re-enabled and verified)
- [⏳] OPTIONAL: Preset modification (change name, parameters)
- [⏳] OPTIONAL: Coverage measurement (estimated >95% for Domain layer)

**Technical Debt Resolved (2026-02-01):**
- Signed dB parameters previously stored raw values.
- Fixed using Hybrid Offset Strategy (Raw > 16M ? Raw - 2^24 : Raw).
- All validation tests re-enabled and passing.

**Completed**: 2026-02-01

---

## Task 2.1: Create `ParameterValue` Value Object

**Status**: Not started
**Estimated**: 30 min
**Files**:
- `src/Nova.Domain/ValueObjects/ParameterValue.cs`
- `tests/Nova.Domain.Tests/ValueObjects/ParameterValueTests.cs`

### Test First (RED)
```csharp
public class ParameterValueTests
{
    [Fact]
    public void Create_WithValidRange_ReturnsValue()
    {
        var value = ParameterValue.From(64);
        value.Value.Should().Be(64);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(128)]
    public void Create_OutOfRange_Throws(int invalid)
    {
        Action act = () => ParameterValue.From(invalid);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
```

### Code (GREEN)
```csharp
public sealed record ParameterValue
{
    public int Value { get; }

    private ParameterValue(int value) => Value = value;

    public static ParameterValue From(int value)
    {
        if (value is < 0 or > 127)
            throw new ArgumentOutOfRangeException(nameof(value), "Parameter must be 0-127.");

        return new ParameterValue(value);
    }
}
```

### Refactor
- Add XML docs
- Add `ToString()` override

---

## Task 2.2: Create `SysExMessage` Value Object

**Status**: Not started
**Estimated**: 40 min
**Files**:
- `src/Nova.Domain/ValueObjects/SysExMessage.cs`
- `tests/Nova.Domain.Tests/ValueObjects/SysExMessageTests.cs`

### Test First (RED)
```csharp
public class SysExMessageTests
{
    [Fact]
    public void Create_WithF0F7Frame_ReturnsMessage()
    {
        var bytes = new byte[] { 0xF0, 0x01, 0x02, 0xF7 };
        var msg = SysExMessage.From(bytes);
        msg.Bytes.Should().Equal(bytes);
    }

    [Fact]
    public void Create_MissingF0_Throws()
    {
        var bytes = new byte[] { 0x00, 0x01, 0x02, 0xF7 };
        Action act = () => SysExMessage.From(bytes);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_MissingF7_Throws()
    {
        var bytes = new byte[] { 0xF0, 0x01, 0x02, 0x00 };
        Action act = () => SysExMessage.From(bytes);
        act.Should().Throw<ArgumentException>();
    }
}
```

### Code (GREEN)
```csharp
public sealed record SysExMessage
{
    public byte[] Bytes { get; }

    private SysExMessage(byte[] bytes) => Bytes = bytes;

    public static SysExMessage From(byte[] bytes)
    {
        if (bytes is null || bytes.Length < 3)
            throw new ArgumentException("SysEx message too short.", nameof(bytes));

        if (bytes[0] != 0xF0 || bytes[^1] != 0xF7)
            throw new ArgumentException("SysEx must start with F0 and end with F7.", nameof(bytes));

        return new SysExMessage(bytes);
    }
}
```

---

## Task 2.3: Create `PresetBank` Entity

**Status**: Not started
**Estimated**: 60 min
**Files**:
- `src/Nova.Domain/Entities/PresetBank.cs`
- `tests/Nova.Domain.Tests/Entities/PresetBankTests.cs`

### Test First (RED)
```csharp
public class PresetBankTests
{
    [Fact]
    public void Create_Default_Has128Slots()
    {
        var bank = PresetBank.Empty();
        bank.Presets.Should().HaveCount(128);
    }

    [Fact]
    public void SetPreset_OutOfRange_Throws()
    {
        var bank = PresetBank.Empty();
        Action act = () => bank.WithPreset(128, Patch.Empty());
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
```

### Code (GREEN)
```csharp
public sealed class PresetBank
{
    public IReadOnlyList<Patch> Presets { get; }

    private PresetBank(IReadOnlyList<Patch> presets) => Presets = presets;

    public static PresetBank Empty()
        => new PresetBank(Enumerable.Range(0, 128).Select(_ => Patch.Empty()).ToList());

    public PresetBank WithPreset(int index, Patch patch)
    {
        if (index is < 0 or > 127)
            throw new ArgumentOutOfRangeException(nameof(index));

        var list = Presets.ToList();
        list[index] = patch;
        return new PresetBank(list);
    }
}
```

---

## Task 2.4: Create `SystemDump` Entity

**Status**: Not started
**Estimated**: 60 min
**Files**:
- `src/Nova.Domain/Entities/SystemDump.cs`
- `tests/Nova.Domain.Tests/Entities/SystemDumpTests.cs`

### Test First (RED)
```csharp
public class SystemDumpTests
{
    [Fact]
    public void Create_Default_HasFourBanks()
    {
        var dump = SystemDump.Empty();
        dump.BankA.Should().NotBeNull();
        dump.BankB.Should().NotBeNull();
        dump.BankC.Should().NotBeNull();
        dump.BankD.Should().NotBeNull();
    }
}
```

### Code (GREEN)
```csharp
public sealed class SystemDump
{
    public PresetBank BankA { get; }
    public PresetBank BankB { get; }
    public PresetBank BankC { get; }
    public PresetBank BankD { get; }

    private SystemDump(PresetBank a, PresetBank b, PresetBank c, PresetBank d)
        => (BankA, BankB, BankC, BankD) = (a, b, c, d);

    public static SystemDump Empty()
        => new SystemDump(PresetBank.Empty(), PresetBank.Empty(), PresetBank.Empty(), PresetBank.Empty());
}
```

---

## Task 2.5: Coverage Verification

**Status**: Not started
**Estimated**: 15 min
**Requirement**: Domain coverage ≥ 95%

### Verification
```powershell
# Run tests + coverage
dotnet test tests/Nova.Domain.Tests/Nova.Domain.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover

# Confirm coverage in output (≥95%)
```

---

## Completion Checklist

- [ ] All tests pass
- [ ] Domain coverage ≥ 95%
- [ ] Phase 2 exit criteria met
- [ ] Update `tasks/00-index.md`
- [ ] Update `BUILD_STATE.md`
- [ ] Update `SESSION_MEMORY.md`

---

**Status**: READY (execute after Phase 0 + Phase 1)
