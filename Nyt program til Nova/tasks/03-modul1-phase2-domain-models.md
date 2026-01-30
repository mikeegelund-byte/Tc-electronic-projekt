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

---

## Exit Criteria (Phase 2 Complete When ALL True)

- [ ] `Nova.Domain` project exists
- [ ] `ParameterValue` value object validates 0–127
- [ ] `SysExMessage` value object validates F0...F7 framing
- [ ] `PresetBank` contains 128 patches
- [ ] `SystemDump` contains 4 banks (A/B/C/D)
- [ ] `dotnet test` passes (all new tests green)
- [ ] Coverage ≥ 95% for Domain layer

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
