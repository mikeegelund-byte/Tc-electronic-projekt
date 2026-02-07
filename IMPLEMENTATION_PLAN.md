# Nova System Manager - MVP Completion Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Complete MVP by wiring existing backend use cases to UI and adding missing upload functionality

**Architecture:** Use existing Clean Architecture - wire ViewModels → UseCases → Domain. All backend exists, focus on UI integration.

**Tech Stack:** C# .NET 8, Avalonia 11.x, MVVM pattern, FluentResults

**Current Status:** 85% MVP complete - Connect, Download Bank works. Missing: Upload Bank, File I/O wiring, Preset editing integration.

---

## Critical Files Reference

**Use Cases (ALL EXIST):**
- `src/Nova.Application/UseCases/SavePresetUseCase.cs` - Upload single preset to slot
- `src/Nova.Application/UseCases/SaveBankUseCase.cs` - Save bank to .syx file
- `src/Nova.Application/UseCases/LoadBankUseCase.cs` - Load bank from .syx file
- `src/Nova.Application/UseCases/ExportSyxPresetUseCase.cs` - Export preset to .syx
- `src/Nova.Application/UseCases/ImportPresetUseCase.cs` - Import preset from .txt

**ViewModels TO WIRE:**
- `src/Nova.Presentation/ViewModels/FileManagerViewModel.cs` - All commands stubbed
- `src/Nova.Presentation/ViewModels/PresetDetailViewModel.cs` - Missing Save button
- `src/Nova.Presentation/ViewModels/MainViewModel.cs` - Needs SendBankToHardware

**Views (UI EXISTS):**
- `src/Nova.Presentation/Views/FileManagerView.axaml` - 4 buttons ready
- `src/Nova.Presentation/Views/PresetDetailView.axaml` - Missing Save/Upload controls
- `src/Nova.Presentation/MainWindow.axaml` - All tabs integrated

---

## Task 1: Wire FileManagerViewModel Save/Load Bank Commands

**Goal:** Enable file I/O for bank backup/restore

**Files:**
- Modify: `src/Nova.Presentation/ViewModels/FileManagerViewModel.cs`
- Test: Manual test with actual .syx file

**Step 1: Inject use cases into FileManagerViewModel constructor**

Add constructor parameters:
```csharp
private readonly ISaveBankUseCase _saveBankUseCase;
private readonly ILoadBankUseCase _loadBankUseCase;
private readonly IExportSyxPresetUseCase _exportPresetUseCase;
private readonly IImportPresetUseCase _importPresetUseCase;

public FileManagerViewModel(
    ISaveBankUseCase saveBankUseCase,
    ILoadBankUseCase loadBankUseCase,
    IExportSyxPresetUseCase exportPresetUseCase,
    IImportPresetUseCase importPresetUseCase)
{
    _saveBankUseCase = saveBankUseCase;
    _loadBankUseCase = loadBankUseCase;
    _exportPresetUseCase = exportPresetUseCase;
    _importPresetUseCase = importPresetUseCase;
}
```

**Step 2: Implement SaveBankAsync command**

Replace stub at line 18-22:
```csharp
[RelayCommand]
private async Task SaveBankAsync()
{
    if (_currentBank == null)
    {
        StatusMessage = "No bank loaded. Download bank first.";
        return;
    }

    // Open file dialog
    var saveFileDialog = new SaveFileDialog
    {
        Title = "Save Bank",
        Filters = new List<FileDialogFilter>
        {
            new FileDialogFilter { Name = "SysEx Files", Extensions = { "syx" } }
        },
        DefaultExtension = "syx",
        InitialFileName = $"NovaBank_{DateTime.Now:yyyyMMdd_HHmmss}.syx"
    };

    var result = await saveFileDialog.ShowAsync(/* Get window from DI */);
    if (string.IsNullOrEmpty(result)) return;

    CurrentFilePath = result;
    StatusMessage = "Saving bank...";

    var saveResult = await _saveBankUseCase.ExecuteAsync(_currentBank, result);

    if (saveResult.IsSuccess)
    {
        StatusMessage = $"Bank saved to {Path.GetFileName(result)}";
    }
    else
    {
        StatusMessage = $"Save failed: {saveResult.Errors.First().Message}";
    }
}
```

**Step 3: Implement LoadBankAsync command**

