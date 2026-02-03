# Kombai Implementation Instructions
## Nova System Manager UI/UX Fixes

**Project**: TC Electronic Nova System Manager  
**Framework**: Avalonia UI (C# Desktop Application)  
**Current Status**: 90% functionally complete, 277 passing tests  
**Priority**: Fix critical accessibility & consistency issues before v1.0 release

---

## Project Context

This is a **professional desktop application** for managing TC Electronic Nova System effects pedal via MIDI. The application uses:
- **Architecture**: Clean Architecture + MVVM pattern
- **Language**: C# 12 / .NET 8
- **UI Framework**: Avalonia UI 11.x
- **Testing**: xUnit with 277 passing tests
- **Design Language**: Apple-inspired minimal design (see UI Guidelines)

**Critical Constraint**: Follow TDD methodology - all UI changes must maintain existing test coverage.

---

## Phase 1: Critical Accessibility Fixes (Priority: CRITICAL)

### 1.1 Color Contrast Compliance (WCAG AA)
**Current Issues:**
- Status text #AAAAAA on #2D2D2D = 2.8:1 ratio (needs 4.5:1)
- Warning background #AA5522 has insufficient contrast

**Required Actions:**
```xaml
<!-- Replace ALL instances of: -->
<SolidColorBrush Color="#AAAAAA" />  <!-- Change to #C0C0C0 (4.6:1) or #CCCCCC (5.3:1) -->
<SolidColorBrush Color="#AA5522" />  <!-- Change to #CC6633 (4.7:1) -->
```

**Files to modify:**
- `MainWindow.axaml` (lines 24, 67-75)
- `PresetDetailView.axaml` (lines 27, 49)
- `SavePresetDialog.axaml` (lines 45-53)

### 1.2 Screen Reader Support
**Add to ALL interactive controls:**
```xaml
<!-- Buttons -->
<Button Content="Connect" 
        AutomationProperties.Name="Connect to MIDI device"
        AutomationProperties.HelpText="Establishes connection to selected MIDI port" />

<!-- ComboBoxes -->
<ComboBox AutomationProperties.Name="MIDI Port Selection"
          AutomationProperties.LabeledBy="{Binding #PortLabel}" />

<!-- NumericUpDown -->
<NumericUpDown AutomationProperties.Name="Delay time in milliseconds"
               AutomationProperties.HelpText="Valid range: 0 to 1000" />
```

**Apply to files:**
- All buttons in `MainWindow.axaml`, `FileManagerView.axaml`, `SavePresetDialog.axaml`
- All input controls in `PresetDetailView.axaml` (50+ NumericUpDown controls)
- ComboBoxes in `MainWindow.axaml` and `SavePresetDialog.axaml`

### 1.3 Keyboard Focus Indicators
**Create in App.axaml:**
```xaml
<Application.Styles>
  <Style Selector="Button:focus-visible">
    <Setter Property="BorderBrush" Value="#0078D4" />
    <Setter Property="BorderThickness" Value="2" />
  </Style>
  
  <Style Selector="TextBox:focus-visible, ComboBox:focus-visible, NumericUpDown:focus-visible">
    <Setter Property="BorderBrush" Value="#0078D4" />
    <Setter Property="BorderThickness" Value="2" />
    <Setter Property="Effect">
      <DropShadowEffect Color="#0078D4" BlurRadius="4" OffsetX="0" OffsetY="0" Opacity="0.4" />
    </Setter>
  </Style>
</Application.Styles>
```

### 1.4 Replace Color-Only Indicators
**Current issue:** Connection status shows only green/gray ellipse

**Fix in MainWindow.axaml (lines 67-75):**
```xaml
<!-- Replace: -->
<StackPanel Orientation="Horizontal" Grid.Column="2">
  <Ellipse Width="12" Height="12" Fill="{Binding StatusColor}" Margin="0,0,8,0" />
  <TextBlock Text="{Binding StatusText}" FontSize="13" />
</StackPanel>

<!-- With: -->
<StackPanel Orientation="Horizontal" Grid.Column="2">
  <PathIcon Width="16" Height="16" 
            Data="{Binding StatusIcon}"
            Foreground="{Binding StatusColor}" 
            Margin="0,0,8,0"
            AutomationProperties.Name="{Binding StatusText}" />
  <TextBlock Text="{Binding StatusText}" 
             FontSize="13" 
             Foreground="#CCCCCC" />
</StackPanel>
```

### 1.5 Keyboard Shortcuts
**Add to MainWindow.axaml:**
```xaml
<Window.KeyBindings>
  <KeyBinding Gesture="Ctrl+R" Command="{Binding RefreshPortsCommand}" />
  <KeyBinding Gesture="Ctrl+D" Command="{Binding DownloadBankCommand}" />
  <KeyBinding Gesture="Ctrl+S" Command="{Binding SavePresetCommand}" />
  <KeyBinding Gesture="F5" Command="{Binding DownloadBankCommand}" />
</Window.KeyBindings>
```

---

## Phase 2: Theme System & Consistency (Priority: HIGH)

### 2.1 Create Theme ResourceDictionary
**Create new file: `src/Nova.Presentation/Themes/NovaTheme.axaml`**

```xaml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  
  <!-- Color Palette (WCAG AA Compliant) -->
  <SolidColorBrush x:Key="BackgroundPrimary" Color="#1E1E1E" />
  <SolidColorBrush x:Key="BackgroundSecondary" Color="#2D2D2D" />
  <SolidColorBrush x:Key="BackgroundTertiary" Color="#3A3A3A" />
  
  <SolidColorBrush x:Key="TextPrimary" Color="#EEEEEE" />
  <SolidColorBrush x:Key="TextSecondary" Color="#CCCCCC" />
  <SolidColorBrush x:Key="TextTertiary" Color="#999999" />
  
  <SolidColorBrush x:Key="AccentPrimary" Color="#0078D4" />
  <SolidColorBrush x:Key="AccentHover" Color="#1084E0" />
  <SolidColorBrush x:Key="AccentPressed" Color="#006CBE" />
  
  <SolidColorBrush x:Key="SuccessColor" Color="#107C10" />
  <SolidColorBrush x:Key="ErrorColor" Color="#D92E1B" />
  <SolidColorBrush x:Key="WarningColor" Color="#FF8C00" />
  
  <!-- Spacing System -->
  <Thickness x:Key="SpacingSmall">6</Thickness>
  <Thickness x:Key="SpacingMedium">12</Thickness>
  <Thickness x:Key="SpacingLarge">24</Thickness>
  
  <!-- Typography -->
  <FontFamily x:Key="FontPrimary">Segoe UI</FontFamily>
  <FontFamily x:Key="FontMonospace">Consolas</FontFamily>
  
  <x:Double x:Key="FontSizeSmall">11</x:Double>
  <x:Double x:Key="FontSizeMedium">13</x:Double>
  <x:Double x:Key="FontSizeLarge">16</x:Double>
  <x:Double x:Key="FontSizeTitle">20</x:Double>
  
</ResourceDictionary>
```

### 2.2 Reference Theme in App.axaml
```xaml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <ResourceInclude Source="/Themes/NovaTheme.axaml" />
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

### 2.3 Replace ALL Hardcoded Colors
**Global find-and-replace in ALL .axaml files:**

```
Find: Background="#2D2D2D"
Replace: Background="{StaticResource BackgroundSecondary}"

Find: Background="#1E1E1E"
Replace: Background="{StaticResource BackgroundPrimary}"

Find: Foreground="#AAAAAA"
Replace: Foreground="{StaticResource TextSecondary}"

Find: Foreground="#4CAF50"
Replace: Foreground="{StaticResource SuccessColor}"

Find: Foreground="#0066CC"
Replace: Foreground="{StaticResource AccentPrimary}"
```

### 2.4 Standardize Spacing
**Replace ALL margin/padding values:**
- `Margin="8"` → `Margin="{StaticResource SpacingSmall}"`
- `Margin="12"` → `Margin="{StaticResource SpacingMedium}"`
- `Margin="16"` → `Margin="{StaticResource SpacingMedium}"` (round to 12)
- `Margin="20"` → `Margin="{StaticResource SpacingLarge}"` (round to 24)
- `Margin="24"` → `Margin="{StaticResource SpacingLarge}"`

### 2.5 Replace Emoji with Proper Icons
**Install FluentAvalonia package first**, then replace:

```xaml
<!-- Before -->
<Button Content="🔌 Connect" />
<TabItem Header="💾 File Manager" />
<Button Content="📥 Download Bank" />

<!-- After -->
<Button>
  <StackPanel Orientation="Horizontal" Spacing="8">
    <PathIcon Data="{StaticResource PlugConnectedRegular}" Width="16" />
    <TextBlock Text="Connect" />
  </StackPanel>
</Button>

<TabItem>
  <TabItem.Header>
    <StackPanel Orientation="Horizontal" Spacing="8">
      <PathIcon Data="{StaticResource SaveRegular}" Width="16" />
      <TextBlock Text="File Manager" />
    </StackPanel>
  </TabItem.Header>
</TabItem>
```

**Files with emoji to replace:**
- `MainWindow.axaml`: 🔌, 💾, 📥, ⚙️
- `SavePresetDialog.axaml`: ⚠️, ✓, ✗

---

## Phase 3: UX Enhancements (Priority: MEDIUM)

### 3.1 Add Units to Numeric Fields
**In PresetDetailView.axaml, wrap ALL NumericUpDown:**

```xaml
<!-- Before -->
<NumericUpDown Value="{Binding DelayTime}" Grid.Column="1" />

<!-- After -->
<StackPanel Orientation="Horizontal" Spacing="4" Grid.Column="1">
  <NumericUpDown Value="{Binding DelayTime}" MinWidth="80" />
  <TextBlock Text="ms" VerticalAlignment="Center" 
             Foreground="{StaticResource TextTertiary}" />
</StackPanel>
```

**Apply units:**
- Time parameters: "ms"
- Level parameters: "dB"
- Percentage parameters: "%"
- Frequency: "Hz"

### 3.2 Add Hover States
**In App.axaml or NovaTheme.axaml:**

```xaml
<Style Selector="Button:pointerover">
  <Setter Property="Background" Value="{StaticResource AccentHover}" />
  <Setter Property="Effect">
    <DropShadowEffect BlurRadius="8" OffsetX="0" OffsetY="2" Opacity="0.3" />
  </Setter>
</Style>

<Style Selector="Button:pressed">
  <Setter Property="Background" Value="{StaticResource AccentPressed}" />
  <Setter Property="Effect">
    <DropShadowEffect BlurRadius="4" OffsetX="0" OffsetY="1" Opacity="0.2" />
  </Setter>
</Style>

<Style Selector="DataGridRow:pointerover /template/ Rectangle#BackgroundRectangle">
  <Setter Property="Fill" Value="#2A2A2A" />
</Style>
```

### 3.3 Add Tooltips
**Add to ALL icon buttons and controls:**

```xaml
<Button ToolTip.Tip="Refresh available MIDI ports (Ctrl+R)">
  <PathIcon Data="{StaticResource ArrowSyncRegular}" />
</Button>

<NumericUpDown ToolTip.Tip="Delay time: 0-1000 milliseconds" />
```

### 3.4 Loading States
**Add to MainWindow.axaml for async operations:**

```xaml
<ProgressBar IsIndeterminate="{Binding IsLoading}"
             IsVisible="{Binding IsLoading}"
             Height="4"
             Grid.Row="1"
             Grid.ColumnSpan="3" />

<Border IsVisible="{Binding IsLoading}"
        Background="#80000000"
        Grid.RowSpan="5">
  <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center"
              Background="{StaticResource BackgroundSecondary}"
              Padding="24">
    <ProgressRing IsActive="True" Width="48" Height="48" />
    <TextBlock Text="{Binding LoadingMessage}" 
               Margin="0,12,0,0"
               TextAlignment="Center" />
  </StackPanel>
</Border>
```

---

## Implementation Guidelines

### File Modification Order
1. **Create theme first**: `Themes/NovaTheme.axaml`
2. **Update App.axaml**: Add theme reference + global styles
3. **Fix critical accessibility** (Phase 1): MainWindow, SavePresetDialog
4. **Replace hardcoded colors** (Phase 2): All .axaml files
5. **Add enhancements** (Phase 3): PresetDetailView, interactions

### Testing Requirements
After each file modification:
```powershell
dotnet build --verbosity quiet
dotnet test --verbosity quiet
```
**All 277 tests must remain green.**

### Code Style Conventions
- Use `StaticResource` for theme values, never `DynamicResource`
- Keep XAML indentation at 2 spaces
- Order properties: AutomationProperties → Layout → Visual → Behavioral
- Always add `AutomationProperties.Name` to interactive elements

### DO NOT Change
- **Any ViewModel code** - UI changes only
- **Test files** - Tests must remain unchanged
- **Use case logic** - Application layer stays intact
- **Domain models** - No business logic changes

---

## Validation Checklist

Before considering work complete, verify:

- [ ] All colors use `{StaticResource}` references
- [ ] WCAG AA contrast ratios met (min 4.5:1 for text)
- [ ] All interactive controls have `AutomationProperties.Name`
- [ ] Keyboard focus visible on all focusable elements
- [ ] All keyboard shortcuts defined in `Window.KeyBindings`
- [ ] Tooltips present on icon-only buttons
- [ ] No emoji in production UI (use PathIcon instead)
- [ ] Spacing follows 6/12/24px system
- [ ] All numeric fields show units (ms, dB, %, Hz)
- [ ] Build succeeds: `dotnet build`
- [ ] All tests pass: `dotnet test` (277 tests green)

---

## Reference Files

**Must read before starting:**
- `docs/08-ui-guidelines.md` - Official design guidelines
- `.kombai/resources/design-review-nova-system-manager-*.md` - Full issue list
- `src/Nova.Presentation/App.axaml` - Application entry point
- `src/Nova.Presentation/MainWindow.axaml` - Main UI structure

**Architecture documentation:**
- `docs/03-architecture.md` - System architecture
- `docs/00-index.md` - Project overview

---

## Deliverables

Provide the following outputs:

1. **Modified XAML files** with all Phase 1 & 2 changes
2. **New theme file**: `Themes/NovaTheme.axaml`
3. **Change summary** listing every modified file and what was changed
4. **Contrast ratio report** proving WCAG AA compliance
5. **Before/after screenshots** (if possible to generate)

---

## Success Criteria

✅ All 7 critical accessibility issues resolved  
✅ All 13 high-priority consistency issues resolved  
✅ 277 tests remain green after all changes  
✅ Application builds without warnings  
✅ WCAG AA compliance verified for all text/background combinations  
✅ No hardcoded colors remain in .axaml files  

---

*Last updated: February 3, 2026*
*Based on Kombai Design Review Report: design-review-nova-system-manager-1738540667.md*
