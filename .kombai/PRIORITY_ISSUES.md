# Priority Issues Matrix
## Nova System Manager - Kombai Implementation

**Source**: design-review-nova-system-manager-1738540667.md  
**Total Issues**: 50  
**This Document**: Top 20 issues by priority

---

## 🔴 CRITICAL (Must Fix Before Release)

### Issue #2: Low color contrast for status text
**Impact**: WCAG AA violation - accessibility barrier  
**Location**: `MainWindow.axaml:24`, `PresetDetailView.axaml:27,49`  
**Current**: #AAAAAA on #2D2D2D = 2.8:1  
**Required**: 4.5:1 minimum  
**Fix**: Change to #CCCCCC (5.3:1) or #C0C0C0 (4.6:1)

### Issue #3: Missing AutomationProperties for screen reader support
**Impact**: Application unusable for visually impaired users  
**Location**: All XAML files  
**Fix**: Add `AutomationProperties.Name` to all interactive controls (50+ controls)

### Issue #4: No keyboard focus indicators
**Impact**: Keyboard navigation impossible to track  
**Location**: All interactive controls  
**Fix**: Define `FocusVisualStyle` in App.axaml with blue border + glow

### Issue #5: Color-only status indicators
**Impact**: Information not conveyed to colorblind users  
**Location**: `MainWindow.axaml:67-75`  
**Fix**: Add icon + text label alongside color indicator

### Issue #34: OverwriteWarning low contrast
**Impact**: Critical warning may be missed  
**Location**: `SavePresetDialog.axaml:45-53`  
**Current**: #AA5522 background  
**Fix**: Change to #CC6633 (4.7:1 contrast)

---

## 🟠 HIGH (Fix in Phase 1-2)

### Issue #1: Inconsistent background colors
**Impact**: Unprofessional appearance, theme system broken  
**Location**: Multiple files  
**Fix**: Standardize to #1E1E1E (primary) and #2D2D2D (secondary)

### Issue #6: Hardcoded color values
**Impact**: Cannot theme, difficult maintenance  
**Location**: All XAML files  
**Fix**: Create `Themes/NovaTheme.axaml` ResourceDictionary with all colors

### Issue #8: Missing hover states
**Impact**: No visual feedback on interaction  
**Location**: All buttons  
**Fix**: Define `:pointerover` styles with color change + shadow

### Issue #11: Fixed window size
**Impact**: Poor UX on different screen sizes  
**Location**: `MainWindow.axaml:8`  
**Current**: Width="1400" Height="800"  
**Fix**: Change to MinWidth/MinHeight, allow resizing

### Issue #14: Missing tooltips on icon-only buttons
**Impact**: Users don't know what buttons do  
**Location**: `MainWindow.axaml:55-58` (refresh button)  
**Fix**: Add `ToolTip.Tip` with description + keyboard shortcut

### Issue #15: No error validation UI
**Impact**: Users can submit invalid data  
**Location**: `SavePresetDialog.axaml:23-42`  
**Fix**: Add error styling + validation messages

### Issue #21: Dialog cannot resize
**Impact**: Accessibility issue for users needing larger text  
**Location**: `SavePresetDialog.axaml:8`  
**Fix**: Change `CanResize="False"` to `CanResize="True"`, add MinWidth/MinHeight

### Issue #30: NumericUpDown lacks units
**Impact**: Users confused about value meaning  
**Location**: `PresetDetailView.axaml:50-245` (all parameters)  
**Fix**: Add TextBlock with "ms", "dB", "%", "Hz" next to controls

### Issue #32: Missing keyboard shortcuts
**Impact**: Power users forced to use mouse  
**Location**: All windows  
**Fix**: Add `Window.KeyBindings` with Ctrl+R, F5, Ctrl+S, etc.

### Issue #39: No visual indication of editable vs read-only
**Impact**: Users try to edit read-only fields  
**Location**: `SystemSettingsView.axaml:19-69`  
**Fix**: Use different background/border for read-only TextBlocks

---

## 🟡 MEDIUM (Phase 3 - Polish)