Replace stub at line 24-29:
```csharp
[RelayCommand]
private async Task LoadBankAsync()
{
    var openFileDialog = new OpenFileDialog
    {
        Title = "Load Bank",
        Filters = new List<FileDialogFilter>
        {
            new FileDialogFilter { Name = "SysEx Files", Extensions = { "syx" } }
        },
        AllowMultiple = false
    };

    var result = await openFileDialog.ShowAsync(/* Get window from DI */);
    if (result == null || result.Length == 0) return;

    var filePath = result[0];
    CurrentFilePath = filePath;
    StatusMessage = "Loading bank...";

    var progress = new Progress<int>(p => StatusMessage = $"Loading preset {p}/60...");
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));

    var loadResult = await _loadBankUseCase.ExecuteAsync(filePath, progress, cts.Token);

    if (loadResult.IsSuccess)
    {
        StatusMessage = $"Bank loaded from {Path.GetFileName(filePath)} - 60 presets sent to pedal";
        _currentBank = /* Need to store loaded bank */;
    }
    else
    {
        StatusMessage = $"Load failed: {loadResult.Errors.First().Message}";
    }
}
```

**Step 4: Add _currentBank field and LoadFromBank method**

Add at top of class:
```csharp
private UserBankDump? _currentBank;

public void LoadFromBank(UserBankDump bank)
{
    _currentBank = bank;
}
```

**Step 5: Wire FileManager to MainViewModel**

In `MainViewModel.cs` after line 188 (DownloadBankAsync success):
```csharp
// Load bank into FileManager too
FileManager.LoadFromBank(bank);
```

**Step 6: Test manually**

Run commands:
```bash
dotnet build src/Nova.Presentation
dotnet run --project src/Nova.Presentation
```

Test flow:
1. Connect to MIDI
2. Download Bank
3. File Manager tab → Save Bank → Select location → Verify .syx file created (31,260 bytes)
4. File Manager tab → Load Bank → Select .syx file → Verify presets sent to pedal

**Step 7: Commit**

```bash
git add src/Nova.Presentation/ViewModels/FileManagerViewModel.cs
git add src/Nova.Presentation/ViewModels/MainViewModel.cs
git commit -m "feat(ui): wire FileManager Save/Load Bank to use cases

- Inject SaveBankUseCase and LoadBankUseCase into FileManagerViewModel
- Implement SaveBankAsync with file dialog
- Implement LoadBankAsync with progress reporting
- Connect FileManager to MainViewModel bank state
- Add manual test verification"
```

---

## Task 2: Add "Upload to Slot" Button in PresetDetailView

**Goal:** Enable users to save modified presets back to pedal

**Files:**
- Modify: `src/Nova.Presentation/ViewModels/PresetDetailViewModel.cs`
- Modify: `src/Nova.Presentation/Views/PresetDetailView.axaml`
- Modify: `src/Nova.Presentation/ViewModels/MainViewModel.cs`

**Step 1: Add SavePresetUseCase to PresetDetailViewModel**

Add constructor injection:
```csharp
private readonly ISavePresetUseCase _savePresetUseCase;
private Preset? _currentPreset;

public PresetDetailViewModel(ISavePresetUseCase savePresetUseCase)
{
    _savePresetUseCase = savePresetUseCase;
}
```

Update `LoadFromPreset()` to store preset:
```csharp
public void LoadFromPreset(Preset? preset)
{
    _currentPreset = preset;
    // ... existing code ...
}
```

**Step 2: Add UploadToSlot command**

Add at end of PresetDetailViewModel:
```csharp
[ObservableProperty]
private int _targetSlot = 1;

[RelayCommand(CanExecute = nameof(CanUploadPreset))]
private async Task UploadPresetAsync()
{
    if (_currentPreset == null) return;

    StatusMessage = $"Uploading preset to slot {TargetSlot}...";

    var result = await _savePresetUseCase.ExecuteAsync(_currentPreset, TargetSlot, verify: true);

    if (result.IsSuccess)
    {
        StatusMessage = $"Preset uploaded to slot {TargetSlot} successfully";
    }
    else
    {
        StatusMessage = $"Upload failed: {result.Errors.First().Message}";
    }
}

private bool CanUploadPreset() => _currentPreset != null;

[ObservableProperty]
private string _statusMessage = "";
```

**Step 3: Add UI controls to PresetDetailView.axaml**

After line 20 (Global Parameters section), add:
```xaml
<!-- Upload Section -->
<Border Background="{StaticResource BackgroundSecondary}"
        Padding="{StaticResource CardPadding}"
        CornerRadius="{StaticResource BorderRadiusLarge}"
        Margin="0,0,0,16">
    <StackPanel Spacing="8">
        <TextBlock Text="Upload Preset"
                   FontWeight="{StaticResource FontWeightBold}"
                   Foreground="{StaticResource TextPrimary}"/>

        <Grid ColumnDefinitions="Auto,*,Auto">
            <TextBlock Grid.Column="0"
                       Text="Target Slot:"
                       VerticalAlignment="Center"
                       Margin="0,0,8,0"/>

            <NumericUpDown Grid.Column="1"
                           Value="{Binding TargetSlot}"
                           Minimum="1"
                           Maximum="60"
                           Increment="1"
                           HorizontalAlignment="Stretch"/>

            <Button Grid.Column="2"
                    Content="Upload to Pedal"
                    Command="{Binding UploadPresetCommand}"
                    Margin="8,0,0,0"
                    Padding="{StaticResource ButtonPadding}"/>
        </Grid>

        <TextBlock Text="{Binding StatusMessage}"
                   Foreground="{StaticResource TextSecondary}"
                   TextWrapping="Wrap"
                   IsVisible="{Binding StatusMessage, Converter={x:Static StringConverters.IsNotNullOrEmpty}}"/>
    </StackPanel>
</Border>
```

