# Task List: Modul 1, Phase 5 — Presentation (UI)

## 📋 Module: 1 (MVP - Connection + Bank Dump)
## 📋 Phase: 5 (Presentation layer - Avalonia UI)
**Duration**: 3-4 days  
**Prerequisite**: Phase 4 complete (MIDI driver works)  
**Output**: Working UI with Connect button and Download button  

---

## Overview

**Goal**: Build the minimal UI that allows a user to connect to their Nova System and download the User Bank.

**KRITISK**: Uden UI er appen ubrugelig for slutbrugeren.

---

## Exit Criteria (Phase 5 Complete When ALL True)

- [ ] MainWindow shows port selection dropdown
- [ ] "Connect" button connects to selected port
- [ ] "Download Bank" button triggers bank download
- [ ] Status bar shows connection state and progress
- [ ] Error messages displayed to user
- [ ] DI container wires everything together
- [ ] Manual test: Full flow works on real pedal

---

## Task 5.1: Setup Dependency Injection Container

**🟡 COMPLEXITY: MEDIUM** — Kræver DI pattern forståelse

**Status**: Not started  
**Estimated**: 30 min  
**Files**:
- `src/Nova.Presentation/App.axaml.cs`
- `src/Nova.Presentation/Nova.Presentation.csproj`

### Steps
1. Add NuGet package:
```powershell
cd src/Nova.Presentation
dotnet add package Microsoft.Extensions.DependencyInjection
```

2. Configure services in App.axaml.cs:
```csharp
using Microsoft.Extensions.DependencyInjection;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        
        // Infrastructure
        services.AddSingleton<IMidiPort, DryWetMidiPort>();
        
        // Application
        services.AddTransient<ConnectUseCase>();
        services.AddTransient<DownloadBankUseCase>();
        
        // ViewModels
        services.AddTransient<MainViewModel>();
        
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
```

---

## Task 5.2: Create MainViewModel

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5 eller højere

**Årsag**: MVVM pattern, INotifyPropertyChanged, async commands, error handling

**Status**: Not started  
**Estimated**: 60 min  
**Files**:
- `src/Nova.Presentation/ViewModels/MainViewModel.cs`
- `src/Nova.Presentation.Tests/ViewModels/MainViewModelTests.cs`

### Test First (RED)
```csharp
public class MainViewModelTests
{
    [Fact]
    public void AvailablePorts_InitiallyEmpty()
    {
        var mockMidi = new Mock<IMidiPort>();
        var vm = new MainViewModel(mockMidi.Object, null!, null!);
        vm.AvailablePorts.Should().BeEmpty();
    }

    [Fact]
    public void IsConnected_WhenNotConnected_ReturnsFalse()
    {
        var mockMidi = new Mock<IMidiPort>();
        mockMidi.Setup(m => m.IsConnected).Returns(false);
        var vm = new MainViewModel(mockMidi.Object, null!, null!);
        vm.IsConnected.Should().BeFalse();
    }

    [Fact]
    public async Task ConnectCommand_CallsConnectUseCase()
    {
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<ConnectUseCase>(mockMidi.Object);
        mockConnect.Setup(c => c.ExecuteAsync(It.IsAny<string>()))
            .ReturnsAsync(Result.Ok());

        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, null!);
        vm.SelectedPort = "Test Port";
        
        await vm.ConnectCommand.ExecuteAsync(null);
        
        mockConnect.Verify(c => c.ExecuteAsync("Test Port"), Times.Once);
    }
}
```

