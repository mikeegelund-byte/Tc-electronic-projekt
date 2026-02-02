# Risici & Antagelser

## Antagelser (som skal holde for success)

### Hardware assumptions
1. **Nova System firmware v1.2** is deployed on user's device
   - **Risk if violated:** Older firmware may have different SysEx format
   - **Mitigation:** Document v1.2 requirement, add version check (future)

2. **USB-MIDI converter is stable and functional**
   - **Risk:** Unreliable converter → unpredictable MIDI
   - **Mitigation:** Support common brands, test with 2-3 converters

3. **Windows 11 MIDI driver is installed and working**
   - **Risk:** User has broken driver
   - **Mitigation:** Clear error message "Check USB drivers", link to support

### Software assumptions
4. **DryWetMIDI library is stable**
   - **Risk:** Library bug → data corruption
   - **Mitigation:** Validate all incoming SysEx before trusting

5. **No other app will access MIDI port while NovaApp is open**
   - **Risk:** Port contention → MIDI errors
   - **Mitigation:** Show clear error "Another app has MIDI port"

6. **User's Windows 11 system is up-to-date**
   - **Risk:** Old system → compatibility issues
   - **Mitigation:** Require Build 22621+

---

## Known risici (og mitigation)

### MIDI protocol risks

**Risk: SysEx checksum mismatch**
- **Cause:** Noise on USB line, or data corruption
- **Impact:** Corrupted preset loaded
- **Mitigation:**
  - Calculate checksum, verify before parsing
  - If mismatch: log warning, show UI badge "Verify data?"
  - Never silently corrupt user data

**Risk: Timeout (Nova not responding)**
- **Cause:** Hardware malfunction, USB unplugged
- **Impact:** UI freeze, user frustration
- **Mitigation:**
  - 30-second timeout on all I/O
  - Async/await (UI never blocks)
  - Auto-retry 3x with backoff
  - Clear message: "Nova System not responding. Check USB."

**Risk: SysEx chunking (split across USB packets)**
- **Cause:** USB MTU limits (often 64 bytes)
- **Impact:** Incomplete message, parse error
- **Mitigation:**
  - Buffer incoming SysEx until F7 received
  - Test with various USB controllers

**Risk: Port name variation (Windows)**
- **Cause:** Different USB converters → different port names
- **Impact:** User can't find Nova port
- **Mitigation:**
  - Filter ports for "MIDI" keyword
  - Allow manual port selection (dropdown)
  - Show all available ports

---

### Architecture risks

**Risk: MIDI layer not properly isolated**
- **Cause:** Tight coupling between UI and MIDI
- **Impact:** Hard to test, brittle to changes
- **Mitigation:**
  - Use `IMidiPort` interface (mockable)
  - All MIDI in separate assembly
  - 100% test coverage for MIDI layer

**Risk: Preset immutability violated**
- **Cause:** Accidental mutation of preset object
- **Impact:** Data inconsistency
- **Mitigation:**
  - Use immutable types (record, readonly)
  - Deep copy on load
  - Avoid mutable collections

---

### Testing risks

**Risk: Tests pass but app fails on real hardware**
- **Cause:** Incomplete mocking, unrealistic test data
- **Impact:** Released with bugs
- **Mitigation:**
  - Manual testing on real Nova System at milestones
  - Real SysEx fixtures (from actual hardware dumps)
  - Integration tests with mock but realistic timing

---

### User experience risks

**Risk: User overwrites important preset accidentally**
- **Cause:** Misclick or unclear UI
- **Impact:** User loses data
- **Mitigation:**
  - Confirmation dialog before overwrite
  - Undo/Redo (Modul 7)
  - Auto-save to temp file before overwrite

**Risk: Port enumeration fails (no ports found)**
- **Cause:** Driver not installed, Nova not connected
- **Impact:** User can't connect
- **Mitigation:**
  - Clear error message
  - Link to troubleshooting guide
  - "Refresh" button to re-enumerate

---

### Deployment risks

**Risk: Installer fails or has malware flags**
- **Cause:** Unsigned code, SmartScreen warnings
- **Impact:** User can't install
- **Mitigation:**
  - Code-sign executable (Phase 2)
  - Use WiX for proper MSI format
  - Test on clean VM before release

**Risk: App runs on unsupported Windows version**
- **Cause:** User has Windows 10 or older
- **Impact:** Crashes, confusing errors
- **Mitigation:**
  - Installer checks Windows version
  - Require Build 22621+
  - Graceful error if running on old OS

---

## Risk matrix (priority)

| Risk | Likelihood | Impact | Mitigation effort | **Priority** |
|------|------------|--------|-------------------|--------------|
| Checksum mismatch | Medium | High | Low | **HIGH** |
| Timeout/no response | High | Medium | Low | **HIGH** |
| SysEx chunking | Medium | High | Medium | **HIGH** |
| Port not found | Medium | Medium | Low | **MEDIUM** |
| MIDI layer too coupled | Low | High | High | **MEDIUM** |
| User overwrites preset | High | High | Low | **HIGH** |

---

## Mitigation timeline

### Before Modul 1 release
- Checksum validation ✓
- Timeout + retry logic ✓
- SysEx buffering ✓
- Error UI ✓

### Before Modul 6 release
- Confirmation dialogs ✓
- Undo/Redo ✓

### Before Modul 10 release
- Code signing ✓
- Installer testing ✓
- OS version check ✓

---

## Monitoring (future)
- Crash reporting (e.g., Sentry integration)
- Usage telemetry (optional, user opt-in)
- Error logging (local .log file)