**Step 4: Register SavePresetUseCase in DI**

Verify in `src/Nova.Presentation/App.axaml.cs`:
```csharp
services.AddTransient<ISavePresetUseCase, SavePresetUseCase>();
services.AddTransient<PresetDetailViewModel>();
```

**Step 5: Test manually**

```bash
dotnet build
dotnet run --project src/Nova.Presentation
```

Test:
1. Connect → Download Bank
2. Click preset in list
3. Change "Target Slot" to different number (e.g., 5)
4. Click "Upload to Pedal"
5. Verify status message shows success
6. Download Bank again and verify preset is in new slot

**Step 6: Commit**

```bash
git add src/Nova.Presentation/ViewModels/PresetDetailViewModel.cs
git add src/Nova.Presentation/Views/PresetDetailView.axaml
git commit -m "feat(ui): add Upload to Slot in PresetDetailView

- Inject SavePresetUseCase into PresetDetailViewModel
- Add UploadPresetCommand with target slot selection
- Add Upload section UI with NumericUpDown + Button
- Add status message display
- Test: Upload preset to different slot works"
```

---

## Task 3: Create SendBankToHardwareUseCase

**Goal:** Send entire 60-preset bank to pedal in one operation

**Files:**
- Create: `src/Nova.Application/UseCases/ISendBankToHardwareUseCase.cs`
- Create: `src/Nova.Application/UseCases/SendBankToHardwareUseCase.cs`
- Test: `src/Nova.Application.Tests/UseCases/SendBankToHardwareUseCaseTests.cs`

**Step 1: Write failing test**

Create test file:
```csharp
using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class SendBankToHardwareUseCaseTests
{
    private readonly Mock<IMidiPort> _mockMidiPort;
    private readonly SendBankToHardwareUseCase _useCase;

    public SendBankToHardwareUseCaseTests()
    {
        _mockMidiPort = new Mock<IMidiPort>();
        _useCase = new SendBankToHardwareUseCase(_mockMidiPort.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidBank_SendsAll60Presets()
    {
        // Arrange
        var bank = CreateValidBank();
        _mockMidiPort.Setup(m => m.IsConnected).Returns(true);

        // Act
        var result = await _useCase.ExecuteAsync(bank, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockMidiPort.Verify(m => m.SendSysExAsync(It.IsAny<byte[]>()), Times.Exactly(60));
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotConnected_ReturnsError()
    {
        // Arrange
        var bank = CreateValidBank();
        _mockMidiPort.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await _useCase.ExecuteAsync(bank, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("not connected");
    }

    private UserBankDump CreateValidBank()
    {
        var presets = new Preset[60];
        for (int i = 0; i < 60; i++)
        {
            presets[i] = TestHelpers.CreateValidPreset(i + 1, $"Preset {i + 1}");
        }
        return new UserBankDump(presets);
    }
}
```

**Step 2: Run test to verify it fails**

```bash
dotnet test src/Nova.Application.Tests/ --filter "FullyQualifiedName~SendBankToHardwareUseCaseTests"
```

Expected: FAIL with "SendBankToHardwareUseCase not found"

**Step 3: Create interface**

```csharp
using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface ISendBankToHardwareUseCase
{
    Task<Result> ExecuteAsync(UserBankDump bank, CancellationToken cancellationToken);
}
```

**Step 4: Write minimal implementation**

```csharp
using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

public class SendBankToHardwareUseCase : ISendBankToHardwareUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly ILogger _logger = Log.ForContext<SendBankToHardwareUseCase>();

    public SendBankToHardwareUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort;
    }

    public async Task<Result> ExecuteAsync(UserBankDump bank, CancellationToken cancellationToken)
    {
        if (!_midiPort.IsConnected)
        {
            return Result.Fail("MIDI port not connected");
        }

        if (bank.Presets.Length != 60)
        {
            return Result.Fail($"Invalid bank size: {bank.Presets.Length} (expected 60)");
        }

        _logger.Information("Sending {Count} presets to hardware", bank.Presets.Length);

        for (int i = 0; i < bank.Presets.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Result.Fail("Upload cancelled by user");
            }

            var preset = bank.Presets[i];
            if (preset == null)
            {
                _logger.Warning("Skipping null preset at index {Index}", i);
                continue;
            }

            try
            {
                await _midiPort.SendSysExAsync(preset.RawSysEx);
                _logger.Debug("Sent preset {Number}: {Name}", preset.Number, preset.Name);

                // Delay between presets per MIDI protocol (50-100ms)
                await Task.Delay(75, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to send preset {Number}", preset.Number);
                return Result.Fail($"Failed at preset {preset.Number}: {ex.Message}");
            }
        }

        _logger.Information("Successfully sent all {Count} presets", bank.Presets.Length);
        return Result.Ok();
    }
}
```

