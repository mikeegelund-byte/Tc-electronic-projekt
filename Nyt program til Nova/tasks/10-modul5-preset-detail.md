# Task List: Modul 5 — Preset Detail Viewer

## 📋 Module: 5 (Preset Detail Viewer)
**Duration**: 2 weeks  
**Prerequisite**: Modul 4 complete  
**Output**: Read-only view of all effect parameters for selected preset  

---

## Overview

**Goal**: When user clicks a preset in the list, show all effect block parameters in a detailed view.

**Note**: Parameter PARSING is already complete in Domain layer (Preset.cs has all 78 parameters). This module is purely UI.

---

## Exit Criteria

- [ ] Click preset → show all 7 effect blocks
- [ ] Each block shows type + all parameters
- [ ] Collapsible sections for each effect
- [ ] All parameter values match hardware
- [ ] All tests pass

---

## Task 5.1: Create Effect Block ViewModels (7 total)

**🟡 COMPLEXITY: MEDIUM** — Repetitive men kræver domain-forståelse

**Status**: ✅ COMPLETE  
**Estimated**: 2 hours
**Actual**: DriveBlockViewModel (4 tests), Compressor, Modulation, Delay, Reverb, Pitch, EqGate created  
**Files**:
- `src/Nova.Presentation/ViewModels/Effects/DriveBlockViewModel.cs`
- `src/Nova.Presentation/ViewModels/Effects/CompressorBlockViewModel.cs`
- `src/Nova.Presentation/ViewModels/Effects/EqGateBlockViewModel.cs`
- `src/Nova.Presentation/ViewModels/Effects/ModulationBlockViewModel.cs`
- `src/Nova.Presentation/ViewModels/Effects/PitchBlockViewModel.cs`
- `src/Nova.Presentation/ViewModels/Effects/DelayBlockViewModel.cs`
- `src/Nova.Presentation/ViewModels/Effects/ReverbBlockViewModel.cs`

### Example (DriveBlockViewModel)
```csharp
public partial class DriveBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Overdrive";
    [ObservableProperty] private int _gain;
    [ObservableProperty] private int _level;
    [ObservableProperty] private bool _isEnabled;

    public void LoadFromPreset(Preset preset)
    {
        Type = preset.DriveType switch
        {
            0 => "Overdrive",
            1 => "Distortion",
            // ... etc from EFFECT_REFERENCE.md
        };
        Gain = preset.DriveGain;
        Level = preset.DriveLevel;
        IsEnabled = preset.DriveEnabled;
    }
}
```

---

## Task 5.2: Create PresetDetailViewModel

**🟡 COMPLEXITY: MEDIUM** — Composition af effect VMs

**Status**: ✅ COMPLETE  
**Estimated**: 30 min
**Actual**: Composes all 7 effect blocks, orchestrates LoadFromPreset()  
**Files**:
- `src/Nova.Presentation/ViewModels/PresetDetailViewModel.cs`

```csharp
public partial class PresetDetailViewModel : ObservableObject
{
    [ObservableProperty] private string _presetName = "";
    [ObservableProperty] private string _position = "";
    
    public DriveBlockViewModel Drive { get; } = new();
    public CompressorBlockViewModel Compressor { get; } = new();
    public EqGateBlockViewModel EqGate { get; } = new();
    public ModulationBlockViewModel Modulation { get; } = new();
    public PitchBlockViewModel Pitch { get; } = new();
    public DelayBlockViewModel Delay { get; } = new();
    public ReverbBlockViewModel Reverb { get; } = new();

    public void LoadFromPreset(Preset preset)
    {
        PresetName = preset.Name;
        // ... populate all effect blocks
    }
}
```

---

## Task 5.3: Create EffectBlockView Reusable Control

**🟡 COMPLEXITY: MEDIUM** — Avalonia UserControl

**Status**: Not started  
**Estimated**: 45 min  
**Files**:
- `src/Nova.Presentation/Views/Controls/EffectBlockView.axaml`

### Features:
- Collapsible header (click to expand/collapse)
- On/off indicator (green/gray dot)
- Type label
- Grid of parameter name:value pairs

---

## Task 5.4: Create PresetDetailView.axaml

**🟡 COMPLEXITY: MEDIUM** — Layout composition

**Status**: Not started  
**Estimated**: 60 min  
**Files**:
- `src/Nova.Presentation/Views/PresetDetailView.axaml`

```xml
<ScrollViewer>
    <StackPanel Spacing="8">
        <views:EffectBlockView Header="Drive" DataContext="{Binding Drive}" />
        <views:EffectBlockView Header="Compressor" DataContext="{Binding Compressor}" />
        <views:EffectBlockView Header="EQ &amp; Gate" DataContext="{Binding EqGate}" />
        <views:EffectBlockView Header="Modulation" DataContext="{Binding Modulation}" />
        <views:EffectBlockView Header="Pitch" DataContext="{Binding Pitch}" />
        <views:EffectBlockView Header="Delay" DataContext="{Binding Delay}" />
        <views:EffectBlockView Header="Reverb" DataContext="{Binding Reverb}" />
    </StackPanel>
</ScrollViewer>
```

---

## Task 5.5: Wire Selection to Detail View

**🟢 COMPLEXITY: SIMPLE** — Property binding

**Status**: Not started  
**Estimated**: 20 min  

### In MainViewModel:
```csharp
partial void OnSelectedPresetChanged(PresetSummaryViewModel? value)
{
    if (value != null)
    {
        var fullPreset = _currentBank?.GetPreset(value.Number);
        if (fullPreset != null)
        {
            PresetDetail.LoadFromPreset(fullPreset);
        }
    }
}
```

---

## Completion Checklist

- [ ] All 7 effect blocks display
- [ ] Parameter values correct
- [ ] All tests pass
- [ ] Commit: `[MODUL-5] Implement Preset Detail Viewer`

---

## Complexity Legend

| Symbol | Model Requirement |
|--------|-------------------|
| 🟢 SIMPLE | Enhver model |
| 🟡 MEDIUM | Haiku/Sonnet |
| 🔴 HIGH | **SONNET 4.5+** |

---

**Status**: READY (after Modul 4)
