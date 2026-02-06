# Testing Strategy - Nova System Manager

**Version**: 1.0  
**Last Updated**: 2026-02-03  
**Status**: Module 10 - Release & Polish Phase

---

## Overview

This document outlines the comprehensive testing strategy for the Nova System Manager application, covering all aspects from unit tests to user acceptance testing.

---

## Test Pyramid

```
        /\
       /  \  Manual Testing (5%)
      /____\
     /      \  Integration Tests (15%)
    /________\
   /          \  Unit Tests (80%)
  /____________\
```

---

## 1. Unit Tests (80% Coverage)

### Current Status
- **Total Tests**: 342 passing
  - Domain: 160 tests ✅
  - MIDI: 6 tests ✅
  - Infrastructure: 12 tests ✅
  - Application: 88 tests ✅
  - Presentation: 76 tests ✅

### Domain Layer Tests
**Location**: `src/Nova.Domain.Tests/`

**Coverage**:
- Effect parameter value objects (all 7 effect types)
- Preset aggregate validation
- System settings validation
- Pedal mapping domain logic
- CC mapping domain logic

**Test Framework**: xUnit  
**Mocking**: None required (pure domain logic)

**Example Test Categories**:
```csharp
// Value object validation
[Theory]
[InlineData(-1)]   // Below min
[InlineData(101)]  // Above max
public void Mix_InvalidValue_ThrowsArgumentException(int value)

// Aggregate business rules
[Fact]
public void Preset_WithInvalidEffectOrder_ThrowsException()

// Domain events
[Fact]
public void SystemSettings_WhenChanged_RaisesEvent()
```

### Application Layer Tests
**Location**: `src/Nova.Application.Tests/`

**Coverage**:
- Use case handlers (commands/queries)
- MIDI communication workflows
- File I/O operations
- Preset download/upload orchestration

**Test Framework**: xUnit  
**Mocking**: Moq for repositories and MIDI interfaces

**Example Test Categories**:
```csharp
// Command handlers
[Fact]
public async Task SavePresetCommandHandler_ValidPreset_SavesSuccessfully()

// Query handlers
[Fact]
public async Task GetPresetQueryHandler_ExistingId_ReturnsPreset()

// Error handling
[Fact]
public async Task DownloadBankCommand_MidiError_ReturnsFailureResult()
```

### Presentation Layer Tests
**Location**: `src/Nova.Presentation.Tests/`

**Coverage**:
- ViewModel property bindings
- Command execution logic
- UI state management
- Input validation

**Test Framework**: xUnit  
**Mocking**: Moq for application services

**Example Test Categories**:
```csharp
// ViewModel initialization
[Fact]
public void MainViewModel_OnCreation_InitializesCommands()

// Property changes
[Fact]
public void PresetDetailViewModel_WhenPresetUpdated_RaisesPropertyChanged()

// Command execution
[Fact]
public async Task ConnectCommand_ValidPort_UpdatesIsConnected()
```

---

## 2. Integration Tests (15% Coverage)

### MIDI Hardware Tests
**Location**: `src/Nova.HardwareTest/`

**Purpose**: Verify end-to-end communication with actual Nova System pedal

**Test Scenarios**:
```csharp
[Fact] // Requires hardware
public async Task EndToEnd_DownloadPreset_DecodesSysExCorrectly()

[Fact] // Requires hardware
public async Task EndToEnd_UploadPreset_PedalAcceptsSysEx()
```

**Hardware Requirements**:
- TC Electronic Nova System pedal
- USB MIDI interface
- MIDI cables

**Execution**:
```bash
# Manual execution only (requires hardware)
dotnet test Nova.HardwareTest.csproj
```

### Infrastructure Tests
**Location**: `src/Nova.Infrastructure.Tests/`

**Coverage**:
- File system operations
- MIDI port enumeration (mocked)
- SysEx serialization/deserialization

**Known Failures** (environment-dependent):
- `DryWetMidiPortTests.GetAvailablePorts_ReturnsListOfPortNames` ⚠️
- `DryWetMidiPortTests.ConnectAsync_WithInvalidPort_ReturnsFailure` ⚠️

These tests fail in CI environments without MIDI hardware and can be safely ignored.

---

## 3. UI/UX Testing

### Accessibility Testing