**Step 5: Run test to verify it passes**

```bash
dotnet test src/Nova.Application.Tests/ --filter "FullyQualifiedName~SendBankToHardwareUseCaseTests" -v n
```

Expected: PASS (2/2 tests)

**Step 6: Register in DI**

In `App.axaml.cs`:
```csharp
services.AddTransient<ISendBankToHardwareUseCase, SendBankToHardwareUseCase>();
```

**Step 7: Commit**

```bash
git add src/Nova.Application/UseCases/ISendBankToHardwareUseCase.cs
git add src/Nova.Application/UseCases/SendBankToHardwareUseCase.cs
git add src/Nova.Application.Tests/UseCases/SendBankToHardwareUseCaseTests.cs
git commit -m "feat(app): add SendBankToHardwareUseCase

- Create ISendBankToHardwareUseCase interface
- Implement with 60 preset loop + 75ms delays
- Handle cancellation and errors
- Add unit tests (2 tests passing)
- Register in DI container"
```

---

## Task 4: Wire "Upload Bank" Button in FileManagerView

**Goal:** Add button to send entire bank to pedal

**Files:**
- Modify: `src/Nova.Presentation/ViewModels/FileManagerViewModel.cs`
- Modify: `src/Nova.Presentation/Views/FileManagerView.axaml`

**Step 1: Inject SendBankToHardwareUseCase**

Update constructor:
```csharp
private readonly ISendBankToHardwareUseCase _sendBankUseCase;

public FileManagerViewModel(
    ISaveBankUseCase saveBankUseCase,
    ILoadBankUseCase loadBankUseCase,
    IExportSyxPresetUseCase exportPresetUseCase,
    IImportPresetUseCase importPresetUseCase,
    ISendBankToHardwareUseCase sendBankUseCase)
{
    // ... existing code ...
    _sendBankUseCase = sendBankUseCase;
}
```

**Step 2: Add UploadBankToHardware command**

```csharp
[RelayCommand(CanExecute = nameof(CanUploadBank))]
private async Task UploadBankToHardwareAsync()
{
    if (_currentBank == null)
    {
        StatusMessage = "No bank loaded. Download or load bank first.";
        return;
    }

    StatusMessage = "Uploading 60 presets to pedal...";
    var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

    var result = await _sendBankUseCase.ExecuteAsync(_currentBank, cts.Token);

    if (result.IsSuccess)
    {
        StatusMessage = "All 60 presets uploaded successfully!";
    }
    else
    {
        StatusMessage = $"Upload failed: {result.Errors.First().Message}";
    }
}

private bool CanUploadBank() => _currentBank != null;
```

**Step 3: Add button to FileManagerView.axaml**

After line 40 (after Load Bank button):
```xaml
<!-- Upload Bank to Hardware -->
<Button Content="Upload Bank to Pedal"
        Command="{Binding UploadBankToHardwareCommand}"
        HorizontalAlignment="Left"
        Padding="{StaticResource ButtonPadding}"
        ToolTip.Tip="Send all 60 presets to Nova System hardware"
        Margin="0,8,0,0" />
```

**Step 4: Test manually**

```bash
dotnet build && dotnet run --project src/Nova.Presentation
```

Test:
1. Connect → Download Bank (or Load Bank from file)
2. File Manager tab → Click "Upload Bank to Pedal"
3. Wait ~5 seconds (60 presets × 75ms)
4. Verify status: "All 60 presets uploaded successfully!"
5. Download Bank again to verify changes

**Step 5: Commit**

```bash
git add src/Nova.Presentation/ViewModels/FileManagerViewModel.cs
git add src/Nova.Presentation/Views/FileManagerView.axaml
git commit -m "feat(ui): add Upload Bank to Pedal button

- Inject ISendBankToHardwareUseCase into FileManagerViewModel
- Add UploadBankToHardwareCommand with 2-minute timeout
- Add UI button in File Manager tab
- Enable after Download/Load Bank
- Test: Upload 60 presets works end-to-end"
```

---

## Task 5: Implement Export/Import Preset in FileManagerViewModel

