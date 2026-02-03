# File Modification Checklist
## Nova System Manager UI/UX Implementation

**Total Files to Modify**: 8 core files + 1 new file  
**Estimated Time**: 8-12 hours for Phase 1+2  
**Test Requirement**: 277/277 tests must pass after each phase

---

## New Files to Create

### ✅ Create: `src/Nova.Presentation/Themes/NovaTheme.axaml`
**Purpose**: Centralized theme ResourceDictionary  
**Size**: ~150 lines  
**Priority**: CRITICAL - Create this first

**Contents**:
- [ ] All color definitions (18 colors)
- [ ] Spacing system (6 values)
- [ ] Typography (fonts, sizes, weights)
- [ ] Component sizing (buttons, icons)
- [ ] Border radius values

**Reference**: See `THEME_SPECIFICATION.md` for complete definition

---

## Files to Modify (Order of Implementation)

### 1. `src/Nova.Presentation/App.axaml`
**Current**: 22 lines  
**Changes**: ~40 lines  
**Priority**: CRITICAL

**Modifications**:
- [ ] Add ResourceDictionary reference to NovaTheme.axaml
- [ ] Define global Button styles (hover, pressed, disabled)
- [ ] Define global input styles (focus, validation)
- [ ] Add keyboard focus indicators (FocusVisualStyle)
- [ ] Define DataGrid styles (alternating rows, hover)

**Test after**: `dotnet build`

---

### 2. `src/Nova.Presentation/MainWindow.axaml`
**Current**: 117 lines  
**Changes**: ~35 modifications  
**Priority**: CRITICAL

**Line-by-line changes**:
- [ ] Line 8: Add MinWidth="1200" MinHeight="700" (remove fixed size)
- [ ] Line 17-18: Replace `#2D2D2D` → `{StaticResource BackgroundSecondary}`
- [ ] Line 24: Replace `#AAAAAA` → `{StaticResource TextSecondary}`
- [ ] Line 26-30: Fix ProgressBar vertical alignment
- [ ] Lines 37, 108, 113: Replace emoji in TabItem headers with PathIcon
- [ ] Line 42: Replace `#1E1E1E` → `{StaticResource BackgroundPrimary}`
- [ ] Line 49-52: Add AutomationProperties to ComboBox
- [ ] Line 55-58: Add tooltip to refresh button
- [ ] Line 60-65: Add AutomationProperties to buttons
- [ ] Lines 67-75: Replace Ellipse with PathIcon + text for connection status
- [ ] Line 90-93: Add AutomationProperties + loading state
- [ ] Add Window.KeyBindings for Ctrl+R, F5, Ctrl+D

**Color replacements**:
```
Find: Background="#2D2D2D"  →  Background="{StaticResource BackgroundSecondary}"
Find: Background="#1E1E1E"  →  Background="{StaticResource BackgroundPrimary}"
Find: Foreground="#AAAAAA"  →  Foreground="{StaticResource TextSecondary}"
```

**Test after**: `dotnet build && dotnet test`

---

### 3. `src/Nova.Presentation/Views/SavePresetDialog.axaml`
**Current**: 95 lines  
**Changes**: ~20 modifications  
**Priority**: HIGH

