# Manual Hardware Test Guide - Task 2.6

## Overview
This guide provides step-by-step instructions for manually testing the Preset Viewer functionality with a physical Nova System pedal.

---

## Prerequisites

### Hardware
- ✅ Physical TC Electronic Nova System pedal
- ✅ USB MIDI Interface (e.g., Roland UM-ONE, M-Audio MIDISport)
- ✅ MIDI cables connecting Nova System to USB MIDI Interface
- ✅ Computer with Windows/macOS/Linux

### Software
- ✅ .NET 8.0 Runtime installed
- ✅ Nova System Manager application built (`dotnet build`)
- ✅ USB MIDI Interface drivers installed (if required)

---

## Test Procedure

### Step 1: Start the Application

1. Navigate to the project directory:
   ```bash
   cd "C:\Projekter\Mikes preset app"
   ```

2. Run the application:
   ```bash
   dotnet run --project src/Nova.Presentation
   ```

3. **Expected Result:**
   - Application window opens with title "Nova System Manager"
   - Connection panel visible at top
   - Status bar shows "Found X MIDI IN / Y MIDI OUT"

### Step 2: Connect to Pedal

1. In the MIDI Connection section:
   - Vælg **MIDI OUT** (til pedalens MIDI IN)
   - Vælg **MIDI IN** (fra pedalens MIDI OUT)
   - Hvis du ser “MIDI 0/1”, brug:
     - MIDI OUT = **MIDI 0**
     - MIDI IN = **MIDI 1**

2. Click the **Connect** button

3. **Expected Results:**
   - ✅ Connect button becomes inactive (grayed out)
   - ✅ Green indicator dot appears with "Connected" text
   - ✅ Status bar shows "Connected (IN: ... / OUT: ...)"
   - ✅ Download Bank button becomes active

### Step 3: Download User Bank

1. On your Nova System pedal:
   - Press **UTILITY** button
   - Navigate to **MIDI** section
   - Select **Send Dump**
   - Choose **User Bank** (60 presets)
   - Confirm to send

2. In the application:
   - Click **📥 Download User Bank** button

3. **Expected Results:**
   - ✅ Status bar shows "Waiting for User Bank dump from pedal..."
   - ✅ Progress bar appears (if visible)
   - ✅ After ~5 seconds, status shows "Downloaded 60 presets successfully"
   - ✅ Preset list appears below with data

### Step 4: Verify Preset List Display

Examine the "User Bank Presets (60)" section:

#### Column 1: Position
- ✅ First row: "00-1" (Bank 0, Slot 1)
- ✅ Second row: "00-2" (Bank 0, Slot 2)
- ✅ Third row: "00-3" (Bank 0, Slot 3)
- ✅ Fourth row: "01-1" (Bank 1, Slot 1)
- ✅ Last row: "19-3" (Bank 19, Slot 3)
- ✅ Format: "XX-Y" where XX = bank (00-19), Y = slot (1-3)

#### Column 2: Name
- ✅ Shows preset names from your pedal
- ✅ If any preset has empty name, shows "[Unnamed #XX]" where XX is preset number
- ✅ No corrupted characters or garbled text
- ✅ Names are trimmed (no leading/trailing spaces)

#### Column 3: Preset #
- ✅ Shows numbers 31-90
- ✅ First row: 31
- ✅ Last row: 90
- ✅ Numbers are sequential in ascending order

#### Overall List
- ✅ Total of 60 rows displayed
- ✅ No empty/blank rows
- ✅ List is scrollable if needed
- ✅ Selection works (clicking a row highlights it)

### Step 5: Verify Edge Cases

#### Test Empty Preset Names
1. If you have presets with empty names on your pedal:
   - ✅ Verify they display as "[Unnamed #XX]" format
   - ✅ Example: "[Unnamed #45]" for preset number 45

2. If no empty names exist, this is normal
   - Document: "No empty preset names found on test pedal"

#### Test UI Responsiveness
1. Scroll through the entire list
   - ✅ Scrolling is smooth
   - ✅ No lag or freezing
   - ✅ All 60 rows remain visible during scrolling

2. Select different presets
   - ✅ Row highlights on click
   - ✅ Selection changes smoothly

3. Check the application console/output
   - ✅ No error messages
   - ✅ No exceptions or warnings