**Goal:** Complete remaining File Manager functions

**Files:**
- Modify: `src/Nova.Presentation/ViewModels/FileManagerViewModel.cs`

**Step 1: Add current preset tracking**

Add field:
```csharp
private Preset? _selectedPreset;

public void SetSelectedPreset(Preset? preset)
{
    _selectedPreset = preset;
}
```

**Step 2: Implement ExportPresetAsync**

Replace stub:
```csharp
[RelayCommand(CanExecute = nameof(CanExportPreset))]
private async Task ExportPresetAsync()
{
    if (_selectedPreset == null)
    {
        StatusMessage = "No preset selected";
        return;
    }

    var saveFileDialog = new SaveFileDialog
    {
        Title = "Export Preset",
        Filters = new List<FileDialogFilter>
        {
            new FileDialogFilter { Name = "SysEx Files", Extensions = { "syx" } }
        },
        DefaultExtension = "syx",
        InitialFileName = $"{_selectedPreset.Name}.syx"
    };

    var result = await saveFileDialog.ShowAsync(/* window */);
    if (string.IsNullOrEmpty(result)) return;

    StatusMessage = "Exporting preset...";
    var exportResult = await _exportPresetUseCase.ExecuteAsync(_selectedPreset, result);

    StatusMessage = exportResult.IsSuccess
        ? $"Preset exported to {Path.GetFileName(result)}"
        : $"Export failed: {exportResult.Errors.First().Message}";
}

private bool CanExportPreset() => _selectedPreset != null;
```

**Step 3: Implement ImportPresetAsync**

Replace stub:
```csharp
[RelayCommand]
private async Task ImportPresetAsync()
{
    var openFileDialog = new OpenFileDialog
    {
        Title = "Import Preset",
        Filters = new List<FileDialogFilter>
        {
            new FileDialogFilter { Name = "Text Files", Extensions = { "txt" } },
            new FileDialogFilter { Name = "SysEx Files", Extensions = { "syx" } }
        },
        AllowMultiple = false
    };

    var result = await openFileDialog.ShowAsync(/* window */);
    if (result == null || result.Length == 0) return;

    var filePath = result[0];
    StatusMessage = "Importing preset...";

    var importResult = await _importPresetUseCase.ExecuteAsync(filePath);

    if (importResult.IsSuccess)
    {
        StatusMessage = $"Preset imported: {importResult.Value.Name}";
        _selectedPreset = importResult.Value;
    }
    else
    {
        StatusMessage = $"Import failed: {importResult.Errors.First().Message}";
    }
}
```

**Step 4: Wire to PresetListView selection**

In MainViewModel, update OnPresetSelectionChanged:
```csharp
private void OnPresetSelectionChanged()
{
    var selectedPreset = PresetList.SelectedPreset;

    if (selectedPreset != null && _currentBank != null)
    {
        var fullPreset = _currentBank.Presets.FirstOrDefault(p => p?.Number == selectedPreset.Number);

        if (fullPreset != null)
        {
            PresetDetail.LoadFromPreset(fullPreset);
            FileManager.SetSelectedPreset(fullPreset); // NEW
        }
    }
    else
    {
        PresetDetail.LoadFromPreset(null);
        FileManager.SetSelectedPreset(null); // NEW
    }
}
```

**Step 5: Test manually**

```bash
dotnet build && dotnet run --project src/Nova.Presentation
```

Test Export:
1. Connect → Download Bank → Select preset
2. File Manager → Export Preset → Save as "Test.syx"
3. Verify file created (521 bytes)

Test Import:
1. File Manager → Import Preset → Select "Test.syx"
2. Verify status: "Preset imported: [Name]"

**Step 6: Commit**

```bash
git add src/Nova.Presentation/ViewModels/FileManagerViewModel.cs
git add src/Nova.Presentation/ViewModels/MainViewModel.cs
git commit -m "feat(ui): implement Export/Import Preset

- Add SetSelectedPreset to FileManagerViewModel
- Implement ExportPresetAsync with file dialog
- Implement ImportPresetAsync for .txt and .syx files
- Wire preset selection from PresetListView
- Test: Export/Import single preset works"
```

---

## Task 6: Add Missing EqGate and Pitch Blocks to PresetDetailView

**Goal:** Complete preset editor UI with all 7 effect blocks

**Files:**
- Modify: `src/Nova.Presentation/Views/PresetDetailView.axaml`

**Step 1: Add EqGate Block UI**