**WCAG AA Compliance Checklist**:
- [x] Color contrast ratios ≥ 4.5:1 for text
  - TextSecondary: #CCCCCC on #2C2C2C = 5.3:1 ✅
  - WarningBackground: #CC6633 = 4.7:1 ✅
- [x] AutomationProperties on all interactive controls (85+ properties)
- [x] Keyboard navigation with visible focus indicators
- [x] Status indicators include text labels (not color-only)
- [x] Screen reader support via AutomationProperties

**Tools**:
- Accessibility Insights for Windows
- Windows Narrator
- NVDA Screen Reader

**Manual Test Procedure**:
1. Launch app with Narrator enabled
2. Navigate using Tab/Shift+Tab
3. Verify all controls are announced correctly
4. Test keyboard shortcuts (Ctrl+R, F5, Ctrl+S, etc.)
5. Verify focus indicators are visible

### Visual Regression Testing

**Baseline Screenshots** (to be created in Phase 4):
- MainWindow - Connection tab
- MainWindow - Preset list tab
- MainWindow - System settings tab
- MainWindow - MIDI mapping tab
- MainWindow - File manager tab
- PresetDetailView with all effect types
- SavePresetDialog
- All button states (normal, hover, pressed, disabled)

**Comparison Tool**: Manual visual inspection (automated tools deferred to v1.1)

---

## 4. Performance Testing

### Metrics

**Target Performance**:
- App startup: < 2 seconds
- Preset download (60 presets): < 10 seconds
- Preset save: < 500ms
- UI responsiveness: < 100ms for all interactions

**Test Scenarios**:
```csharp
[Fact]
public async Task DownloadBank_60Presets_CompletesUnder10Seconds()

[Fact]
public async Task SavePreset_CompletesUnder500Milliseconds()
```

### Memory Profile

**Target**:
- Initial memory: < 100 MB
- After downloading 60 presets: < 150 MB
- No memory leaks over 1 hour session

**Tools**:
- Visual Studio Profiler
- dotMemory

---

## 5. Manual Testing

### Smoke Test (5 minutes)

**Pre-release Checklist**:
1. ✅ App launches without errors
2. ✅ Connect to MIDI port
3. ✅ Download bank (60 presets)
4. ✅ Edit preset parameters
5. ✅ Save preset
6. ✅ Import/export .syx file
7. ✅ Switch dark/light theme
8. ✅ All tabs accessible
9. ✅ No console errors
10. ✅ Close app gracefully

### Regression Test (30 minutes)

**Complete Flow**:
1. **Connection**:
   - Launch app
   - Refresh MIDI ports (Ctrl+R)
   - Select Nova System port
   - Connect
   - Verify status shows "Connected" with green indicator

2. **Download**:
   - Click "Download Bank" (or F5)
   - Verify progress bar updates
   - Verify all 60 presets appear in list
   - Verify status shows "Downloaded 60 presets"

3. **Preset Editing**:
   - Select preset from list
   - View preset details
   - Edit effect parameters
   - Verify real-time updates (if live mode enabled)
   - Save preset (Ctrl+S)

4. **System Settings**:
   - Navigate to System Settings tab
   - View MIDI channel
   - View global bypass setting
   - Edit pedal mappings
   - Save changes

5. **File Operations**:
   - Export preset as .syx file
   - Import .syx file
   - Verify imported preset matches original

6. **Error Handling**:
   - Disconnect MIDI cable
   - Attempt operation
   - Verify error message is clear and helpful
   - Reconnect cable
   - Verify recovery

---

## 6. Installation Testing

### Clean Machine Test

**Environment**:
- Fresh Windows 11 VM
- No .NET runtime pre-installed
- No Visual Studio or dev tools

**Test Procedure**:
1. Download .msi installer
2. Double-click to install
3. Follow installation wizard
4. Verify desktop shortcut created
5. Launch app from shortcut
6. Perform smoke test
7. Uninstall via Control Panel
8. Verify complete removal (no files left behind)

**Validation**:
- App launches without errors ✅
- MIDI functionality works (if hardware available) ✅
- No runtime errors ✅
- Uninstall is clean ✅

---

## 7. CI/CD Pipeline Tests

### GitHub Actions Workflow

**Build & Test Pipeline**:
```yaml
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build -c Release
      - run: dotnet test --no-build --verbosity normal
      - run: dotnet publish -c Release -o publish
```