---

## Hardware CLI Tests (optional)

Kør fra `C:\Projekter\Mikes preset app`:

```bash
dotnet run --project src/Nova.HardwareTest -- --list-devices
dotnet run --project src/Nova.HardwareTest -- --pair-test --midi-in="MIDI 1" --midi-out="MIDI 0"
dotnet run --project src/Nova.HardwareTest -- --request-system-dump --midi-in="MIDI 1" --midi-out="MIDI 0"
dotnet run --project src/Nova.HardwareTest -- --request-bank-dump --midi-in="MIDI 1" --midi-out="MIDI 0"
dotnet run --project src/Nova.HardwareTest -- --roundtrip-bank --midi-in="MIDI 1" --midi-out="MIDI 0"
dotnet run --project src/Nova.HardwareTest -- --cc-learn --midi-in="MIDI 1" --midi-out="MIDI 0"
dotnet run --project src/Nova.HardwareTest -- --disconnect-reconnect --midi-in="MIDI 1" --midi-out="MIDI 0"
```

> Justér portnavne efter dine faktiske enheder. `MIDI OUT` skal gå til pedalens MIDI IN.

---

## Expected Final State

After completing all steps:
- ✅ Application is running without errors
- ✅ Connected to Nova System pedal
- ✅ 60 presets downloaded and displayed
- ✅ All columns (Position, Name, Preset#) show correct data
- ✅ UI is responsive and functional
- ✅ No corrupted data or display issues

---

## Troubleshooting

### Port Not Found
**Problem:** MIDI IN/OUT doesn't appear in dropdown

**Solutions:**
- Check physical MIDI cable connections
- Verify USB MIDI Interface is powered on
- Install/reinstall USB MIDI Interface drivers
- Click the 🔄 Refresh button to re-scan ports

### Wrong IN/OUT Pairing
**Problem:** Connection succeeds but no data received or send fails

**Solutions:**
- Swap MIDI IN/OUT selections (MIDI OUT must go to pedal MIDI IN)
- Confirm pedal device ID and MIDI channel
- Try "MIDI 0" as OUT and "MIDI 1" as IN if available

### Connection Fails
**Problem:** Connect button clicked but connection fails

**Solutions:**
- Check MIDI cables are in correct IN/OUT ports
- Try a different MIDI channel (pedal should be on channel 1)
- Restart the application
- Power cycle the Nova System pedal

### No Data Received
**Problem:** Download Bank button clicked but no presets appear

**Solutions:**
- Verify pedal actually sent the dump (check pedal display)
- Try triggering "Send Dump" again from the pedal
- Ensure you selected "User Bank" not "System Dump"
- Wait up to 60 seconds (timeout period)
- Check status bar for error messages

### Display Issues
**Problem:** Presets show but data looks wrong

**Check:**
- Position format correct? Should be "00-1" to "19-3"
- Preset numbers correct? Should be 31-90
- Names match what's on your pedal?
- If data seems correct, this may be expected behavior

---

## Test Results Template

Copy this template to document your test results:

```
## Manual Hardware Test Results - Task 2.6
**Date:** [YYYY-MM-DD]
**Tester:** [Your Name]
**Pedal:** TC Electronic Nova System
**MIDI Interface:** [Interface Model]

### Test Results
- [ ] Step 1: Application Start - PASS/FAIL
- [ ] Step 2: Connect to Pedal - PASS/FAIL
- [ ] Step 3: Download User Bank - PASS/FAIL
- [ ] Step 4: Verify Preset List Display - PASS/FAIL
- [ ] Step 5: Verify Edge Cases - PASS/FAIL

### Issues Found
[List any issues, bugs, or unexpected behavior]

### Additional Notes
[Any other observations or comments]

### Screenshots
[Attach screenshot of preset list displaying data]

### Final Verdict
✅ PASS - All tests passed, ready for production
❌ FAIL - Issues found, needs fixes
```

---

## Next Steps After Testing

1. Document test results using template above
2. Take screenshot of working preset list
3. Update PROGRESS.md with test outcome
4. If tests pass: Mark Task 2.6 as ✅ COMPLETE
5. If tests fail: Document issues and create fix tasks

---

**Test Guide Version:** 1.0  
**Created:** 2025-02-01  
**Last Updated:** 2025-02-01