After Compressor block section, add:
```xaml
<!-- EQ/Gate Block -->
<Border Background="{StaticResource BackgroundSecondary}"
        Padding="{StaticResource CardPadding}"
        CornerRadius="{StaticResource BorderRadiusLarge}">
    <StackPanel Spacing="8">
        <Grid ColumnDefinitions="Auto,*">
            <Ellipse Grid.Column="0"
                     Width="12" Height="12"
                     Fill="{Binding EqGateBlock.IsEnabled, Converter={StaticResource BoolToColorConverter}}"
                     VerticalAlignment="Center"
                     Margin="0,0,8,0"/>
            <TextBlock Grid.Column="1"
                       Text="EQ/Gate"
                       FontWeight="{StaticResource FontWeightBold}"
                       Foreground="{StaticResource TextPrimary}"/>
        </Grid>

        <Grid ColumnDefinitions="Auto,*,Auto" RowDefinitions="Auto,Auto,Auto">
            <TextBlock Grid.Row="0" Grid.Column="0" Text="Type:" VerticalAlignment="Center"/>
            <TextBlock Grid.Row="0" Grid.Column="1"
                       Text="{Binding EqGateBlock.Type}"
                       Foreground="{StaticResource TextSecondary}"
                       Margin="8,0"/>

            <TextBlock Grid.Row="1" Grid.Column="0" Text="Gate Threshold:" VerticalAlignment="Center"/>
            <NumericUpDown Grid.Row="1" Grid.Column="1"
                           Value="{Binding EqGateBlock.GateThreshold}"
                           Minimum="-90" Maximum="0"
                           Increment="1"
                           Margin="8,0"/>
            <TextBlock Grid.Row="1" Grid.Column="2" Text="dB" VerticalAlignment="Center"/>

            <TextBlock Grid.Row="2" Grid.Column="0" Text="EQ Gain:" VerticalAlignment="Center"/>
            <NumericUpDown Grid.Row="2" Grid.Column="1"
                           Value="{Binding EqGateBlock.EqGain}"
                           Minimum="-12" Maximum="12"
                           Increment="1"
                           Margin="8,0"/>
            <TextBlock Grid.Row="2" Grid.Column="2" Text="dB" VerticalAlignment="Center"/>
        </Grid>
    </StackPanel>
</Border>
```

**Step 2: Add Pitch Block UI**

After Modulation block section, add:
```xaml
<!-- Pitch Block -->
<Border Background="{StaticResource BackgroundSecondary}"
        Padding="{StaticResource CardPadding}"
        CornerRadius="{StaticResource BorderRadiusLarge}">
    <StackPanel Spacing="8">
        <Grid ColumnDefinitions="Auto,*">
            <Ellipse Grid.Column="0"
                     Width="12" Height="12"
                     Fill="{Binding PitchBlock.IsEnabled, Converter={StaticResource BoolToColorConverter}}"
                     VerticalAlignment="Center"
                     Margin="0,0,8,0"/>
            <TextBlock Grid.Column="1"
                       Text="Pitch"
                       FontWeight="{StaticResource FontWeightBold}"
                       Foreground="{StaticResource TextPrimary}"/>
        </Grid>

        <Grid ColumnDefinitions="Auto,*,Auto" RowDefinitions="Auto,Auto,Auto">
            <TextBlock Grid.Row="0" Grid.Column="0" Text="Type:" VerticalAlignment="Center"/>
            <TextBlock Grid.Row="0" Grid.Column="1"
                       Text="{Binding PitchBlock.Type}"
                       Foreground="{StaticResource TextSecondary}"
                       Margin="8,0"/>

            <TextBlock Grid.Row="1" Grid.Column="0" Text="Pitch Shift:" VerticalAlignment="Center"/>
            <NumericUpDown Grid.Row="1" Grid.Column="1"
                           Value="{Binding PitchBlock.PitchShift}"
                           Minimum="-24" Maximum="24"
                           Increment="1"
                           Margin="8,0"/>
            <TextBlock Grid.Row="1" Grid.Column="2" Text="semitones" VerticalAlignment="Center"/>

            <TextBlock Grid.Row="2" Grid.Column="0" Text="Mix:" VerticalAlignment="Center"/>
            <NumericUpDown Grid.Row="2" Grid.Column="1"
                           Value="{Binding PitchBlock.Mix}"
                           Minimum="0" Maximum="100"
                           Increment="1"
                           Margin="8,0"/>
            <TextBlock Grid.Row="2" Grid.Column="2" Text="%" VerticalAlignment="Center"/>
        </Grid>
    </StackPanel>
</Border>
```

**Step 3: Verify ViewModels exist**

Check that these ViewModels have required properties:
- `src/Nova.Presentation/ViewModels/Effects/EqGateBlockViewModel.cs`
- `src/Nova.Presentation/ViewModels/Effects/PitchBlockViewModel.cs`

**Step 4: Test manually**

```bash
dotnet build && dotnet run --project src/Nova.Presentation
```