### Code (GREEN)
```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Nova.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMidiPort _midiPort;
    private readonly ConnectUseCase _connectUseCase;
    private readonly DownloadBankUseCase _downloadBankUseCase;

    [ObservableProperty]
    private ObservableCollection<string> _availablePorts = new();

    [ObservableProperty]
    private string? _selectedPort;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private int _downloadProgress;

    public MainViewModel(
        IMidiPort midiPort,
        ConnectUseCase connectUseCase,
        DownloadBankUseCase downloadBankUseCase)
    {
        _midiPort = midiPort;
        _connectUseCase = connectUseCase;
        _downloadBankUseCase = downloadBankUseCase;
    }

    [RelayCommand]
    private void RefreshPorts()
    {
        AvailablePorts.Clear();
        foreach (var port in DryWetMidiPort.GetAvailablePorts())
        {
            AvailablePorts.Add(port);
        }
        StatusMessage = $"Found {AvailablePorts.Count} MIDI ports";
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        if (string.IsNullOrEmpty(SelectedPort)) return;

        StatusMessage = $"Connecting to {SelectedPort}...";
        var result = await _connectUseCase.ExecuteAsync(SelectedPort);

        if (result.IsSuccess)
        {
            IsConnected = true;
            StatusMessage = $"Connected to {SelectedPort}";
        }
        else
        {
            StatusMessage = $"Error: {result.Errors.First().Message}";
        }
    }

    private bool CanConnect() => !string.IsNullOrEmpty(SelectedPort) && !IsConnected;

    [RelayCommand(CanExecute = nameof(CanDownload))]
    private async Task DownloadBankAsync()
    {
        IsDownloading = true;
        StatusMessage = "Waiting for User Bank dump from pedal...";
        DownloadProgress = 0;

        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var result = await _downloadBankUseCase.ExecuteAsync(cts.Token);

            if (result.IsSuccess)
            {
                var bank = result.Value;
                StatusMessage = $"Downloaded {bank.Presets.Count} presets";
                DownloadProgress = 100;
                // TODO: Store bank and update preset list
            }
            else
            {
                StatusMessage = $"Error: {result.Errors.First().Message}";
            }
        }
        finally
        {
            IsDownloading = false;
        }
    }

    private bool CanDownload() => IsConnected && !IsDownloading;
}
```

---

## Task 5.3: Add CommunityToolkit.Mvvm NuGet Package

**🟢 COMPLEXITY: TRIVIAL** — Kan udføres af enhver model

**Status**: Not started  
**Estimated**: 5 min  
**Files**:
- `src/Nova.Presentation/Nova.Presentation.csproj`

### Steps
```powershell
cd src/Nova.Presentation
dotnet add package CommunityToolkit.Mvvm
```

---

## Task 5.4: Build MainWindow.axaml UI

**🟡 COMPLEXITY: MEDIUM** — Kræver Avalonia XAML kendskab

**Status**: Not started  
**Estimated**: 45 min  
**Files**:
- `src/Nova.Presentation/MainWindow.axaml`

### Code
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:Nova.Presentation.ViewModels"
        x:Class="Nova.Presentation.MainWindow"
        x:DataType="vm:MainViewModel"
        Title="Nova System Manager"
        Width="600" Height="400"
        WindowStartupLocation="CenterScreen">

    <Design.DataContext>
        <vm:MainViewModel />
    </Design.DataContext>

    <DockPanel Margin="16">
        <!-- Status Bar at Bottom -->
        <Border DockPanel.Dock="Bottom" 
                Background="#1E1E1E" 
                Padding="8" 
                CornerRadius="4"
                Margin="0,16,0,0">
            <Grid ColumnDefinitions="*,Auto">
                <TextBlock Text="{Binding StatusMessage}" 
                           Foreground="#CCCCCC"
                           VerticalAlignment="Center"/>
                <ProgressBar Grid.Column="1" 
                             Value="{Binding DownloadProgress}"
                             Width="100"
                             IsVisible="{Binding IsDownloading}"
                             Margin="16,0,0,0"/>
            </Grid>
        </Border>

        <!-- Main Content -->
        <StackPanel Spacing="16">
            <!-- Connection Section -->
            <Border Background="#2D2D2D" Padding="16" CornerRadius="8">
                <StackPanel Spacing="12">
                    <TextBlock Text="MIDI Connection" 
                               FontWeight="Bold" 
                               FontSize="16"/>
                    
                    <Grid ColumnDefinitions="*,Auto,Auto" RowDefinitions="Auto">
                        <ComboBox ItemsSource="{Binding AvailablePorts}"
                                  SelectedItem="{Binding SelectedPort}"
                                  PlaceholderText="Select MIDI Port..."
                                  HorizontalAlignment="Stretch"/>
                        
                        <Button Grid.Column="1" 
                                Content="🔄" 
                                Command="{Binding RefreshPortsCommand}"
                                ToolTip.Tip="Refresh port list"
                                Margin="8,0,0,0"/>
                        
                        <Button Grid.Column="2"
                                Content="{Binding IsConnected, Converter={x:Static BoolConverters.Or}, ConverterParameter='Disconnect|Connect'}"
                                Command="{Binding ConnectCommand}"
                                Margin="8,0,0,0"
                                Width="100"/>
                    </Grid>
                    
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <Ellipse Width="12" Height="12"
                                 Fill="{Binding IsConnected, Converter={x:Static BoolConverters.Or}, ConverterParameter='#4CAF50|#757575'}"/>
                        <TextBlock Text="{Binding IsConnected, Converter={x:Static BoolConverters.Or}, ConverterParameter='Connected|Disconnected'}"
                                   Foreground="#AAAAAA"/>
                    </StackPanel>
                </StackPanel>
            </Border>

            <!-- Bank Operations Section -->
            <Border Background="#2D2D2D" Padding="16" CornerRadius="8">
                <StackPanel Spacing="12">
                    <TextBlock Text="User Bank" 
                               FontWeight="Bold" 
                               FontSize="16"/>
                    
                    <TextBlock Text="Trigger 'Send Dump' from your Nova System pedal (UTILITY → MIDI → Send Dump)"
                               Foreground="#888888"
                               TextWrapping="Wrap"/>
                    
                    <Button Content="📥 Download User Bank"
                            Command="{Binding DownloadBankCommand}"
                            HorizontalAlignment="Left"
                            Padding="16,8"/>
                </StackPanel>
            </Border>
        </StackPanel>
    </DockPanel>