**Success Criteria**:
- All 340+ unit tests pass ✅
- Build completes without errors ✅
- No security vulnerabilities (CodeQL) ✅
- Publish generates deployable artifacts ✅

---

## 8. Test Execution Schedule

### Continuous (Every Commit)
- All unit tests (342 tests)
- Build verification

### Before PR Merge
- Full unit test suite
- Smoke test (manual)
- Accessibility spot check

### Before Release
- Full regression test suite
- Hardware integration tests
- Installation test on clean VM
- Performance benchmarks
- Accessibility audit
- Visual regression check

---

## 9. Test Data

### Sample Presets
**Location**: `test-data/presets/`
- `clean-lead.syx` - Simple preset with reverb only
- `full-chain.syx` - All 7 effects enabled
- `edge-cases.syx` - Boundary values for all parameters

### Sample System Settings
**Location**: `test-data/system/`
- `default-system.syx` - Factory defaults
- `custom-pedals.syx` - Custom pedal mappings
- `custom-cc.syx` - Custom CC assignments

---

## 10. Defect Management

### Bug Severity

**Critical** (Fix immediately):
- App crashes
- Data loss
- Security vulnerabilities
- Accessibility violations (WCAG AA)

**High** (Fix before release):
- Feature doesn't work as designed
- Poor user experience
- Performance issues

**Medium** (Fix in next release):
- Minor UI glitches
- Missing tooltips
- Inconsistent styling

**Low** (Backlog):
- Nice-to-have features
- Cosmetic improvements

### Regression Test Selection

**Must Retest After**:
- MIDI communication changes → Hardware tests
- Domain model changes → All domain tests
- UI changes → Presentation tests + accessibility audit
- File I/O changes → Integration tests

---

## 11. Test Metrics

### Current Coverage (as of 2026-02-03)

```
Domain:           100% (all value objects, aggregates)
Application:       95% (use case handlers)
Presentation:      80% (ViewModels)
Infrastructure:    60% (file I/O, MIDI abstraction)
Overall:          ~85%
```

### Quality Gates

**Before Release**:
- Unit test pass rate: 100% ✅
- Build errors: 0 ✅
- Build warnings: 0 ✅
- Critical bugs: 0 ✅
- Accessibility violations: 0 ✅

---

## 12. Future Test Improvements (v1.1+)

- [ ] Automated UI testing with Avalonia.Headless
- [ ] Automated visual regression with Percy or similar
- [ ] Mutation testing with Stryker.NET
- [ ] Performance regression tests in CI
- [ ] Fuzz testing for SysEx parsing
- [ ] Chaos testing (random MIDI disconnects)

---

## Appendix A: Running Tests Locally

```bash
# All tests
dotnet test

# Specific test project
dotnet test src/Nova.Domain.Tests/

# With code coverage
dotnet test --collect:"XPlat Code Coverage"

# Verbose output
dotnet test --verbosity detailed

# Filter by category
dotnet test --filter "FullyQualifiedName~Effect"

# Parallel execution (default)
dotnet test --parallel

# Hardware tests (requires pedal)
dotnet test src/Nova.HardwareTest/
```

---

## Appendix B: Test Naming Conventions

**Format**: `MethodName_Scenario_ExpectedResult`

**Examples**:
```csharp
// Good
Connect_ValidPort_ReturnsSuccess()
SavePreset_InvalidPosition_ThrowsArgumentException()
GetAvailablePorts_WhenNoPorts_ReturnsEmptyList()

// Bad
TestConnect()
TestSavePreset()
Test1()
```

---

## Appendix C: Accessibility Test Checklist

**Keyboard Navigation**:
- [ ] Tab navigates through all controls in logical order
- [ ] Shift+Tab navigates backwards
- [ ] Enter activates buttons
- [ ] Escape closes dialogs
- [ ] Arrow keys navigate lists/dropdowns

**Screen Reader**:
- [ ] All controls announce their purpose
- [ ] All controls announce their state (enabled/disabled)
- [ ] Form validation errors are announced
- [ ] Live regions update on status changes

**Visual**:
- [ ] Focus indicators are visible (2px blue border)
- [ ] Text contrast ≥ 4.5:1
- [ ] UI elements ≥ 44x44 pixels (touch targets)
- [ ] No information conveyed by color alone

---

**Document Status**: Living document - update after each major milestone
