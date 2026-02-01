# Task List: Modul 10 — Polish & Release

## 📋 Module: 10 (Release)
**Duration**: 3 weeks  
**Prerequisite**: Modul 9 complete  
**Output**: Production-ready v1.0.0 release  

---

## Exit Criteria

- [ ] Professional UI appearance
- [ ] Dark/Light mode
- [ ] Keyboard shortcuts
- [ ] WiX installer (.msi)
- [ ] User manual
- [ ] GitHub Actions CI/CD
- [ ] All regression tests pass

---

## Phase 1: UI/UX Polish (1 uge)

### Task 10.1.1: Apply Design System

**🟡 COMPLEXITY: MEDIUM** — Følg 08-ui-guidelines.md

**Status**: Not started  
**Estimated**: 4 hours  

- Consistent spacing (8px grid)
- Color palette (#1E1E1E, #2D2D2D, accent colors)
- Typography (fonts, sizes)
- Border radius (4px, 8px)

---

### Task 10.1.2: Implement Animations

**🟡 COMPLEXITY: MEDIUM** — Avalonia animations

**Status**: Not started  
**Estimated**: 2 hours  

- 100ms hover transitions
- 200ms expand/collapse
- Smooth scrolling

---

### Task 10.1.3: Dark/Light Mode Toggle

**🟡 COMPLEXITY: MEDIUM** — Theme switching

**Status**: Not started  
**Estimated**: 2 hours  

---

### Task 10.1.4: Keyboard Shortcuts

**🟢 COMPLEXITY: SIMPLE** — KeyBindings

**Status**: Not started  
**Estimated**: 1 hour  

| Shortcut | Action |
|----------|--------|
| Ctrl+S | Save preset |
| Ctrl+Z | Undo |
| Ctrl+Y | Redo |
| Ctrl+C | Copy preset |
| F5 | Refresh ports |
| Escape | Cancel dialog |

---

### Task 10.1.5: Accessibility Review

**🟡 COMPLEXITY: MEDIUM** — Contrast, screen reader

**Status**: Not started  
**Estimated**: 2 hours  

---

## Phase 2: Installer & Distribution (1 uge)

### Task 10.2.1: Create WiX Installer

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: WiX XML syntax, component groups, registry

**Status**: Not started  
**Estimated**: 4 hours  
**Reference**: docs/09-release-installer.md

---

### Task 10.2.2: Desktop Shortcut + Start Menu

**🟢 COMPLEXITY: SIMPLE** — WiX shortcut elements

**Status**: Not started  
**Estimated**: 30 min  

---

### Task 10.2.3: Test on Clean VM

**🟢 COMPLEXITY: SIMPLE** — Men tidskrævende

**Status**: Not started  
**Estimated**: 2 hours  

### Procedure:
1. Spin up fresh Windows 11 VM
2. Install .msi
3. Verify app launches
4. Verify MIDI works (if hardware available)
5. Uninstall → verify clean removal

---

### Task 10.2.4: Release Notes Template

**🟢 COMPLEXITY: SIMPLE** — Markdown template

**Status**: Not started  
**Estimated**: 30 min  

---

## Phase 3: Documentation & CI (1 uge)

### Task 10.3.1: Write User Manual

**🟡 COMPLEXITY: MEDIUM** — Screenshots, instructions

**Status**: Not started  
**Estimated**: 4 hours  

Sections:
- Installation
- First connection
- Downloading presets
- Editing parameters
- Saving presets
- File import/export
- Troubleshooting

---

### Task 10.3.2: Setup GitHub Actions

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: YAML workflow, build matrix, artifact publish

**Status**: Not started  
**Estimated**: 3 hours  

```yaml
# .github/workflows/release.yml
name: Build and Release
on:
  push:
    tags: ['v*']
jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet build -c Release
      - run: dotnet test
      - run: dotnet publish -c Release -o publish
      # ... installer build, artifact upload
```

---

### Task 10.3.3: Auto-Update Plan (DEFERRED)

**🔴 COMPLEXITY: HIGH** — Kan udskydes til v1.1

**Note**: Start med manual "Check for Updates" button

**Status**: Deferred to v1.1  

---

### Task 10.3.4: Final Regression Testing

**🟡 COMPLEXITY: MEDIUM** — Comprehensive test suite

**Status**: Not started  
**Estimated**: 4 hours  

### Checklist:
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] Manual flow: Connect → Download → Edit → Save → Verify
- [ ] Import/export roundtrip
- [ ] Installer test on clean machine

---

## v1.0.0 Release Checklist

- [ ] All modules complete (1-10)
- [ ] All tests pass
- [ ] Installer works on clean Windows 11
- [ ] User manual complete
- [ ] Release notes written
- [ ] Git tag: `v1.0.0`
- [ ] GitHub release created
- [ ] Artifact uploaded (.msi)

---

## Post-Release (v1.1 Roadmap)

- Auto-update via Squirrel.Windows
- macOS build
- Cloud preset sync (optional)
- AI preset generation features

---

**Status**: READY (after Modul 9) — **FINAL MODULE**