Test:
1. Connect → Download Bank → Select preset
2. Scroll in PresetDetailView
3. Verify all 7 blocks visible: Compressor, Drive, EqGate, Modulation, Pitch, Delay, Reverb
4. Verify enabled indicators (green/gray circles)

**Step 5: Commit**

```bash
git add src/Nova.Presentation/Views/PresetDetailView.axaml
git commit -m "feat(ui): add missing EqGate and Pitch blocks

- Add EqGate block UI with GateThreshold and EqGain
- Add Pitch block UI with PitchShift and Mix
- Complete all 7 effect blocks in preset editor
- ViewModels already existed, just wired to UI
- Test: All 7 blocks visible when preset selected"
```

---

## Task 7: Make System Settings Editable

**Goal:** Enable editing and saving of system configuration

**Files:**
- Modify: `src/Nova.Presentation/ViewModels/SystemSettingsViewModel.cs`
- Modify: `src/Nova.Presentation/Views/SystemSettingsView.axaml`

**Step 1: Change properties to editable**

Replace read-only properties with editable ones:
```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
private int _midiChannel;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
private int _deviceId;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
private bool _midiClockEnabled;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
private bool _midiProgramChangeEnabled;

public bool HasUnsavedChanges => _originalDump != null && HasChangesFromOriginal();

private SystemDump? _originalDump;

private bool HasChangesFromOriginal()
{
    if (_originalDump == null) return false;

    return MidiChannel != _originalDump.RawSysEx[8]
        || DeviceId != _originalDump.RawSysEx[4]
        || MidiClockEnabled != (_originalDump.RawSysEx[20] == 0x01)
        || MidiProgramChangeEnabled != (_originalDump.RawSysEx[21] == 0x01);
}
```

**Step 2: Update LoadFromDump to store original**

```csharp
public void LoadFromDump(SystemDump dump)
{
    _originalDump = dump;

    MidiChannel = dump.RawSysEx[8];
    DeviceId = dump.RawSysEx[4];
    MidiClockEnabled = dump.RawSysEx[20] == 0x01;
    MidiProgramChangeEnabled = dump.RawSysEx[21] == 0x01;
    Version = $"{dump.RawSysEx[22]}.{dump.RawSysEx[23]}";
}

public void RevertChanges()
{
    if (_originalDump != null)
    {
        LoadFromDump(_originalDump);
    }
}
```

**Step 3: Update SystemSettingsView.axaml to use inputs**

Replace TextBlocks with editable controls:
```xaml
<!-- MIDI Channel -->
<TextBlock Grid.Row="0" Grid.Column="0" Text="MIDI Channel:" VerticalAlignment="Center"/>
<NumericUpDown Grid.Row="0" Grid.Column="1"
               Value="{Binding MidiChannel}"
               Minimum="0" Maximum="15"
               Increment="1"/>

<!-- Device ID -->
<TextBlock Grid.Row="1" Grid.Column="0" Text="Device ID:" VerticalAlignment="Center"/>
<NumericUpDown Grid.Row="1" Grid.Column="1"
               Value="{Binding DeviceId}"
               Minimum="0" Maximum="127"
               Increment="1"/>

<!-- MIDI Clock -->
<TextBlock Grid.Row="2" Grid.Column="0" Text="MIDI Clock:" VerticalAlignment="Center"/>
<CheckBox Grid.Row="2" Grid.Column="1"
          IsChecked="{Binding MidiClockEnabled}"
          Content="Enabled"/>

<!-- Program Change -->
<TextBlock Grid.Row="3" Grid.Column="0" Text="Program Change:" VerticalAlignment="Center"/>
<CheckBox Grid.Row="3" Grid.Column="1"
          IsChecked="{Binding MidiProgramChangeEnabled}"
          Content="Enabled"/>

<!-- Version (Read-Only) -->
<TextBlock Grid.Row="4" Grid.Column="0" Text="Firmware Version:" VerticalAlignment="Center"/>
<TextBlock Grid.Row="4" Grid.Column="1"
           Text="{Binding Version}"
           Foreground="{StaticResource TextSecondary}"/>
```

**Step 4: Update MainViewModel SaveSystemSettings**

Update CanSaveSystemSettings:
```csharp
private bool CanSaveSystemSettings() => !IsDownloading && SystemSettings.HasUnsavedChanges;
```

**Step 5: Implement CancelSystemChanges in MainViewModel**

Replace stub:
```csharp
[RelayCommand]
private void CancelSystemChanges()
{
    SystemSettings.RevertChanges();
    StatusMessage = "System settings changes cancelled";
}
```

**Step 6: Test manually**

```bash
dotnet build && dotnet run --project src/Nova.Presentation
```