</Window>
```

---

## Task 5.5: Update MainWindow.axaml.cs Code-Behind

**🟢 COMPLEXITY: SIMPLE** — Minimal code-behind

**Status**: Not started  
**Estimated**: 10 min  
**Files**:
- `src/Nova.Presentation/MainWindow.axaml.cs`

### Code
```csharp
using Avalonia.Controls;

namespace Nova.Presentation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

---

## Task 5.6: Create Simple BoolToStringConverter

**🟢 COMPLEXITY: SIMPLE** — Standard converter pattern

**Status**: Not started  
**Estimated**: 15 min  
**Files**:
- `src/Nova.Presentation/Converters/BoolToStringConverter.cs`

### Code
```csharp
using System.Globalization;
using Avalonia.Data.Converters;

namespace Nova.Presentation.Converters;

public class BoolToStringConverter : IValueConverter
{
    public static readonly BoolToStringConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && parameter is string options)
        {
            var parts = options.Split('|');
            return b ? parts[0] : parts[1];
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

---

## Task 5.7: Wire Up Project References

**🟢 COMPLEXITY: TRIVIAL** — Kan udføres af enhver model

**Status**: Not started  
**Estimated**: 10 min  
**Files**:
- `src/Nova.Presentation/Nova.Presentation.csproj`

### Ensure references:
```xml
<ItemGroup>
  <ProjectReference Include="..\Nova.Application\Nova.Application.csproj" />
  <ProjectReference Include="..\Nova.Infrastructure\Nova.Infrastructure.csproj" />
  <ProjectReference Include="..\Nova.Midi\Nova.Midi.csproj" />
</ItemGroup>
```

---

## Task 5.8: Manual End-to-End Test

**🟢 COMPLEXITY: SIMPLE** — Men kræver fysisk hardware

**Status**: Not started  
**Estimated**: 30 min  

### Test Procedure
1. Run `dotnet run --project src/Nova.Presentation`
2. Click "🔄" to refresh ports
3. Select your USB MIDI port
4. Click "Connect" → Verify status shows "Connected"
5. Click "Download User Bank"
6. On pedal: UTILITY → MIDI → Send Dump
7. Verify: Status shows "Downloaded 60 presets"
8. Document any issues in PITFALLS_FOUND.md

---

## Completion Checklist

- [ ] All tests pass
- [ ] Presentation coverage ≥ 50%
- [ ] Manual end-to-end test successful
- [ ] Update `tasks/00-index.md`
- [ ] Update `BUILD_STATE.md`
- [ ] Update `SESSION_MEMORY.md`
- [ ] Commit: `[MODUL-1] [PHASE-5] Implement Avalonia UI for Connect and Download`

---

## Complexity Legend

| Symbol | Meaning | Model Requirement |
|--------|---------|-------------------|
| 🟢 TRIVIAL | Direkte, ingen beslutninger | Enhver model |
| 🟢 SIMPLE | Ligetil logik, få edge cases | Enhver model |
| 🟡 MEDIUM | Kræver framework-forståelse | Haiku/Sonnet |
| 🔴 HIGH | Kompleks patterns/arkitektur | **SONNET 4.5+** |

---

**Status**: READY FOR EXECUTION (after Phase 4)
