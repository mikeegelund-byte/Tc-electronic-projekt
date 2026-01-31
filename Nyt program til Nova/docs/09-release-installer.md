# Release & installer

## Versionering (SemVer)
Format: `MAJOR.MINOR.PATCH` (e.g., 1.0.0, 1.2.3)

- **MAJOR:** Breaking changes
- **MINOR:** New features (backward compat)
- **PATCH:** Bug fixes

**Version in code:**  
`.csproj` → `<Version>1.0.0</Version>`

---

## Release process

### 1. Pre-release checks
```bash
# Run full test suite
dotnet test

# Build release version
dotnet build -c Release

# Verify no warnings
```

### 2. Tag in Git
```bash
git tag -a v1.0.0 -m "Release 1.0.0: MVP with bank dump"
git push origin v1.0.0
```

### 3. Create release notes
**File:** `RELEASE_NOTES_1.0.0.md`
- Features added
- Bug fixes
- Known issues
- Installation instructions

### 4. Package for distribution
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

**Output:** `NovaApp/bin/Release/net8.0-windows/win-x64/publish/`

---

## Windows Installer (.msi) - Phase 2

**Tool:** WiX (Windows Installer XML)

**Contents:**
- Executable
- Runtime dependencies (if not self-contained)
- Desktop shortcut
- Start menu entry
- Uninstall support
- Registry entries (file association for .syx later)

**Installer name:** `NovaSystemApp-1.0.0-x64.msi`

---

## Desktop shortcut
- **Target:** `C:\Program Files\NovaApp\Nova.Presentation.exe`
- **Icon:** App icon (256x256 .ico)
- **Working directory:** App install dir

---

## Auto-update (Phase 3, future)

**Strategy:** Squirrel.Windows + GitHub releases

1. App checks `https://api.github.com/repos/user/nova-app/releases/latest`
2. If newer version, downloads .msi
3. Runs installer in silent mode
4. Relaunches app

---

## Distribution channels

### Phase 1 (MVP): Manual
- GitHub Releases page
- Users download .zip or .msi
- Extract and run installer

### Phase 2: Auto-update
- Squirrel.Windows integration
- Users click "Check for updates"
- Download + install + relaunch

### Phase 3+ (future): Windows Store
- Microsoft Store listing
- One-click install
- Automatic updates

---

## Code signing (later)
**Why:** Avoid SmartScreen warnings on first run

**Process (when ready):**
1. Get code-signing certificate (Sectigo, Digicert, etc.)
2. Sign executable: `signtool sign /f cert.pfx /p password /t http://timestamp.sectigo.com novaapp.exe`
3. Include timestamp URL (for long-term validity)

---

## Artifact checklist
- [ ] Executable (.exe)
- [ ] Dependencies bundled (if self-contained)
- [ ] App icon (.ico)
- [ ] Release notes
- [ ] README (installation steps)
- [ ] LICENSE file
- [ ] Changelog

---

## Testing release build
Before shipping:
1. Run installer on clean Windows 11 VM
2. Verify app launches
3. Test basic functionality (connect, download)
4. Verify uninstall removes all files
5. Check Desktop shortcut works

---

## Version bumping guide
- **First modul 1 release:** v1.0.0
- **Add modul 2 features:** v1.1.0
- **Bug fix in modul 2:** v1.1.1
- **Breaking change (e.g., new MIDI protocol):** v2.0.0

---

## GitHub Actions (later)
```yaml
name: Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
      - run: dotnet publish -c Release -r win-x64
      - uses: softprops/action-gh-release@v1
        with:
          files: bin/Release/**/*
```