**Line-by-line changes**:
- [ ] Line 8: Change `CanResize="False"` → `CanResize="True"`, add MinWidth="400" MinHeight="250"
- [ ] Line 12: Replace background color → `{StaticResource BackgroundSecondary}`
- [ ] Line 24: Remove IsReadOnly="True" from preset name TextBox
- [ ] Line 23-42: Add AutomationProperties to all inputs
- [ ] Lines 45-53: Fix OverwriteWarning contrast (#AA5522 → `{StaticResource WarningBackground}`)
- [ ] Lines 50, 76, 83: Replace emoji with PathIcon
- [ ] Line 68: Replace `#0066CC` → `{StaticResource AccentPrimary}`
- [ ] Add validation error UI (red border on invalid input)

**Test after**: `dotnet build && dotnet test`

---

### 4. `src/Nova.Presentation/Views/PresetDetailView.axaml`
**Current**: 247 lines  
**Changes**: ~120 modifications  
**Priority**: HIGH

**Major changes**:
- [ ] Lines 12, 39: Replace background colors with theme resources
- [ ] Lines 27, 49: Replace `#AAAAAA` → `{StaticResource TextSecondary}`
- [ ] Lines 50-245: Add units (ms, dB, %, Hz) to ALL NumericUpDown controls
- [ ] Lines 50-245: Add AutomationProperties.Name to all 50+ controls
- [ ] Lines 50-245: Add tooltips with valid ranges
- [ ] All Margins: Replace with `{StaticResource SpacingMedium}`

**Example transformation** (repeat for each parameter):
```xml
<!-- BEFORE -->
<TextBlock Text="Delay Time:" Grid.Row="0" Grid.Column="0" />
<NumericUpDown Value="{Binding DelayTime}" Grid.Row="0" Grid.Column="1" />

<!-- AFTER -->
<TextBlock Text="Delay Time:" Grid.Row="0" Grid.Column="0" 
           Foreground="{StaticResource TextPrimary}" />
<StackPanel Orientation="Horizontal" Spacing="4" Grid.Row="0" Grid.Column="1">
  <NumericUpDown Value="{Binding DelayTime}" MinWidth="80"
                 AutomationProperties.Name="Delay time in milliseconds"
                 ToolTip.Tip="Valid range: 0 to 1000 ms" />
  <TextBlock Text="ms" VerticalAlignment="Center" 
             Foreground="{StaticResource TextTertiary}" />
</StackPanel>
```

**Test after**: `dotnet build && dotnet test`

---

### 5. `src/Nova.Presentation/Views/PresetListView.axaml`
**Current**: 49 lines  
**Changes**: ~15 modifications  
**Priority**: MEDIUM

**Line-by-line changes**:
- [ ] Lines 7, 34: Replace background colors with theme resources
- [ ] Line 12: Font size matches typography scale
- [ ] Lines 23-49: Add AutomationProperties to DataGrid
- [ ] Line 37-40: Add alternating row background
- [ ] Lines 41-43: Add TextTrimming + tooltip for long names
- [ ] Lines 16-21: Improve empty state message

**Test after**: `dotnet build && dotnet test`

---

### 6. `src/Nova.Presentation/Views/FileManagerView.axaml`
**Current**: 74 lines  
**Changes**: ~10 modifications  
**Priority**: MEDIUM

**Line-by-line changes**:
- [ ] Replace all background colors with theme resources
- [ ] Lines 24, 30: Standardize button padding
- [ ] Lines 20-53: Add AutomationProperties to all buttons
- [ ] Lines 20-53: Disable "TODO" buttons (IsEnabled="False")
- [ ] Replace emoji with PathIcon

**Test after**: `dotnet build && dotnet test`

---

### 7. `src/Nova.Presentation/Views/SystemSettingsView.axaml`
**Current**: 85 lines  
**Changes**: ~20 modifications  
**Priority**: MEDIUM

**Line-by-line changes**:
- [ ] Line 10: Change Margin="20" → Margin="{StaticResource SpacingLarge}"
- [ ] Lines 19-69: Add visual distinction for read-only fields (lighter background)
- [ ] Lines 47, 57: Replace "True/False" with user-friendly labels ("Enabled/Disabled")
- [ ] Line 77, 84: Remove `$parent[Window]` pattern, use proper binding
- [ ] Line 80: Replace `#4CAF50` → `{StaticResource SuccessColor}`
- [ ] Add AutomationProperties to all controls

**Test after**: `dotnet build && dotnet test`

---

### 8. `src/Nova.Presentation/Converters/` (All converters)
**Files**: BoolToColorConverter.cs, EffectTypeConverter.cs, etc.  
**Changes**: Update hardcoded color strings  
**Priority**: MEDIUM

**Changes needed**:
- [ ] Replace hardcoded hex colors with theme resource references
- [ ] Or: Use converter parameters to pass theme colors from XAML

**Note**: May require refactoring to accept colors as parameters instead of hardcoding.

**Test after**: `dotnet build && dotnet test`

---

## Phase-by-Phase Implementation

### Phase 1: Foundation (2-3 hours)
1. ✅ Create NovaTheme.axaml
2. ✅ Modify App.axaml (add theme + global styles)
3. ✅ Test: `dotnet build`

### Phase 2: Critical Fixes (3-4 hours)
4. ✅ Modify MainWindow.axaml (accessibility + contrast)
5. ✅ Modify SavePresetDialog.axaml (contrast + validation)
6. ✅ Test: `dotnet build && dotnet test` (277/277 must pass)

### Phase 3: Enhancements (2-3 hours)
7. ✅ Modify PresetDetailView.axaml (units + tooltips)
8. ✅ Modify PresetListView.axaml (consistency)
9. ✅ Test: `dotnet build && dotnet test`

### Phase 4: Polish (1-2 hours)
10. ✅ Modify FileManagerView.axaml
11. ✅ Modify SystemSettingsView.axaml
12. ✅ Update converters
13. ✅ Final test: `dotnet build && dotnet test`

---

## Verification Commands

After each file modification:
```powershell
# Quick check
dotnet build --verbosity quiet

# Full validation
dotnet test --verbosity quiet

# Expected output:
# Passed! - Failed: 0, Passed: 277, Skipped: 0, Total: 277
```

---

## File Backup Strategy

Before modifying each file:
```powershell
# Create backup
Copy-Item "src/Nova.Presentation/MainWindow.axaml" "src/Nova.Presentation/MainWindow.axaml.backup"

# After verification, delete backup
Remove-Item "src/Nova.Presentation/MainWindow.axaml.backup"
```

---

## Common Patterns for Find/Replace

### Pattern 1: Background Colors
```
Find:    Background="#2D2D2D"
Replace: Background="{StaticResource BackgroundSecondary}"

Find:    Background="#1E1E1E"
Replace: Background="{StaticResource BackgroundPrimary}"
```

### Pattern 2: Text Colors
```
Find:    Foreground="#AAAAAA"
Replace: Foreground="{StaticResource TextSecondary}"

Find:    Foreground="#EEEEEE"
Replace: Foreground="{StaticResource TextPrimary}"
```

### Pattern 3: Spacing
```
Find:    Margin="12"
Replace: Margin="{StaticResource SpacingMedium}"

Find:    Padding="16,8"
Replace: Padding="{StaticResource ButtonPadding}"
```

### Pattern 4: Font Sizes
```
Find:    FontSize="11"
Replace: FontSize="{StaticResource FontSizeSmall}"

Find:    FontSize="16"
Replace: FontSize="{StaticResource FontSizeLarge}"
```

---

## Rollback Plan

If tests fail after modification:

1. **Identify which file broke tests**:
   ```powershell
   git diff
   ```

2. **Revert specific file**:
   ```powershell
   git checkout -- src/Nova.Presentation/MainWindow.axaml
   ```

3. **Re-test**:
   ```powershell
   dotnet test
   ```

4. **Document issue** in `.kombai/issues-encountered.md`

---

## Success Criteria Checklist

- [ ] NovaTheme.axaml created with all resources
- [ ] All 8 XAML files modified
- [ ] Zero hardcoded colors remain (grep check)
- [ ] All interactive controls have AutomationProperties
- [ ] All buttons have hover/pressed styles
- [ ] All numeric fields have units
- [ ] All icon buttons have tooltips
- [ ] Keyboard shortcuts defined
- [ ] Build succeeds: `dotnet build` ✅
- [ ] All tests pass: `dotnet test` (277/277) ✅
- [ ] Contrast ratios verified (WCAG AA) ✅
- [ ] No emoji in production UI ✅

---

## Post-Implementation Tasks

1. **Run full test suite 3 times** to check for flaky tests
2. **Visual inspection** of all windows/views
3. **Keyboard navigation test** (Tab through all controls)
4. **High contrast mode test** (Windows settings)
5. **Screen reader test** (NVDA or Narrator)
6. **Generate before/after screenshots**
7. **Update documentation** with new theme system
8. **Create migration guide** for future UI changes

---

*Use this checklist to track progress and ensure nothing is missed.*  
*Check off items as completed, document any issues encountered.*
