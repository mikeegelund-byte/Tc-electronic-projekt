# Task List: Modul 1, Phase 3 — Application Use Cases

## 📋 Module: 1 (MVP - Connection + Bank Dump)
## 📋 Phase: 3 (Application layer - use cases)
**Duration**: 1 week  
**Prerequisite**: Phase 0 complete + Modul 1 Phase 1 & 2 complete  
**Output**: Connect + Download bank use cases with tests  

---

## Overview

**Goal**: Build application-layer use cases that orchestrate MIDI I/O and domain models.

**Rules**:
- Application layer depends on Domain + Infrastructure abstractions only
- Use cases are orchestrators; no business rules inside
- All async operations cancellable

---

## Exit Criteria (Phase 3 Complete When ALL True)

- [ ] `ConnectUseCase` implemented and tested
- [ ] `DownloadBankUseCase` implemented and tested
- [ ] Uses `IMidiPort` abstraction (no direct MIDI calls)
- [ ] `dotnet test` passes
- [ ] Application layer coverage ≥ 80%

---

## Task 3.1: Implement `ConnectUseCase`

**Status**: Not started  
**Estimated**: 60 min  
**Files**:
- `src/Nova.Application/UseCases/ConnectUseCase.cs`
- `tests/Nova.Application.Tests/UseCases/ConnectUseCaseTests.cs`

### Test First (RED)
```csharp
public class ConnectUseCaseTests
{
    [Fact]
    public async Task Execute_WhenPortConnects_ReturnsSuccess()
    {
        var midi = new Mock<IMidiPort>();
        midi.Setup(m => m.ConnectAsync("Nova System")).ReturnsAsync(Result.Ok());

        var useCase = new ConnectUseCase(midi.Object);
        var result = await useCase.ExecuteAsync("Nova System");

        result.IsSuccess.Should().BeTrue();
    }
}
```

### Code (GREEN)
```csharp
public sealed class ConnectUseCase
{
    private readonly IMidiPort _midiPort;

    public ConnectUseCase(IMidiPort midiPort) => _midiPort = midiPort;

    public Task<Result> ExecuteAsync(string portName)
        => _midiPort.ConnectAsync(portName);
}
```

---

## Task 3.2: Implement `DownloadBankUseCase`

**Status**: Not started  
**Estimated**: 90 min  
**Files**:
- `src/Nova.Application/UseCases/DownloadBankUseCase.cs`
- `tests/Nova.Application.Tests/UseCases/DownloadBankUseCaseTests.cs`

### Test First (RED)
```csharp
public class DownloadBankUseCaseTests
{
    [Fact]
    public async Task Execute_WhenSysExReceived_ReturnsSystemDump()
    {
        var midi = new Mock<IMidiPort>();
        var response = new byte[] { 0xF0, 0x00, 0x20, 0x1F, 0x00, 0x63, 0x45, 0x03, 0xF7 };

        midi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>())).ReturnsAsync(Result.Ok());
        midi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var useCase = new DownloadBankUseCase(midi.Object);
        var result = await useCase.ExecuteAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }
}
```

### Code (GREEN)
```csharp
public sealed class DownloadBankUseCase
{
    private readonly IMidiPort _midiPort;

    public DownloadBankUseCase(IMidiPort midiPort) => _midiPort = midiPort;

    public async Task<Result<SystemDump>> ExecuteAsync(CancellationToken ct)
    {
        var request = SysExBuilder.BuildBankDumpRequest();
        var send = await _midiPort.SendSysExAsync(request);
        if (send.IsFailed) return Result.Fail(send.Errors.First().Message);

        var response = await _midiPort.ReceiveSysExAsync(ct);
        var sysEx = SysExMessage.From(response);

        // Placeholder mapping until parser exists
        var dump = SystemDump.Empty();
        return Result.Ok(dump);
    }
}
```

---

## Task 3.3: Coverage Verification

**Status**: Not started  
**Estimated**: 15 min  
**Requirement**: Application coverage ≥ 80%

### Verification
```powershell
# Run tests + coverage
dotnet test tests/Nova.Application.Tests/Nova.Application.Tests.csproj \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover

# Confirm coverage in output (≥80%)
```

---

## Completion Checklist

- [ ] All tests pass
- [ ] Application coverage ≥ 80%
- [ ] Phase 3 exit criteria met
- [ ] Update `tasks/00-index.md`
- [ ] Update `BUILD_STATE.md`
- [ ] Update `SESSION_MEMORY.md`

---

**Status**: READY (execute after Phase 0 + Phase 1 + Phase 2)