### Issue #7: Inconsistent font sizes
**Impact**: Visual hierarchy unclear  
**Location**: Multiple files  
**Fix**: Define typography scale: 11pt body, 13pt labels, 16pt section headers, 20pt titles

### Issue #9: No loading states
**Impact**: Users think app froze during long operations  
**Location**: `MainWindow.axaml:90-93`  
**Fix**: Add ProgressRing + overlay during async operations

### Issue #10: Inconsistent spacing
**Impact**: Unprofessional appearance  
**Location**: All XAML files  
**Fix**: Standardize to 6px/12px/24px system from guidelines

### Issue #17: Emoji usage for icons
**Impact**: Unprofessional, inconsistent rendering  
**Location**: `MainWindow.axaml:37,90,108,113`  
**Fix**: Replace with FluentAvalonia PathIcon components

### Issue #27: Long preset names may truncate
**Impact**: Users can't see full preset names  
**Location**: `PresetListView.axaml:41-43`  
**Fix**: Add `TextTrimming="CharacterEllipsis"` + tooltip with full name

---

## Implementation Priority Order

### Week 1: Critical Accessibility
1. ✅ Create NovaTheme.axaml with WCAG AA compliant colors
2. ✅ Fix all contrast ratios (#2, #34)
3. ✅ Add AutomationProperties to all controls (#3)
4. ✅ Define keyboard focus styles (#4)
5. ✅ Fix color-only indicators (#5)

### Week 2: Theme & Consistency
6. ✅ Replace all hardcoded colors with theme resources (#6)
7. ✅ Add hover states to buttons (#8)
8. ✅ Standardize spacing system (#10)
9. ✅ Fix inconsistent backgrounds (#1)
10. ✅ Define typography scale (#7)

### Week 3: UX Enhancements
11. ✅ Add keyboard shortcuts (#32)
12. ✅ Add tooltips (#14)
13. ✅ Add units to numeric fields (#30)
14. ✅ Add loading states (#9)
15. ✅ Replace emoji with proper icons (#17)

### Week 4: Polish
16. ⚠️ Allow window resizing (#11, #21)
17. ⚠️ Add error validation UI (#15)
18. ⚠️ Show editable vs read-only distinction (#39)
19. ⚠️ Add tooltips for truncated text (#27)
20. ⚠️ Add context menus (from issue #48 - not in top 20 but valuable)

---

## Quick Reference: File Impact

| File | Critical Issues | High Issues | Medium Issues | Total Changes Needed |
|------|----------------|-------------|---------------|---------------------|
| `MainWindow.axaml` | 3 (#2, #5) | 5 (#1, #8, #11, #14, #32) | 3 (#9, #10, #17) | ~35 lines |
| `PresetDetailView.axaml` | 1 (#2) | 2 (#30, #39) | 2 (#10, #27) | ~120 lines |
| `SavePresetDialog.axaml` | 1 (#34) | 3 (#15, #21) | 2 (#10, #17) | ~25 lines |
| `App.axaml` | 2 (#3, #4) | 2 (#6, #8) | 1 (#7) | ~60 lines |
| `PresetListView.axaml` | 0 | 1 (#1) | 2 (#10, #27) | ~15 lines |
| `FileManagerView.axaml` | 0 | 1 (#8) | 2 (#10, #17) | ~10 lines |
| `SystemSettingsView.axaml` | 0 | 1 (#39) | 1 (#10) | ~20 lines |

**Total estimated changes**: ~285 lines across 7 files

---

## Testing After Each Phase

```powershell
# After each file modification:
dotnet build --verbosity quiet
dotnet test --verbosity quiet

# Expected result: 277 tests passing, 0 failed
```

---

## Success Metrics

- **Accessibility Score**: 0% → 95%+ (WCAG AA compliant)
- **Consistency Score**: 40% → 90%+ (follows UI guidelines)
- **UX Score**: 60% → 85%+ (professional interactions)
- **Code Quality**: 0 hardcoded colors, 100% theme coverage
- **Tests**: 277/277 passing (100% maintained)

---

*This document prioritizes the top 20 issues from the full 50-issue review.*  
*Focus on Critical + High issues first for maximum impact.*