Test:
1. Connect → Download System Settings (TODO: implement this first OR manually request dump)
2. System Settings tab
3. Change MIDI Channel to 5
4. Toggle MIDI Clock
5. Click "Save to Pedal" → Verify success
6. Click "Cancel" → Verify reverted

**Step 7: Commit**

```bash
git add src/Nova.Presentation/ViewModels/SystemSettingsViewModel.cs
git add src/Nova.Presentation/Views/SystemSettingsView.axaml
git add src/Nova.Presentation/ViewModels/MainViewModel.cs
git commit -m "feat(ui): make System Settings editable

- Change SystemSettingsViewModel properties to editable
- Add HasUnsavedChanges tracking
- Replace TextBlocks with NumericUpDown and CheckBox
- Implement RevertChanges method
- Update CanSaveSystemSettings condition
- Implement CancelSystemChanges command
- Test: Edit and save system settings works"
```

---

## Verification Section

### End-to-End Test Scenarios

**Scenario 1: MVP Bank Roundtrip**
```
1. Connect to MIDI device
2. Download Bank (60 presets)
3. File Manager → Save Bank to file
4. Modify preset in PresetDetailView
5. Upload modified preset to slot
6. File Manager → Upload Bank to Pedal
7. Download Bank again
8. Verify changes persisted

Expected: All presets saved and restored correctly
```

**Scenario 2: File I/O Operations**
```
1. Download Bank
2. File Manager → Save Bank ("TestBank.syx")
3. Disconnect
4. File Manager → Load Bank from "TestBank.syx"
5. Verify all 60 presets sent (status messages)

Expected: Bank loaded from file and sent to pedal
```

**Scenario 3: Preset Export/Import**
```
1. Download Bank → Select preset #35
2. File Manager → Export Preset → "Preset35.syx"
3. File Manager → Import Preset → Select "Preset35.syx"
4. Verify preset name matches

Expected: Single preset exported and re-imported correctly
```

**Scenario 4: System Settings Edit**
```
1. Connect → Request System Dump (manual or auto)
2. System Settings tab
3. Change MIDI Channel to 3
4. Enable MIDI Clock
5. Save to Pedal
6. Request System Dump again
7. Verify settings persisted

Expected: System configuration saved successfully
```

### Automated Test Coverage

**Unit Tests to Run:**
```bash
# Run all tests
dotnet test NovaApp.sln

# Specific test suites
dotnet test src/Nova.Application.Tests/ --filter "FullyQualifiedName~SendBankToHardwareUseCaseTests"
dotnet test src/Nova.Application.Tests/ --filter "FullyQualifiedName~FileManager"
```

**Expected Results:**
- All 453 production tests passing (100%)
- 5 investigation tests skipped
- New SendBankToHardwareUseCase tests passing (2 tests)

### Manual Testing Checklist

- [ ] Connect to MIDI device works
- [ ] Download Bank receives 60 presets
- [ ] PresetListView displays all presets
- [ ] Click preset → PresetDetailView shows all 7 blocks
- [ ] Upload preset to different slot works
- [ ] Save Bank to file creates .syx file (31,260 bytes)
- [ ] Load Bank from file sends 60 presets
- [ ] Upload Bank to Pedal sends all 60 presets
- [ ] Export Preset creates .syx file (521 bytes)
- [ ] Import Preset loads .syx file
- [ ] System Settings displays correctly
- [ ] Edit System Settings and save works
- [ ] Cancel System Settings reverts changes

---

## Success Criteria

**MVP Complete When:**
1. ✅ Connect to MIDI device (already works)
2. ✅ Download Bank (60 presets) (already works)
3. ✅ **Upload Bank** (NEW - Task 3 + 4)
4. ✅ **Save/Load Bank to file** (NEW - Task 1)
5. ✅ **Edit and upload presets** (NEW - Task 2)
6. ✅ **All 7 effect blocks visible** (NEW - Task 6)
7. ✅ **System Settings editable** (NEW - Task 7)

**Technical Criteria:**
- All 453 production tests passing (maintain 100%)
- No MIDI memory leaks (already fixed)
- Proper error messages for all operations
- File dialogs work correctly
- Status messages inform user of progress

**User Experience:**
- User can backup entire bank to .syx file
- User can restore bank from .syx file
- User can edit preset parameters and save to any slot
- User can see all effect blocks in preset editor
- User can configure system MIDI settings

---

## Notes

- **No Domain/Application changes needed** - All use cases exist!
- **Focus 100% on UI/ViewModel wiring** - Backend is solid
- **Test after each task** - Verify UI works end-to-end
- **Commit frequently** - Small, focused commits per feature
- **Use existing patterns** - Follow MainViewModel structure

**Estimated Time:** 4-6 hours (all tasks are straightforward UI wiring)

**Dependencies:** None - All use cases already implemented

**Risk:** Low - Just connecting existing pieces together
