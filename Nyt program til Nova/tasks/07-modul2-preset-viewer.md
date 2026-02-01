# Task List: Modul 2 — Preset Viewer

## 📋 Module: 2 (Preset Viewer)
**Duration**: 1 week  
**Prerequisite**: Modul 1 complete (all 5 phases)  
**Output**: List of 60 preset names visible in UI  

---

## Overview

**Goal**: Display the downloaded User Bank as a scrollable list showing preset number and name.

---

## Exit Criteria (Modul 2 Complete When ALL True)

- [ ] All 60 presets displayed in a list/grid (IN PROGRESS - UI created)
- [x] Each row shows: Position (00-1 to 19-3), Name ✅ (ViewModels done)
- [x] List is sortable by position ✅ (OrderBy preset number)
- [ ] Loading indicator during download (TODO - wire to MainViewModel)
- [ ] Handles empty/corrupt preset names gracefully (TODO - Task 2.5)
- [ ] All tests pass (TODO - Task 2.5)

---

## Task 2.1: Create PresetSummaryViewModel

**🟢 COMPLEXITY: SIMPLE** — Simpel record/class

**Status**: ✅ COMPLETE  
**Estimated**: 15 min  
**Files**:
- `src/Nova.Presentation/ViewModels/PresetSummaryViewModel.cs`

### Code
```csharp
namespace Nova.Presentation.ViewModels;

public record PresetSummaryViewModel(
    int Number,
    string Position,  // "00-1", "00-2", "00-3", "01-1", etc.
    string Name,
    int BankGroup     // 0-19
)
{
    public static PresetSummaryViewModel FromPreset(Preset preset)
    {
        var bankGroup = (preset.Number - 31) / 3;
        var slot = ((preset.Number - 31) % 3) + 1;
        var position = $"{bankGroup:D2}-{slot}";
        
        return new PresetSummaryViewModel(
            preset.Number,
            position,
            preset.Name,
            bankGroup
        );
    }
}
```

---

## Task 2.2: Create PresetListViewModel

**🟡 COMPLEXITY: MEDIUM** — ObservableCollection + sorting

**Status**: Not started  
**Estimated**: 30 min  
**Files**:
- `src/Nova.Presentation/ViewModels/PresetListViewModel.cs`
- `src/Nova.Presentation.Tests/ViewModels/PresetListViewModelTests.cs`

### Test First (RED)
```csharp
[Fact]
public void LoadFromBank_PopulatesWith60Presets()
{
    var vm = new PresetListViewModel();
    var mockBank = CreateMockUserBankDump(60);
    
    vm.LoadFromBank(mockBank);
    
    vm.Presets.Should().HaveCount(60);
}

[Fact]
public void LoadFromBank_SortsByPositionAscending()
{
    var vm = new PresetListViewModel();
    var mockBank = CreateMockUserBankDump(60);
    
    vm.LoadFromBank(mockBank);
    
    vm.Presets[0].Position.Should().Be("00-1");
    vm.Presets[59].Position.Should().Be("19-3");
}
```

### Code (GREEN)
```csharp
public partial class PresetListViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<PresetSummaryViewModel> _presets = new();

    [ObservableProperty]
    private PresetSummaryViewModel? _selectedPreset;

    public void LoadFromBank(UserBankDump bank)
    {
        Presets.Clear();
        
        var sorted = bank.Presets
            .Where(p => p != null)
            .OrderBy(p => p!.Number)
            .Select(p => PresetSummaryViewModel.FromPreset(p!));

        foreach (var preset in sorted)
        {
            Presets.Add(preset);
        }
    }
}
```

---

## Task 2.3: Create PresetListView.axaml

**🟡 COMPLEXITY: MEDIUM** — DataGrid XAML

**Status**: Not started  
**Estimated**: 45 min  
**Files**:
- `src/Nova.Presentation/Views/PresetListView.axaml`
- `src/Nova.Presentation/Views/PresetListView.axaml.cs`

### Code (AXAML)
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Nova.Presentation.ViewModels"
             x:Class="Nova.Presentation.Views.PresetListView"
             x:DataType="vm:PresetListViewModel">

    <Border Background="#2D2D2D" Padding="16" CornerRadius="8">
        <DockPanel>
            <TextBlock DockPanel.Dock="Top" 
                       Text="User Bank Presets" 
                       FontWeight="Bold" 
                       FontSize="16"
                       Margin="0,0,0,12"/>

            <DataGrid ItemsSource="{Binding Presets}"
                      SelectedItem="{Binding SelectedPreset}"
                      AutoGenerateColumns="False"
                      IsReadOnly="True"
                      CanUserReorderColumns="False"
                      CanUserResizeColumns="True"
                      GridLinesVisibility="Horizontal"
                      BorderThickness="0">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="Position" 
                                        Binding="{Binding Position}" 
                                        Width="80"/>
                    <DataGridTextColumn Header="Name" 
                                        Binding="{Binding Name}" 
                                        Width="*"/>
                </DataGrid.Columns>
            </DataGrid>
        </DockPanel>
    </Border>
</UserControl>
```

---

## Task 2.4: Integrate PresetList into MainWindow

**🟢 COMPLEXITY: SIMPLE** — XAML composition

**Status**: Not started  
**Estimated**: 20 min  
**Files**:
- `src/Nova.Presentation/MainWindow.axaml`
- `src/Nova.Presentation/ViewModels/MainViewModel.cs`

### Update MainViewModel
```csharp
[ObservableProperty]
private PresetListViewModel _presetList = new();

// In DownloadBankAsync, after successful download:
PresetList.LoadFromBank(result.Value);
```

### Update MainWindow.axaml
Add after Bank Operations section:
```xml
<views:PresetListView DataContext="{Binding PresetList}" />
```

---

## Task 2.5: Handle Edge Cases

**🟢 COMPLEXITY: SIMPLE** — Defensive coding

**Status**: Not started  
**Estimated**: 20 min  
**Files**:
- `src/Nova.Presentation/ViewModels/PresetSummaryViewModel.cs`

### Updates
```csharp
public static PresetSummaryViewModel FromPreset(Preset preset)
{
    // Handle empty or corrupt names
    var name = string.IsNullOrWhiteSpace(preset.Name) 
        ? $"[Unnamed #{preset.Number}]" 
        : preset.Name.Trim();
    
    // ... rest of logic
}
```

---

## Completion Checklist

- [ ] All tests pass (TODO - Task 2.5)
- [ ] 60 presets display correctly (TODO - Task 2.4 wire-up + manual test)
- [ ] Edge cases handled (TODO - Task 2.5)
- [x] Update `tasks/00-index.md` ✅
- [x] Update `BUILD_STATE.md` (TODO - when complete)
- [x] Commit: `[MODUL-2] Implement Preset Viewer` (IN PROGRESS - Tasks 2.1-2.3 done)

---

## Complexity Legend

| Symbol | Meaning | Model Requirement |
|--------|---------|-------------------|
| 🟢 TRIVIAL/SIMPLE | Ligetil | Enhver model |
| 🟡 MEDIUM | Framework-kendskab | Haiku/Sonnet |
| 🔴 HIGH | Kompleks arkitektur | **SONNET 4.5+** |

---

**Status**: 🔄 IN PROGRESS (Tasks 2.1-2.3 DONE, working on 2.4 wire-up)
