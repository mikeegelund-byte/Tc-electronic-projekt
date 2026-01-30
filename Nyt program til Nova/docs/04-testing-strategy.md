# Teststrategi

## Målsætning
**Høj stabilitet, lav risiko, disciplineret udvikling**

Hver modul skal være 100% test-grøn før næste module starter.

---

## Test pyramid

```
        ╱╲
       ╱  ╲      UI Integration (10%)
      ╱────╲
     ╱      ╲    Integration tests (30%)
    ╱────────╲
   ╱          ╲  Unit tests (60%)
  ╱────────────╲
```

---

## Unit Tests (60%)
**Hvad:** Parsing, validation, domain logic  
**Framework:** xUnit + FluentAssertions  
**Mocking:** Moq for interfaces

### Eksempler (Modul 1)
```csharp
[Fact]
public void ParseUserBankDump_ValidSysEx_ReturnsBank()
{
    // Arrange
    var validSysEx = TestFixtures.LoadUserBankDump();
    var parser = new UserBankDumpParser();
    
    // Act
    var result = parser.Parse(validSysEx);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Presets.Should().HaveCount(60);
}

[Fact]
public void ValidateChecksum_BadData_ReturnsFalse()
{
    // Arrange
    var corruptData = TestFixtures.LoadUserBankDump()
        .With(data => data[518] = 0xFF); // Wrong checksum
    
    // Act
    var valid = SysExValidator.IsValid(corruptData);
    
    // Assert
    valid.Should().BeFalse();
}
```

---

## Integration Tests (30%)
**Hvad:** MIDI I/O roundtrip, end-to-end flows  
**Mocking:** IMidiPort mocked with test data

### Eksempler (Modul 1)
```csharp
[Fact]
public async Task DownloadBankUseCase_RequestAndReceive_UpdatesViewModel()
{
    // Arrange
    var mockMidiPort = new Mock<IMidiPort>();
    mockMidiPort
        .Setup(p => p.SendSysExAsync(It.IsAny<byte[]>()))
        .Returns(Task.CompletedTask);
    mockMidiPort
        .Setup(p => p.ReceiveSysExAsync())
        .Returns(TestFixtures.StreamUserBankDump()); // Stream chunks
    
    var useCase = new DownloadBankUseCase(mockMidiPort.Object);
    var viewModel = new MainViewModel(useCase);
    
    // Act
    await useCase.ExecuteAsync();
    
    // Assert
    viewModel.PresetNames.Should().HaveCount(60);
    viewModel.StatusMessage.Should().Contain("success");
}
```

---

## UI Integration Tests (10%)
**Hvad:** UI binding, user interactions  
**Framework:** Avalonia TestHost (later phase)

---

## Test fixtures
```
NovaApp.Tests/
├── Fixtures/
│   ├── user-bank-dump-valid.syx
│   ├── user-bank-dump-bad-checksum.syx
│   ├── system-dump-valid.syx
│   └── TestData.cs (helpers)
└── Unit/
    ├── SysExParsingTests.cs
    ├── ChecksumValidationTests.cs
    └── ...
```

---

## Test gate per module

| Modul | Unit % | Integration % | Manual | Gate |
|-------|--------|---------------|--------|------|
| 1 | 100% | 100% | Yes (real pedal) | ✅ Before Modul 2 |
| 2 | 100% | 100% | Nej (UI only) | ✅ Before Modul 3 |
| 3 | 100% | 100% | Yes | ✅ Before Modul 4 |

**Rule:** Ingen commit til main uden grøn test suite.

---

## Running tests
```bash
# All tests
dotnet test

# Specific file
dotnet test NovaApp.Tests/Unit/SysExParsingTests.cs

# With coverage (later)
dotnet test /p:CollectCoverage=true
```

---

## CI/CD (GitHub Actions, later)
```yaml
on: [push, pull_request]
jobs:
  test:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
      - run: dotnet test
```

---

## Mock MIDI strategy
Ikke real MIDI i tests. Alle tests bruger:
- `Mock<IMidiPort>` med `Returns(TestData)`
- Fixtures: real SysEx fra Nova-System (eller constructor'ed)
- Timeouts simuleret med `Task.Delay()`

---

## Wat NOT to test
- Avalonia framework (trusted, don't mock internals)
- DryWetMIDI library (trusted, only test our wrapper)
- External hardware behavior (use fixtures)
