# Nova System Manager - User Manual
**Version**: 1.0.0  
**Last Updated**: February 3, 2026

---

## Table of Contents

1. [Introduction](#introduction)
2. [System Requirements](#system-requirements)
3. [Installation](#installation)
4. [First Time Setup](#first-time-setup)
5. [Connecting to Your Nova System](#connecting-to-your-nova-system)
6. [Downloading Presets](#downloading-presets)
7. [Viewing and Editing Presets](#viewing-and-editing-presets)
8. [Saving Presets](#saving-presets)
9. [File Operations](#file-operations)
10. [System Settings](#system-settings)
11. [MIDI Mapping](#midi-mapping)
12. [Keyboard Shortcuts](#keyboard-shortcuts)
13. [Troubleshooting](#troubleshooting)
14. [FAQ](#faq)

---

## Introduction

Nova System Manager is a desktop application for managing presets and settings on your TC Electronic Nova System multi-effects pedal. With this software, you can:

- **Download** all 60 presets from your pedal
- **Edit** effect parameters for all 7 effect blocks
- **Save** modified presets back to the hardware
- **Import/Export** presets as `.syx` files for backup and sharing
- **Configure** MIDI CC mappings and expression pedal response
- **Organize** your preset library with ease

---

## System Requirements

### Minimum Requirements
- **Operating System**: Windows 10 (64-bit) or later
- **RAM**: 4 GB
- **Storage**: 200 MB free space
- **.NET Runtime**: .NET 8.0 Desktop Runtime (included in installer)
- **MIDI Interface**: USB MIDI interface or built-in MIDI port

### Recommended Requirements
- **Operating System**: Windows 11 (64-bit)
- **RAM**: 8 GB or more
- **Screen Resolution**: 1920x1080 or higher

### Hardware Requirements
- TC Electronic Nova System guitar effects processor
- USB MIDI interface (recommended: class-compliant USB MIDI adapter)
- Standard MIDI cables (5-pin DIN)

---

## Installation

### Step 1: Download the Installer

1. Navigate to the [GitHub Releases page](https://github.com/mikeegelund-byte/Tc-electronic-projekt/releases)
2. Download the latest `NovaSystemManager-v1.0.0.msi` file

### Step 2: Run the Installer

1. Double-click the downloaded `.msi` file
2. If you see a security warning, click **"Run"** or **"Yes"**
3. Follow the installation wizard:
   - Accept the license agreement
   - Choose installation folder (default: `C:\Program Files\NovaSystemManager\`)
   - Click **Install**

### Step 3: Launch the Application

After installation completes:
- A desktop shortcut will be created automatically
- You can also launch from **Start Menu → Nova System Manager**

---

## First Time Setup

### 1. Connect Your MIDI Interface

Before launching the application:
1. Connect your USB MIDI interface to your computer
2. Connect MIDI cables from the interface to your Nova System:
   - **MIDI OUT** (interface) → **MIDI IN** (Nova System)
   - **MIDI IN** (interface) → **MIDI OUT** (Nova System)
3. Power on your Nova System pedal
4. Wait for Windows to recognize the MIDI device (usually automatic)

### 2. Launch Nova System Manager

1. Open the application from the desktop shortcut
2. The main window will appear with the **Connection** tab active

---

## Connecting to Your Nova System

### Automatic Port Detection

1. Click the **Refresh** button (↻) or press **Ctrl+R**
2. The application will scan for available MIDI ports
3. Select your MIDI interface from the dropdown list
   - Example names: "USB MIDI Interface", "Focusrite USB MIDI", etc.

### Manual Connection

1. After selecting the MIDI port, click **Connect**
2. If successful, you'll see:
   - **Green indicator** with "Connected" status
   - Status bar shows: "Connected to [Port Name]"

### Troubleshooting Connection Issues

If the connection fails:
- Verify MIDI cables are connected correctly
- Ensure the Nova System is powered on
- Try unplugging and reconnecting the USB MIDI interface
- Click **Refresh** (Ctrl+R) to rescan ports
- Check Windows Device Manager for MIDI device status

---

## Downloading Presets

### Download All 60 Presets

1. Ensure you're connected to the Nova System
2. Click **"Download User Bank"** or press **F5** (or **Ctrl+D**)
3. A progress indicator will appear
4. Wait for the download to complete (~15-30 seconds)
5. The status bar will show: "Downloaded 60 presets"

### Viewing Downloaded Presets

After downloading:
- The **Preset List** on the left will populate with all 60 presets
- Each preset shows its **position** (1-60) and **name**
- Presets are displayed in numerical order

---

## Viewing and Editing Presets

### Select a Preset

1. Click any preset in the **Preset List** on the left
2. The **Preset Detail View** on the right will display all parameters

### Preset Information Display

The detail view shows:

#### Global Parameters
- **Tap Tempo**: Tempo setting for time-based effects (100-3000 ms)
- **Routing**: Signal path configuration (Serial/Semi-Parallel/Parallel)
- **Level Out Left/Right**: Output levels (-20 to +20 dB)

#### Effect Blocks (7 total)

Each effect block displays:
- **On/Off indicator** (green circle = active, gray = bypassed)
- **Effect Type**: Current effect algorithm
- **Parameters**: All adjustable parameters with current values and units

**Available Effect Blocks:**
1. **Compressor** (Percussive, Sustaining, Advanced)
2. **Drive** (Overdrive, Distortion - NDT™ analog section)
3. **Modulation** (Chorus, Flanger, Vibrato, Phaser, Tremolo, Panner)
4. **Delay** (Clean, Analog, Tape, Dynamic, Dual, Ping-Pong)
5. **Reverb** (Spring, Hall, Room, Plate)
6. **EQ + Noise Gate** (3-band parametric EQ)
7. **Pitch** (Shifter, Octaver, Whammy, Detune, Intelligent)

### Units Display

All numeric parameters show appropriate units:
- **Time**: milliseconds (ms)
- **Level**: decibels (dB)
- **Percentage**: percent (%)
- **Frequency**: Hertz (Hz) - where applicable

---

## Saving Presets

### Save to Hardware

After editing a preset:
1. Ensure you're connected to the Nova System
2. Click **"Save Preset"** or press **Ctrl+S**
3. The **Save Preset Dialog** will appear:
   - **Preset Name**: Enter a name (max 16 characters)
   - **Target Slot**: Choose slot 1-60
   - **Overwrite Warning**: If the slot is occupied, a warning appears
4. Click **"Save to Hardware"**
5. Wait for confirmation: "Preset saved successfully"

### Undo/Redo

- **Undo**: Press **Ctrl+Z** to undo the last change
- **Redo**: Press **Ctrl+Y** to redo an undone change

### Copy Preset

- **Copy**: Press **Ctrl+C** to copy the currently selected preset

---

## File Operations

### Export Preset to File

1. Select a preset from the list
2. Navigate to **File Manager** tab
3. Click **"Export Preset"**
4. Choose a save location
5. Enter a filename (e.g., `MyFavoriteSound.syx`)
6. Click **Save**

### Import Preset from File

1. Navigate to **File Manager** tab
2. Click **"Import Preset"**
3. Browse to the `.syx` file
4. Select the file and click **Open**
5. The preset will be loaded into the application
6. Use **"Save Preset"** to write it to the hardware

### Export/Import User Bank

**Export Entire Bank** (all 60 presets):
1. Click **"Export User Bank"**
2. Choose save location
3. Enter filename (e.g., `MyBankBackup.syx`)
4. Click **Save**

**Import Entire Bank**:
1. Click **"Import User Bank"**
2. Browse to the bank `.syx` file
3. Select and click **Open**
4. All 60 presets will be loaded
5. Click **"Upload Bank"** to write them to the hardware

### File Format

Nova System Manager uses **SysEx** (System Exclusive) format:
- **Single Preset**: ~520 bytes
- **User Bank**: ~31 KB (60 presets)
- **System Dump**: ~1 KB (global settings)

Files are compatible with the original NovaManager v1.20.1 software.

---

## System Settings

### View System Configuration

1. Navigate to **System Settings** tab
2. View read-only global settings:
   - **MIDI Channel**: Current MIDI channel (1-16)
   - **Global Bypass**: Master bypass setting
   - **Device ID**: SysEx device identifier

### Edit System Settings

⚠️ **Note**: System settings editing is planned for v1.1. Currently, these values are read-only.

---

## MIDI Mapping

### CC MIDI Mapping

**View CC Assignments**:
1. Navigate to **MIDI Mapping** tab
2. The CC Assignment Table shows:
   - **CC Number** (0-127)
   - **Assigned Parameter**
   - **Current Value**

**Edit CC Assignment** (v1.1 feature):
- Select a CC number
- Choose the parameter to control
- Save changes to hardware

### Expression Pedal Configuration

**Configure Pedal Mapping**:
1. Navigate to **MIDI Mapping** tab → **Pedal Mapping** section
2. Set **Parameter**: Which effect parameter the pedal controls
3. Adjust **Response Curve**:
   - **Minimum Value**: Value when pedal is heel-down (0%)
   - **Middle Value**: Value at mid-travel (50%)
   - **Maximum Value**: Value when pedal is toe-down (100%)

**Visual Curve Editor**:
- Drag the yellow control points to adjust the response curve
- The curve shows how pedal position maps to parameter value
- Linear, exponential, and custom curves are possible

**Pedal Calibration** (v1.1 feature):
- Click **"Learn Min"** and move pedal to heel-down position
- Click **"Learn Max"** and move pedal to toe-down position
- This ensures accurate tracking for your specific pedal

---

## Keyboard Shortcuts

### Main Window
| Shortcut | Action |
|----------|--------|
| **Ctrl+R** | Refresh MIDI ports |
| **F5** | Download User Bank (all 60 presets) |
| **Ctrl+D** | Download User Bank (alternate) |
| **Ctrl+S** | Save current preset to hardware |
| **Ctrl+Z** | Undo last change |
| **Ctrl+Y** | Redo last undone change |
| **Ctrl+C** | Copy current preset |
| **Escape** | Cancel/Close dialog |

### Dialog Windows
| Shortcut | Action |
|----------|--------|
| **Enter** | Confirm action (Save, OK) |
| **Escape** | Cancel and close dialog |

---

## Troubleshooting

### Common Issues and Solutions

#### Application Won't Launch
**Symptom**: Double-clicking the shortcut does nothing  
**Solution**:
1. Check if .NET 8.0 Desktop Runtime is installed:
   - Open **Control Panel → Programs → Programs and Features**
   - Look for "Microsoft .NET Runtime - 8.0..."
2. If missing, download from: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
3. Reinstall Nova System Manager

#### No MIDI Ports Found
**Symptom**: Dropdown list is empty after clicking Refresh  
**Solution**:
1. Verify USB MIDI interface is connected
2. Check Windows Device Manager:
   - Open **Device Manager** → **Sound, video and game controllers**
   - Look for your MIDI device (no yellow warning icon)
3. Try a different USB port
4. Restart the computer
5. Update MIDI interface drivers

#### Connection Fails
**Symptom**: "Failed to connect" error message  
**Solution**:
1. Verify MIDI cables are connected correctly:
   - OUT → IN and IN → OUT
2. Ensure Nova System is powered on and initialized
3. Check if another application is using the MIDI port (close it)
4. Try disconnecting and reconnecting the USB MIDI interface
5. Restart Nova System Manager

#### Download Hangs or Fails
**Symptom**: Progress bar stops or error occurs during download  
**Solution**:
1. Ensure stable MIDI connection (cables not loose)
2. Close other MIDI applications
3. Try downloading again (click **Download User Bank**)
4. If problem persists:
   - Disconnect and reconnect MIDI interface
   - Restart Nova System Manager
   - Restart Nova System pedal

#### Preset Changes Don't Save
**Symptom**: Edited parameters revert after saving  
**Solution**:
1. Verify you're connected to the Nova System
2. Ensure the correct target slot is selected
3. Check that the Nova System is not in a protected mode
4. Try saving to a different slot
5. Re-download the bank and try editing again

#### Wrong Values Displayed
**Symptom**: Parameters show incorrect numbers  
**Solution**:
1. Click **Download User Bank** (F5) to refresh from hardware
2. Ensure you're editing the correct preset slot
3. Check that the Nova System firmware is up to date
4. Verify MIDI cables are making good contact

---

## FAQ

### General Questions

**Q: Is Nova System Manager compatible with macOS or Linux?**  
A: Version 1.0 is Windows-only. macOS support is planned for v1.1.

**Q: Can I use this without a MIDI interface?**  
A: No, a MIDI interface is required to communicate with the Nova System pedal.

**Q: Does this replace the original NovaManager software?**  
A: Yes, Nova System Manager is a modern replacement with more features and better compatibility with Windows 10/11.

**Q: Are my preset files from NovaManager v1.20.1 compatible?**  
A: Yes! All `.syx` files are fully compatible in both directions.

### Technical Questions

**Q: What MIDI protocol does this use?**  
A: Standard MIDI SysEx (System Exclusive) messages specific to the TC Electronic Nova System.

**Q: How long does it take to download 60 presets?**  
A: Approximately 15-30 seconds depending on your MIDI interface speed.

**Q: Can I edit presets offline without the hardware connected?**  
A: Yes, you can import `.syx` files, edit them, export them, and later upload to the hardware.

**Q: What happens if I disconnect the MIDI cable during a save operation?**  
A: The operation will fail with an error message. No data will be corrupted on the pedal.

**Q: Can I control multiple Nova System pedals?**  
A: You can connect to one pedal at a time. To switch between pedals, disconnect and select a different MIDI port.

### Feature Requests

**Q: Will there be a preset library or cloud sync feature?**  
A: Preset library management is planned for v1.1. Cloud sync is under consideration for v1.2.

**Q: Can I add custom effect algorithms?**  
A: No, effect algorithms are built into the Nova System hardware and cannot be modified.

**Q: Will there be MIDI learn functionality for CC mapping?**  
A: Yes! MIDI Learn mode for CC mapping is planned for v1.1.

**Q: Can I automate preset changes via MIDI Program Change?**  
A: This is a hardware feature of the Nova System. The manager software does not need to be running for this.

---

## Support and Resources

### Official Resources
- **GitHub Repository**: [https://github.com/mikeegelund-byte/Tc-electronic-projekt](https://github.com/mikeegelund-byte/Tc-electronic-projekt)
- **Report Issues**: [https://github.com/mikeegelund-byte/Tc-electronic-projekt/issues](https://github.com/mikeegelund-byte/Tc-electronic-projekt/issues)
- **Discussions**: [https://github.com/mikeegelund-byte/Tc-electronic-projekt/discussions](https://github.com/mikeegelund-byte/Tc-electronic-projekt/discussions)

### Community
- **TC Electronic Forum**: [https://forum.tcelectronic.com](https://forum.tcelectronic.com) (community discussions)
- **Discord**: Join the Nova System user community (link in GitHub repository)

### Additional Documentation
- **MIDI Protocol**: See `MIDI_PROTOCOL.md` in the repository
- **Effect Reference**: See `EFFECT_REFERENCE.md` for parameter details
- **Developer Docs**: See `docs/` folder in the repository

---

## Version History

### Version 1.0.0 (February 3, 2026)
- Initial public release
- Full SysEx protocol implementation
- Preset download/upload
- Effect parameter editing
- File import/export (.syx)
- Expression pedal mapping
- Dark/Light theme support
- WCAG AA accessibility compliance
- 340+ unit tests

---

## License

Nova System Manager is open-source software licensed under the MIT License.

Copyright © 2026 TC Electronic Community

---

**End of User Manual**

For the latest updates and information, visit:  
[https://github.com/mikeegelund-byte/Tc-electronic-projekt](https://github.com/mikeegelund-byte/Tc-electronic-projekt)
