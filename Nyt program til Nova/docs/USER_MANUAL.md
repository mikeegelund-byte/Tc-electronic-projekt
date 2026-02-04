# Nova System Manager - User Manual

**Version:** 1.0  
**Last Updated:** February 3, 2026

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Installation](#2-installation)
3. [Getting Started](#3-getting-started)
4. [Main Interface](#4-main-interface)
5. [Working with Presets](#5-working-with-presets)
6. [System Settings](#6-system-settings)
7. [File Operations](#7-file-operations)
8. [Keyboard Shortcuts](#8-keyboard-shortcuts)
9. [Troubleshooting](#9-troubleshooting)
10. [Technical Reference](#10-technical-reference)

---

## 1. Introduction

### What is Nova System Manager

Nova System Manager is a modern, cross-platform application for managing your TC Electronic Nova System guitar multi-effects pedal. It provides full control over presets, system settings, and MIDI configuration through an intuitive, user-friendly interface.

The application replaces the legacy NovaManager software with:
- **Modern UI design** - Clean, intuitive interface with dark/light themes
- **Full MIDI protocol support** - Complete compatibility with Nova System hardware
- **Cross-platform** - Works on Windows, macOS, and Linux
- **Advanced features** - Preset management, system backup, expression pedal calibration
- **Fast and responsive** - Sub-100ms parameter updates

### System Requirements

**Minimum Requirements:**
- **Operating System:**
  - Windows 10 or later (64-bit)
  - macOS 10.15 (Catalina) or later
  - Linux (Ubuntu 20.04 or equivalent)
- **RAM:** 2 GB minimum, 4 GB recommended
- **Storage:** 100 MB free disk space
- **Display:** 1024x768 minimum resolution

**Additional Requirements:**
- USB port for MIDI interface
- Internet connection for initial download (not required for operation)

### What You'll Need

To use Nova System Manager, you'll need:

1. **TC Electronic Nova System Pedal**
   - Firmware version 1.2 or later recommended
   - Any Nova System model is supported

2. **USB-MIDI Interface**
   - Standard USB-MIDI cable or adapter
   - Must support bidirectional MIDI communication
   - Examples: M-Audio MIDI cables, Roland UM-ONE, or similar

3. **MIDI Cables** (if not using USB-MIDI)
   - Standard 5-pin DIN MIDI cables
   - Connect from computer MIDI interface to Nova System MIDI In/Out ports

> **Tip:** Most USB-MIDI interfaces work plug-and-play on modern operating systems. Check your device documentation if drivers are required.

---

## 2. Installation

### Download and Run Installer

#### Windows Installation

1. **Download the Installer**
   - Visit the official releases page
   - Download `NovaSystemManager-Setup-x64.exe`
   - Save to your Downloads folder

2. **Run the Installer**
   - Double-click the downloaded installer
   - If Windows SmartScreen appears, click "More info" → "Run anyway"
   - Accept the User Account Control prompt

3. **Follow Installation Wizard**
   - Choose installation location (default: `C:\Program Files\Nova System Manager\`)
   - Select whether to create desktop shortcut
   - Click "Install" and wait for completion
   - Click "Finish" to exit the installer

4. **First Launch**
   - The application will launch automatically after installation
   - Or find "Nova System Manager" in your Start menu

#### macOS Installation

1. **Download the Installer**
   - Visit the official releases page
   - Download `NovaSystemManager.dmg`
   - Save to your Downloads folder

2. **Install the Application**
   - Double-click the `.dmg` file to mount it
   - Drag "Nova System Manager" to your Applications folder
   - Eject the disk image

3. **First Launch**
   - Open Applications folder
   - Double-click "Nova System Manager"
   - If macOS shows a security warning:
     - Go to System Preferences → Security & Privacy
     - Click "Open Anyway" next to Nova System Manager

#### Linux Installation

1. **Download the Package**
   - Visit the official releases page
   - Download the appropriate package:
     - `.deb` for Ubuntu/Debian
     - `.rpm` for Fedora/RedHat
     - `.AppImage` for universal Linux

2. **Install the Package**
   
   **For .deb (Ubuntu/Debian):**
   ```bash
   sudo dpkg -i NovaSystemManager.deb
   sudo apt-get install -f  # Install dependencies if needed
   ```
   
   **For .rpm (Fedora/RedHat):**
   ```bash
   sudo rpm -i NovaSystemManager.rpm
   ```
   
   **For .AppImage:**
   ```bash
   chmod +x NovaSystemManager.AppImage
   ./NovaSystemManager.AppImage
   ```

3. **First Launch**
   - Find "Nova System Manager" in your applications menu
   - Or run from terminal: `nova-system-manager`

### First Launch

When you first launch Nova System Manager:

1. **Welcome Screen**
   - The application will display a welcome message
   - You may be prompted to select your preferred theme (Light/Dark)

2. **MIDI Port Detection**
   - The application will automatically scan for available MIDI ports
   - Connected MIDI devices will be listed in the port selector

3. **Main Window Opens**
   - You'll see the main interface with connection panel at the top
   - Status bar at the bottom will show "Ready - No device connected"

[Screenshot: First launch window showing port selector and connect button]

### Troubleshooting Installation Issues

#### Windows Issues

**Issue: "Windows protected your PC" message**
- **Solution:** Click "More info" → "Run anyway"
- This is normal for new applications not yet widely installed

**Issue: Installer won't run**
- **Solution:** Right-click the installer → "Run as administrator"

**Issue: Application won't start after installation**
- **Solution:** Install .NET Runtime if missing:
  - Download from: https://dotnet.microsoft.com/download
  - Install .NET 6.0 or later

#### macOS Issues

**Issue: "App is damaged and can't be opened"**
- **Solution:** Remove quarantine attribute:
  ```bash
  xattr -cr /Applications/Nova\ System\ Manager.app
  ```

**Issue: MIDI permissions denied**
- **Solution:** Grant MIDI access:
  - Go to System Preferences → Security & Privacy → Privacy
  - Select "Automation" or "MIDI" in the left panel
  - Check the box next to Nova System Manager

#### Linux Issues

**Issue: MIDI port permissions denied**
- **Solution:** Add your user to the audio group:
  ```bash
  sudo usermod -a -G audio $USER
  ```
  Log out and log back in for changes to take effect

**Issue: Application won't start (missing libraries)**
- **Solution:** Install required dependencies:
  ```bash
  # Ubuntu/Debian
  sudo apt-get install libasound2 libavahi-client3
  
  # Fedora
  sudo dnf install alsa-lib avahi
  ```

---

## 3. Getting Started

### Connecting Your Nova System

Follow these steps to connect your Nova System to the computer:

#### Step 1: Physical Connections

1. **Power on your Nova System** pedal
   - Connect power adapter to the pedal
   - Turn on the pedal using the power switch

2. **Connect MIDI Interface to Computer**
   - Plug USB-MIDI interface into a free USB port
   - Wait for operating system to recognize the device (usually automatic)

3. **Connect MIDI Cables**
   - Connect MIDI OUT from computer interface → MIDI IN on Nova System
   - Connect MIDI IN from computer interface → MIDI OUT on Nova System
   - Both connections are required for bidirectional communication

[Screenshot: Diagram showing MIDI connection setup]

> **Important:** Both MIDI In and Out must be connected for full functionality. The Nova System needs to send preset data back to the computer.

#### Step 2: Verify MIDI Connection

1. **Check MIDI Indicator** on your MIDI interface
   - Most interfaces have LED indicators
   - Lights should be steady (not blinking) when idle

2. **Test in Operating System**
   - **Windows:** Open Device Manager → Sound, video and game controllers
   - **macOS:** Open Audio MIDI Setup → MIDI Studio
   - **Linux:** Run `aconnect -l` in terminal
   - Verify your MIDI interface is listed

### Selecting MIDI Port

Once hardware is connected:

1. **Open Nova System Manager**
   - Launch the application if not already running

2. **Locate Port Selector**
   - Find the dropdown menu at the top of the window
   - It will be labeled "Select MIDI Port"

3. **Refresh Port List** (if needed)
   - Click the refresh icon or press `Ctrl+R`
   - This rescans for available MIDI devices

4. **Select Your Device**
   - Click the dropdown to see available MIDI ports
   - Look for entries containing "Nova" or your MIDI interface name
   - Example: "USB MIDI Interface - Port 1"
   - Select the appropriate port from the list

[Screenshot: Port selector dropdown with MIDI devices listed]

> **Tip:** If you have multiple MIDI devices, try each port until you find the one connected to Nova System. The status bar will confirm successful connection.

5. **Click "Connect" Button**
   - Click the blue "Connect" button next to the port selector
   - Button will show "Connecting..." briefly
   - Status bar will update to show connection status

**Expected Results:**
- ✓ Status shows "Connected to Nova System"
- ✓ Connection indicator turns green
- ✓ Download and other features become enabled

**If Connection Fails:**
- Check physical MIDI cable connections
- Verify Nova System is powered on
- Try selecting a different MIDI port
- See [Troubleshooting](#9-troubleshooting) section for more help

### First Download

After successfully connecting, download presets from your Nova System:

1. **Click "Download from Pedal" Button**
   - Located in the main toolbar
   - Or press `F5` keyboard shortcut
   - Button becomes disabled while downloading

2. **Monitor Progress**
   - Progress bar appears showing download status
   - Status message shows "Downloading preset X/60"
   - Download takes approximately 30-60 seconds for full bank

3. **View Downloaded Presets**
   - Presets appear in the main list as they download
   - Each preset shows:
     - Number (00-1 through 19-3)
     - Name (e.g., "Clean Lead", "Heavy Rhythm")
   - Total of 60 user presets will be downloaded

[Screenshot: Download progress with preset list populating]

> **Note:** The first download retrieves all 60 user presets from your Nova System. Factory presets (F0-1 through F9-3) remain on the pedal and cannot be overwritten.

4. **Download Complete**
   - Status bar shows "Download complete - 60 presets received"
   - All presets are now available for viewing and editing
   - Presets are automatically saved to local storage

**What Happens During Download:**
- Application sends SysEx request messages to Nova System
- Nova System responds with preset data for each slot (00-1 to 19-3)
- Each preset includes:
  - Preset name
  - All effect parameters
  - Routing configuration
  - Expression pedal mapping

---

## 4. Main Interface

The Nova System Manager interface is organized into several key areas:

### Connection Panel

Located at the top of the window, the connection panel manages MIDI communication:

**Components:**

1. **Port Selector Dropdown**
   - Shows available MIDI input/output ports
   - Displays device names from operating system
   - Click dropdown arrow to select different port
   - Automatically filters to show compatible devices

2. **Refresh Button** (🔄)
   - Rescans for MIDI devices
   - Use after connecting/disconnecting hardware
   - Keyboard shortcut: `Ctrl+R`

3. **Connect Button**
   - Establishes connection to selected port
   - Changes to "Disconnect" when connected
   - Shows "Connecting..." during connection attempt
   - Color-coded: Blue (ready), Gray (disabled), Green (connected)

4. **Connection Status Indicator**
   - Visual indicator of connection state
   - 🔴 Red circle = Not connected
   - 🟢 Green circle = Connected
   - 🟡 Yellow circle = Connecting...

[Screenshot: Connection panel with labeled components]

### Preset List View

The main area of the interface displays all presets:

**Layout:**

1. **List Columns**
   - **Number:** Preset location (00-1 to 19-3)
   - **Name:** Preset name (up to 24 characters)
   - **Modified:** Indicator (*) if preset has unsaved changes

2. **List Features**
   - Alternating row colors for readability
   - Hover effect highlights row
   - Single-click selects preset
   - Double-click opens preset editor
   - Right-click shows context menu

3. **Context Menu** (Right-click)
   - **Edit Preset** - Open preset editor
   - **Rename Preset** - Change preset name
   - **Copy Preset** - Copy to another slot
   - **Export to File** - Save as .syx file
   - **Delete Preset** - Clear preset slot

[Screenshot: Preset list with context menu open]

**List Navigation:**
- Arrow keys: Move selection up/down
- Page Up/Down: Jump by page
- Home/End: Jump to first/last preset
- Type preset name: Quick search/filter

### System Settings View

Access system-wide configuration:

1. **Click "System Settings" Button** in toolbar
   - Or select "View → System Settings" from menu
   - Or press `Ctrl+,` (Settings shortcut)

2. **Settings Categories**
   - **Global Parameters**
     - Tuner reference frequency
     - MIDI channel
     - FX Mute mode (Soft/Hard spillover)
   
   - **MIDI Configuration**
     - Device ID
     - Program Change send/receive
     - Control Change mappings
   
   - **Expression Pedal**
     - Pedal calibration
     - Response curve
     - Min/Max values
   
   - **Application Settings**
     - Theme (Light/Dark)
     - Auto-connect on startup
     - Backup preferences

[Screenshot: System Settings dialog]

### File Manager

Manage preset files and backups:

1. **Access File Manager**
   - Click "File" menu → "File Manager"
   - Or press `Ctrl+F`

2. **File Manager Features**
   - Browse saved preset files (.syx)
   - View preset file details
   - Import multiple presets
   - Export multiple presets
   - Organize into folders
   - Search by name or tags

3. **File Information Display**
   - File name
   - Date saved
   - Preset count (for bank files)
   - File size
   - Preview preset name

[Screenshot: File Manager window]

---

## 5. Working with Presets

### Downloading Presets from Pedal

Retrieve presets from your Nova System:

**Full Bank Download:**

1. **Connect to Nova System** (see [Getting Started](#3-getting-started))

2. **Click "Download from Pedal"** or press `F5`

3. **Wait for Completion**
   - Progress indicator shows current preset number
   - Takes 30-60 seconds for all 60 presets
   - Can continue working during download

4. **Verify Download**
   - Check preset list is populated
   - Status bar confirms "60 presets downloaded"

**Single Preset Download:**

1. **Select Preset Slot** in list (e.g., 05-2)

2. **Right-click** → **"Download This Preset"**
   - Or press `Shift+F5`

3. **Preset Updates**
   - Selected slot updates with current data from pedal
   - Other presets remain unchanged

> **Note:** Download retrieves the current state from the pedal's memory, including any changes made on the pedal itself.

### Viewing Preset Details

Examine preset configuration without editing:

1. **Select Preset** in list

2. **View Details Panel** (right side of window)
   - Shows effect block states (On/Off)
   - Displays key parameters
   - Shows routing configuration

**Effect Blocks Shown:**
- Drive (Overdrive/Distortion)
- Compressor (Percussive/Sustaining/Advanced)
- EQ + Noise Gate
- Modulation (Chorus/Flanger/Vibrato/Phaser/Tremolo/Panner)
- Pitch (Pitch Shifter/Octaver/Whammy/Detune/Intelligent)
- Delay (Clean/Analog/Tape/Dynamic/Dual/Ping-Pong)
- Reverb (Spring/Hall/Room/Plate)

[Screenshot: Preset details panel showing effect blocks]

### Editing Preset Parameters

Modify preset settings using the preset editor:

1. **Open Preset Editor**
   - Double-click preset in list
   - Or select preset and click "Edit" button
   - Or press `Enter` with preset selected

2. **Preset Editor Layout**
   - **Header Section:**
     - Preset name (editable)
     - Preset number
     - Global parameters (Tap Tempo, Routing)
   
   - **Effect Blocks Section:**
     - Tabbed interface for each effect block
     - Effect type selector
     - Parameter controls (sliders, knobs, dropdowns)
     - On/Off toggle for each block

3. **Editing Parameters**
   
   **Effect Type Selection:**
   - Click effect type dropdown
   - Select from available types (e.g., Chorus → Flanger)
   - Parameters update to match selected type
   
   **Parameter Adjustment:**
   - **Sliders:** Drag to adjust continuous values
   - **Knobs:** Click and drag vertically
   - **Dropdowns:** Select from discrete options
   - **Text fields:** Type numeric values directly
   
   **Fine Control:**
   - Hold `Shift` while dragging for fine adjustment
   - Use arrow keys for step-by-step changes
   - Double-click slider/knob to reset to default

4. **Real-Time Preview** (when connected)
   - Changes sent to pedal immediately
   - Hear results in real-time through your amp
   - Press `Escape` to revert all changes

[Screenshot: Preset editor with effect parameters]

### Understanding Effect Types and Parameters

Each effect block has specific types and parameters. For detailed information on each effect, refer to [EFFECT_REFERENCE.md](../EFFECT_REFERENCE.md).

**Quick Reference:**

**Drive Block:**
- **Types:** Overdrive, Distortion
- **Key Parameters:** Gain, Tone, Level, Boost
- **Note:** Uses analog NDT™ (Nova Drive Technology)

**Compressor Block:**
- **Types:** Percussive, Sustaining, Advanced
- **Percussive/Sustaining:** Drive, Response, Level
- **Advanced:** Threshold, Ratio, Attack, Release, Level

**EQ Block:**
- **Type:** 3-Band Parametric
- **Each Band:** Frequency (41Hz-20kHz), Gain (±12dB), Width (0.3-1.6 octaves)

**Noise Gate:**
- **Parameters:** Mode (Hard/Soft), Threshold, Damp, Release

**Modulation Block:**
- **Types:** Chorus, Flanger, Vibrato, Phaser, Tremolo, Panner
- **Common Parameters:** Speed, Depth, Tempo, Mix
- **Type-Specific:** Feedback, Delay, Hi-Cut, etc.

**Pitch Block:**
- **Types:** Pitch Shifter, Octaver, Whammy, Detune, Intelligent
- **Key Parameters:** Voice pitch (cents), Pan, Delay, Level
- **Intelligent:** Adds Key and Scale parameters

**Delay Block:**
- **Types:** Clean, Analog, Tape, Dynamic, Dual, Ping-Pong
- **Common Parameters:** Time, Tempo, Feedback, Hi/Lo-Cut, Mix
- **Special:** Analog/Tape add Drive; Dynamic adds ducking

**Reverb Block:**
- **Types:** Spring, Hall, Room, Plate
- **Parameters:** Decay, Pre-Delay, Shape, Size, Hi/Lo Color, Mix

> **Tip:** For complete parameter ranges and effect descriptions, see the [EFFECT_REFERENCE.md](../EFFECT_REFERENCE.md) document.

### Saving Changes

After editing a preset:

1. **Save to Local Storage**
   - Click "Save" button in editor
   - Or press `Ctrl+S`
   - Preset marked as modified (*)
   - Changes saved to application database

2. **Upload to Pedal**
   - Click "Upload to Pedal" button
   - Or press `Ctrl+U`
   - Preset sent via MIDI SysEx to Nova System
   - Takes 1-2 seconds per preset

3. **Save and Upload**
   - Click "Save & Upload" button
   - Or press `Ctrl+Shift+S`
   - Performs both actions in sequence

[Screenshot: Preset editor save options]

**Unsaved Changes Indicator:**
- Modified presets show (*) asterisk
- Status bar shows "X unsaved presets"
- Application prompts before closing if unsaved changes exist

---

## 6. System Settings

### Global Settings

Configure system-wide parameters:

1. **Access Global Settings**
   - Menu: "Settings" → "Global Settings"
   - Or press `Ctrl+,`

2. **Available Settings**

   **Audio Settings:**
   - **Output Level (Left/Right):** -100 to 0 dB
   - **Boost Max:** 0-10 dB (maximum boost amount)
   - **FX Mute Mode:**
     - Soft: Delays/reverb ring out on preset change
     - Hard: Immediate silence on preset change

   **Tuner:**
   - **Reference Frequency:** 435-445 Hz (default: 440 Hz)
   - Adjust for different tuning standards

   **MIDI Settings:**
   - **Device ID:** 0-126 or All (127)
   - **MIDI Channel:** 1-16 or Omni
   - **Program Change Send:** On/Off
   - **Program Change Receive:** On/Off

   **Pedal Settings:**
   - **Tap Master Mode:**
     - Preset: Uses stored delay time on preset change
     - Global: Uses current tapped tempo on preset change
   
   - **Boost Lock:** On/Off
     - When On: Boost state persists across preset changes
     - When Off: Boost state per-preset

[Screenshot: Global Settings dialog]

### MIDI Mapping Configuration

Assign MIDI Control Change (CC) messages to functions:

1. **Open MIDI Mapping**
   - Menu: "Settings" → "MIDI Mapping"
   - Or click "MIDI Map" button in toolbar

2. **Mapping Functions**

   **Available Assignments:**
   - Tap Tempo
   - Drive On/Off
   - Compressor On/Off
   - Noise Gate On/Off
   - EQ On/Off
   - Boost On/Off
   - Modulation On/Off
   - Pitch On/Off
   - Delay On/Off
   - Reverb On/Off
   - Expression Pedal

3. **Assign CC Number**
   - Select function from list
   - Choose CC number (0-127)
   - Click "Assign" button
   - Or use "Learn" mode (see below)

4. **CC Learn Mode**
   - Click "Learn" button next to function
   - Move MIDI controller (pedal, knob, button)
   - Application automatically detects and assigns CC number
   - Click "Stop Learning" when done

5. **Test Mapping**
   - Move MIDI controller
   - Watch for activity indicator in mapping list
   - Verify function responds correctly

[Screenshot: MIDI Mapping configuration]

### Expression Pedal Setup

Configure expression pedal behavior:

**Pedal Connection:**
- Connect expression pedal to Nova System "Pedal" input (1/4" TRS jack)
- Supported pedals: any with 10k-100k linear potentiometer
- Examples: Roland EV-5, Boss FV-500L, M-Audio EX-P

**Calibration Procedure:**

1. **Open Expression Pedal Settings**
   - Menu: "Settings" → "Expression Pedal"

2. **Start Calibration**
   - Click "Calibrate Pedal" button
   - Follow on-screen instructions

3. **Calibration Steps**
   
   **Step 1: Minimum Position**
   - Move pedal to heel-down (minimum) position
   - Hold steady
   - Click "Capture Min" button
   - Application records minimum voltage value

   **Step 2: Maximum Position**
   - Move pedal to toe-down (maximum) position
   - Hold steady
   - Click "Capture Max" button
   - Application records maximum voltage value

   **Step 3: Verify Range**
   - Move pedal through full range
   - Watch value indicator (should sweep 0-100%)
   - If range is incorrect, repeat calibration

4. **Save Calibration**
   - Click "Save" button
   - Calibration stored in system settings
   - Automatically applied on subsequent connections

[Screenshot: Expression pedal calibration dialog]

**Pedal Mapping:**

Configure which parameter(s) the pedal controls:

1. **Open Preset Editor** for desired preset

2. **Locate Pedal Assignment Section**

3. **Select Parameter to Control**
   - Click "Assign" button
   - Choose from available parameters:
     - Whammy pitch (automatic in Whammy mode)
     - Volume
     - Any effect parameter (Speed, Depth, Mix, etc.)

4. **Configure Response Curve**
   - **Min Value:** Parameter value at heel-down (0%)
   - **Mid Value:** Parameter value at mid-point (50%)
   - **Max Value:** Parameter value at toe-down (100%)
   - Adjust these to create linear or curved response

5. **Test Pedal Response**
   - Move pedal while connected to Nova System
   - Listen for parameter changes
   - Adjust curve if response feels unnatural

**Expression Pedal Modes:**

- **Preset Mode:**
  - Each preset remembers pedal assignment
  - Changing presets updates pedal function
  - Pedal position jumps to stored value

- **Pedal Mode:**
  - Pedal maintains current position across presets
  - Useful for master volume control
  - Set via "Tap Master" setting

---

## 7. File Operations

### Exporting Presets to File

Save presets as SysEx (.syx) files:

**Export Single Preset:**

1. **Select Preset** in list

2. **Right-click** → **"Export Preset"**
   - Or Menu: "File" → "Export Preset"
   - Or press `Ctrl+E`

3. **Choose Save Location**
   - File dialog opens
   - Navigate to desired folder
   - Enter filename (e.g., "My Clean Tone.syx")
   - Click "Save"

4. **Export Complete**
   - Status bar shows "Preset exported successfully"
   - File contains full preset SysEx dump (520 bytes)

**Export Multiple Presets:**

1. **Select Presets** (Ctrl+Click or Shift+Click)

2. **Right-click** → **"Export Selected Presets"**

3. **Choose Export Method**
   - **Individual Files:** Each preset as separate .syx file
   - **Single Bank File:** Combined into one .syx file

4. **Choose Save Location and Export**

**Export Full Bank:**

1. **Menu: "File" → "Export Bank"**
   - Or press `Ctrl+Shift+E`

2. **Choose Save Location**
   - Navigate to desired folder
   - Enter filename (e.g., "My Nova Bank.syx")
   - Click "Save"

3. **Export Progress**
   - Progress bar shows export status
   - File contains all 60 user presets (~31 KB)

[Screenshot: Export dialog with file name entry]

### Importing Presets from File

Load presets from SysEx (.syx) files:

**Import Single Preset:**

1. **Menu: "File" → "Import Preset"**
   - Or press `Ctrl+I`

2. **Select File**
   - File dialog opens
   - Navigate to .syx file location
   - Select preset file
   - Click "Open"

3. **Choose Destination Slot**
   - Dialog shows preset details (name, effects)
   - Select target slot (00-1 to 19-3)
   - Click "Import"

4. **Confirm Overwrite** (if slot occupied)
   - Application shows warning
   - Click "Yes" to replace existing preset
   - Click "No" to choose different slot
   - Click "Cancel" to abort import

**Import Bank:**

1. **Menu: "File" → "Import Bank"**
   - Or press `Ctrl+Shift+I`

2. **Select Bank File**
   - Choose .syx file containing multiple presets
   - Bank files are typically ~31 KB (60 presets)

3. **Import Options**
   - **Replace All:** Overwrites all 60 presets
   - **Merge:** Import only to empty slots
   - **Select Range:** Choose which slots to import

4. **Confirm and Import**
   - Review import summary
   - Click "Import" to proceed
   - Progress bar shows import status

**Import from Legacy NovaManager:**

Nova System Manager is fully compatible with .syx files from the original NovaManager application:

1. **Locate NovaManager Files**
   - Typically in `Documents\TC Electronic\Nova System\`
   - Files end with `.syx` extension

2. **Import Using Standard Procedure**
   - Use "File" → "Import Preset" or "Import Bank"
   - Select NovaManager .syx file
   - Import proceeds normally

> **Note:** All Nova System .syx files follow the same format, ensuring compatibility between old and new software.

[Screenshot: Import preset dialog showing file selection]

### Backup and Restore

Protect your presets with regular backups:

**Create Backup:**

1. **Menu: "File" → "Backup Bank"**
   - Or press `Ctrl+B`

2. **Choose Backup Location**
   - Select folder for backup file
   - Default filename includes date/time stamp
   - Example: `Nova_Backup_2026-02-03_1430.syx`

3. **Backup Options**
   - **Include System Settings:** Save global configuration
   - **Compress:** Create smaller backup file (ZIP format)
   - **Cloud Sync:** Upload to cloud storage (if configured)

4. **Create Backup**
   - Click "Backup" button
   - Progress bar shows backup progress
   - Status confirms "Backup complete"

**Automatic Backups:**

Configure automatic backup schedule:

1. **Menu: "Settings" → "Preferences"**

2. **Backup Settings**
   - **Auto-Backup:** Enable/Disable
   - **Frequency:** Daily, Weekly, Monthly
   - **Keep Last:** Number of backups to retain (1-10)
   - **Backup Location:** Choose folder

3. **Save Preferences**
   - Automatic backups run in background
   - No user interaction required

**Restore from Backup:**

1. **Menu: "File" → "Restore from Backup"**

2. **Select Backup File**
   - Navigate to backup location
   - Select .syx or .zip backup file
   - Click "Open"

3. **Review Backup Contents**
   - Application displays backup information:
     - Backup date/time
     - Number of presets
     - System settings included (yes/no)

4. **Restore Options**
   - **Full Restore:** Replace all presets and settings
   - **Presets Only:** Restore presets, keep current settings
   - **Settings Only:** Restore settings, keep current presets

5. **Confirm Restoration**
   - Click "Restore" button
   - Application performs restoration
   - Restart required if system settings changed

> **Important:** Always create a backup before making major changes to your preset bank. Restoration overwrites current data.

[Screenshot: Backup dialog with options]

---

## 8. Keyboard Shortcuts

Speed up your workflow with keyboard shortcuts:

### Global Shortcuts

| Shortcut | Action | Description |
|----------|--------|-------------|
| `Ctrl+R` | Refresh MIDI ports | Rescan for MIDI devices |
| `Ctrl+,` | Settings | Open application settings |
| `Ctrl+Q` | Quit | Exit application |
| `F1` | Help | Open user manual |
| `F11` | Full screen | Toggle full screen mode |

### Connection Shortcuts

| Shortcut | Action | Description |
|----------|--------|-------------|
| `Ctrl+K` | Connect/Disconnect | Toggle MIDI connection |
| `F5` | Download from pedal | Download all presets |
| `Shift+F5` | Download selected | Download single preset |
| `Ctrl+U` | Upload to pedal | Upload selected preset |
| `Ctrl+Shift+U` | Upload all | Upload all modified presets |

### Preset Management

| Shortcut | Action | Description |
|----------|--------|-------------|
| `Enter` | Edit preset | Open preset editor |
| `F2` | Rename preset | Rename selected preset |
| `Ctrl+D` | Duplicate preset | Copy preset to new slot |
| `Delete` | Clear preset | Delete selected preset |
| `Ctrl+C` | Copy preset | Copy preset to clipboard |
| `Ctrl+V` | Paste preset | Paste preset from clipboard |
| `Ctrl+Z` | Undo | Undo last change |
| `Ctrl+Y` | Redo | Redo last undone change |

### File Operations

| Shortcut | Action | Description |
|----------|--------|-------------|
| `Ctrl+S` | Save | Save current changes |
| `Ctrl+Shift+S` | Save all | Save all modified presets |
| `Ctrl+E` | Export preset | Export selected preset to file |
| `Ctrl+Shift+E` | Export bank | Export all presets to file |
| `Ctrl+I` | Import preset | Import preset from file |
| `Ctrl+Shift+I` | Import bank | Import bank from file |
| `Ctrl+B` | Backup | Create backup of all presets |

### Preset Editor

| Shortcut | Action | Description |
|----------|--------|-------------|
| `Ctrl+S` | Save preset | Save changes to preset |
| `Escape` | Cancel | Close editor without saving |
| `Tab` | Next field | Move to next parameter |
| `Shift+Tab` | Previous field | Move to previous parameter |
| `Ctrl+Enter` | Save and close | Save preset and close editor |
| `Ctrl+T` | Toggle effect | Enable/disable current effect block |
| `Space` | Play/Audition | Preview preset (when connected) |

### List Navigation

| Shortcut | Action | Description |
|----------|--------|-------------|
| `↑` `↓` | Navigate list | Move selection up/down |
| `Page Up` `Page Down` | Jump page | Move by one page |
| `Home` | First preset | Jump to first preset |
| `End` | Last preset | Jump to last preset |
| `Ctrl+F` | Find preset | Open search/filter |
| `Ctrl+A` | Select all | Select all presets |
| `Ctrl+Click` | Multi-select | Add/remove from selection |
| `Shift+Click` | Range select | Select range of presets |

### View Shortcuts

| Shortcut | Action | Description |
|----------|--------|-------------|
| `Ctrl+1` | Preset list view | Show preset list |
| `Ctrl+2` | System settings view | Show system settings |
| `Ctrl+3` | File manager view | Show file manager |
| `Ctrl+L` | Toggle theme | Switch light/dark theme |
| `Ctrl+Plus` | Zoom in | Increase UI scale |
| `Ctrl+Minus` | Zoom out | Decrease UI scale |
| `Ctrl+0` | Reset zoom | Reset UI scale to 100% |

> **Tip:** You can customize keyboard shortcuts in Settings → Keyboard Shortcuts.

---

## 9. Troubleshooting

### MIDI Connection Issues

**Problem: Cannot connect to Nova System**

**Symptoms:**
- "Connection failed" error message
- "No response from device" error
- Connection button stays in "Connecting..." state

**Solutions:**

1. **Verify Physical Connections**
   - Check both MIDI In and Out cables are connected
   - Ensure cables are firmly seated in jacks
   - Try different MIDI cables (cables can fail)
   - Verify MIDI interface is connected to computer via USB

2. **Check Nova System Power**
   - Ensure pedal is powered on
   - Check power adapter connection
   - Verify power LED is lit on pedal

3. **Verify MIDI Interface**
   - Check MIDI interface is recognized by operating system
   - Windows: Device Manager → Sound, video and game controllers
   - macOS: Audio MIDI Setup → MIDI Studio
   - Linux: Run `aconnect -l` in terminal

4. **Test MIDI Interface**
   - Try connecting different MIDI device
   - If other devices work, issue is with Nova System
   - If no devices work, issue is with MIDI interface

5. **Check Device ID**
   - Nova System default Device ID is 0
   - Application default is "All" (127) or 0
   - Verify settings match in Settings → MIDI Configuration

6. **Restart Everything**
   - Close application
   - Power cycle Nova System (off/on)
   - Disconnect/reconnect MIDI interface
   - Relaunch application
   - Try connecting again

### "No MIDI Ports Found"

**Symptoms:**
- Port selector dropdown is empty
- "No MIDI devices detected" message
- Refresh button doesn't help

**Solutions:**

1. **Check MIDI Interface Driver**
   - Some interfaces require drivers
   - Visit manufacturer website for latest drivers
   - Install drivers and restart computer

2. **Verify USB Connection**
   - Try different USB port
   - Avoid USB hubs (connect directly to computer)
   - Check USB cable is data-capable (not power-only)

3. **Operating System Permissions**
   
   **Windows:**
   - Check Windows privacy settings
   - Settings → Privacy → Microphone/Audio
   
   **macOS:**
   - System Preferences → Security & Privacy → Privacy
   - Check "Automation" and "MIDI" permissions
   
   **Linux:**
   - Add user to audio group: `sudo usermod -a -G audio $USER`
   - Log out and log back in

4. **Firewall/Antivirus**
   - Some security software blocks MIDI communication
   - Add Nova System Manager to allowed applications list

5. **Check MIDI Service**
   
   **Windows:**
   - Open Services (services.msc)
   - Verify "Windows Audio" service is running
   
   **macOS:**
   - Check Core MIDI server is running
   - Force restart: `sudo launchctl stop com.apple.audio.coreaudiod`

### Upload Failures

**Problem: Preset fails to upload to pedal**

**Symptoms:**
- "Upload failed" error
- "No acknowledgment from device" error
- Upload timeout

**Solutions:**

1. **Verify Connection**
   - Ensure connection is active (green indicator)
   - Status bar shows "Connected to Nova System"
   - Try downloading a preset first (verifies bidirectional communication)

2. **Check Preset Slot**
   - Factory presets (F0-1 to F9-3) cannot be overwritten
   - Only user presets (00-1 to 19-3) are writable
   - Select a user preset slot before uploading

3. **MIDI Buffer Overflow**
   - Wait 2-3 seconds between uploads
   - Large number of rapid uploads can overwhelm MIDI buffer
   - Upload one at a time if multiple uploads fail

4. **Preset Data Corruption**
   - Try re-editing preset
   - Verify all parameters are within valid ranges
   - Export and re-import preset to validate SysEx data

5. **Firmware Compatibility**
   - Nova System firmware v1.2 or later recommended
   - Check firmware version: Utility menu on pedal
   - Update firmware if necessary (see TC Electronic website)

### Common Error Messages

**Error: "Invalid SysEx data"**
- **Cause:** Corrupted preset file or invalid parameter values
- **Solution:** 
  - Try importing different preset file
  - Re-download preset from pedal
  - Check file size (should be ~520 bytes for single preset)

**Error: "Checksum mismatch"**
- **Cause:** Data corruption during transmission
- **Solution:**
  - Retry download/upload
  - Check MIDI cables for damage
  - Reduce MIDI cable length (keep under 15 feet)

**Error: "Device timeout"**
- **Cause:** Nova System not responding within expected time
- **Solution:**
  - Verify pedal is not in Tuner or Edit mode
  - Wait for any preset change to complete on pedal
  - Increase timeout in Settings → Advanced

**Error: "Port already in use"**
- **Cause:** Another application is using MIDI port
- **Solution:**
  - Close other MIDI applications
  - Disconnect and reconnect MIDI interface
  - Restart computer if problem persists

**Error: "Unsupported firmware version"**
- **Cause:** Nova System firmware is too old
- **Solution:**
  - Update firmware to v1.2 or later
  - Download from TC Electronic support website
  - Follow firmware update procedure in Nova System manual

### Download/Connection Hangs

**Problem: Application becomes unresponsive during operations**

**Solutions:**

1. **Force Timeout**
   - Wait 30 seconds for automatic timeout
   - Click "Cancel" button if available
   - Press `Escape` key to abort operation

2. **Close and Restart**
   - Close application (Ctrl+Q or Alt+F4)
   - If frozen, force quit:
     - Windows: Task Manager → End Task
     - macOS: Force Quit (Cmd+Option+Esc)
     - Linux: `killall nova-system-manager`
   - Restart application

3. **Check System Resources**
   - Verify sufficient RAM available
   - Close other applications to free memory
   - Check CPU usage (Task Manager/Activity Monitor)

4. **Reduce Concurrent Operations**
   - Don't start new operations while download in progress
   - Wait for each operation to complete
   - Disable auto-save during long operations

### Audio/Tone Issues

**Problem: Preset sounds different than expected**

> **Note:** Nova System Manager does not process audio. All sound is generated by the Nova System pedal itself.

**If Preset Sounds Wrong:**

1. **Verify Preset Upload**
   - Ensure preset was successfully uploaded to pedal
   - Check status bar for "Upload complete" confirmation
   - Try uploading again

2. **Check Pedal State**
   - Verify you're on correct preset (check pedal display)
   - Ensure correct input selected (Drive vs Line)
   - Check global volume levels aren't muted

3. **Expression Pedal Position**
   - Expression pedal position affects sound
   - If pedal controls volume, check position
   - Try re-calibrating expression pedal

4. **Compare with Original**
   - Download preset from pedal
   - Compare with version in application
   - Check for parameter differences

### Getting Further Help

If problems persist after trying these solutions:

**Check Application Logs:**
1. Menu: "Help" → "Open Log Folder"
2. View recent log file for error details
3. Share log file when requesting support

**Contact Support:**
- Email: support@novasystemmanager.com
- Forum: https://forum.novasystemmanager.com
- GitHub Issues: https://github.com/novasystemmanager/issues

**Include This Information:**
- Application version (Help → About)
- Operating system and version
- MIDI interface model
- Nova System firmware version
- Description of problem
- Steps to reproduce issue
- Error messages (exact text)
- Log files (if available)

---

## 10. Technical Reference

### MIDI Protocol

Nova System Manager uses standard MIDI communication protocol. For complete technical details, see [MIDI_PROTOCOL.md](../MIDI_PROTOCOL.md).

**Quick Overview:**

**MIDI Messages Used:**
- **Program Change (PC):** Preset selection (0-126)
- **Control Change (CC):** Real-time parameter control
- **System Exclusive (SysEx):** Preset/system dumps and requests

**SysEx Message Structure:**
```
F0 00 20 1F [DeviceID] 63 [MessageType] [DataType] [Data...] [Checksum] F7
```

**Header Breakdown:**
- `F0` = SysEx start
- `00 20 1F` = TC Electronic manufacturer ID
- `[DeviceID]` = 0-126 or 127 (All)
- `63` = Nova System model ID
- `[MessageType]` = 20 (Dump) or 45 (Request)
- `[DataType]` = 01 (Preset), 02 (System), 03 (Bank)

**Message Types:**

1. **Preset Dump (520 bytes)**
   - Contains single preset data
   - Includes name, all parameters, routing
   - Sent from pedal in response to request

2. **User Bank Dump (~31 KB)**
   - Contains all 60 user presets
   - Sent as single continuous SysEx message
   - Used for full backup/restore

3. **System Dump (~1 KB)**
   - Contains global settings
   - MIDI CC mappings
   - Program map configuration

**Bidirectional Communication:**
- Both MIDI In and Out required
- Application sends requests via MIDI Out
- Nova System responds via MIDI In
- Real-time parameter changes sent immediately

> **Note:** For detailed byte-level protocol specification, see [MIDI_PROTOCOL.md](../MIDI_PROTOCOL.md).

### SysEx Format

For complete SysEx format details, see [docs/06-sysex-formats.md](06-sysex-formats.md).

**Preset SysEx Structure:**

```
Offset  Length  Description
------  ------  -----------
0       1       F0 (SysEx start)
1-3     3       00 20 1F (TC Electronic ID)
4       1       Device ID (0-126 or 127)
5       1       63 (Nova System model)
6       1       20 (Dump message)
7       1       01 (Preset data type)
8       1       Preset number (01-5A)
9       1       Reserved
10-33   24      Preset name (ASCII, padded with 0x00)
34-517  484     Effect block parameters
518     1       Checksum
519     1       F7 (SysEx end)
```

**Parameter Encoding:**

Nova System uses 4-byte nibble encoding for all parameters:
```
Value = (b4 << 21) | (b3 << 14) | (b2 << 7) | b1
```

Each byte contains only 7 bits of data (0-127), allowing safe transmission via MIDI.

**Checksum Calculation:**
```
sum = 0
for byte in data[34:518]:
    sum += byte
checksum = sum & 0x7F  // Keep only 7 LSBs
```

**Effect Block Offsets:**

| Block | Offset Range | Description |
|-------|-------------|-------------|
| Global | 34-69 | Tap tempo, routing, output levels |
| Compressor | 70-133 | Comp type and parameters |
| Drive | 134-197 | Overdrive/distortion settings |
| Modulation | 198-261 | Chorus, flanger, etc. |
| Delay | 262-325 | Delay type and parameters |
| Reverb | 326-389 | Reverb type and parameters |
| EQ + Gate | 390-453 | EQ bands and noise gate |
| Pitch | 454-517 | Pitch shifter settings |

> **Note:** For complete parameter offset map and value ranges, see [docs/06-sysex-formats.md](06-sysex-formats.md) and [MIDI_PROTOCOL.md](../MIDI_PROTOCOL.md).

### Preset File Format

Nova System Manager uses standard .syx (SysEx) file format:

**Single Preset File (.syx):**
- Size: 520 bytes
- Format: Raw SysEx dump
- Compatible with NovaManager and other Nova System software
- Can be shared between users

**Bank File (.syx):**
- Size: ~31,200 bytes (60 × 520)
- Format: Concatenated preset dumps
- Single checksum for entire bank
- Includes all user presets (00-1 to 19-3)

**File Compatibility:**
- ✓ Import/export with original NovaManager
- ✓ Share presets via email, forums, cloud storage
- ✓ Cross-platform compatible (Windows/Mac/Linux)
- ✓ Backward compatible with all Nova System firmware versions

### Effect Parameter Ranges

For complete effect parameter information, see [EFFECT_REFERENCE.md](../EFFECT_REFERENCE.md).

**Common Parameter Types:**

| Parameter Type | Range | Units | Description |
|---------------|-------|-------|-------------|
| Percentage | 0-100 | % | Mix, depth, feedback |
| Level | -100 to 0 | dB | Output levels |
| Time | 0-1800 | ms | Delay times |
| Frequency | 41-20000 | Hz | EQ, filter frequencies |
| Pitch | -1200 to +1200 | cents | Pitch shift (100 cents = 1 semitone) |
| Speed | 0.05-20 | Hz | LFO rates |

**Tempo Sync Values:**
- Ignore (use fixed time)
- 2 bars
- 1 bar
- 1/2, 1/2T (triplet)
- 1/4, 1/4D (dotted), 1/4T
- 1/8, 1/8D, 1/8T
- 1/16, 1/16D, 1/16T
- 1/32, 1/32D, 1/32T

**Routing Options:**
- **Semi-Parallel:** Default, delays/reverb in parallel
- **Serial:** All effects in series
- **Parallel:** All effects mixed in parallel

### Application Architecture

Nova System Manager is built using modern, maintainable architecture:

**Technology Stack:**
- **UI Framework:** Avalonia (cross-platform XAML)
- **Language:** C# (.NET 6.0+)
- **MIDI Library:** DryWetMIDI
- **Architecture:** Clean Architecture with MVVM

**Project Structure:**
```
src/
├── Nova.Common/          # Shared utilities
├── Nova.Domain/          # Business logic
├── Nova.Midi/            # MIDI abstraction
├── Nova.Infrastructure/  # MIDI implementation
├── Nova.Application/     # Use cases
└── Nova.Presentation/    # Avalonia UI
```

**Data Flow:**
1. User interaction in Presentation layer (XAML/ViewModel)
2. Command/Query sent to Application layer (Use Cases)
3. Use Case calls Domain services
4. Domain services use MIDI abstraction (Nova.Midi)
5. MIDI messages sent via Infrastructure layer (DryWetMIDI)
6. Nova System responds via MIDI
7. Response processed back through layers to UI

**Testing:**
- Unit tests for all business logic
- Integration tests for MIDI communication
- UI tests for critical user flows
- Hardware tests with physical Nova System (optional)

### Performance Specifications

**MIDI Communication:**
- Preset download: ~500ms per preset
- Full bank download: 30-60 seconds
- Preset upload: 1-2 seconds per preset
- Parameter change latency: <50ms

**Application Performance:**
- Startup time: <2 seconds
- UI responsiveness: <100ms for all actions
- Memory usage: ~50-100 MB typical
- CPU usage: <5% idle, <20% during operations

**File Operations:**
- Preset export: <100ms
- Bank export: <1 second
- Preset import: <100ms
- Bank import: 1-2 seconds

### Compatibility

**Hardware Compatibility:**
- TC Electronic Nova System (all revisions)
- Firmware v1.0 or later (v1.2+ recommended)
- Any standard MIDI interface (USB-MIDI, 5-pin DIN)

**Software Compatibility:**
- **Preset Files:** Compatible with NovaManager v1.20.1
- **Operating Systems:**
  - Windows 10/11 (x64)
  - macOS 10.15+ (Catalina or later)
  - Linux (Ubuntu 20.04+, Fedora 33+, other distros)

**Known Limitations:**
- Cannot modify factory presets (F0-1 to F9-3)
- Expression pedal calibration requires physical pedal
- Real-time preview requires active MIDI connection
- Some MIDI interfaces have slower communication speeds

---

## Appendix A: Quick Reference Card

**Essential Shortcuts:**
- `F5` - Download from pedal
- `Ctrl+S` - Save preset
- `Ctrl+E` - Export preset
- `Ctrl+I` - Import preset
- `Enter` - Edit preset
- `Escape` - Cancel operation

**Connection Steps:**
1. Connect MIDI cables (both In and Out)
2. Power on Nova System
3. Select MIDI port in dropdown
4. Click "Connect"
5. Press F5 to download

**Common Tasks:**
- **Edit Preset:** Double-click in list
- **Rename:** Select and press F2
- **Backup:** Ctrl+B
- **Upload to Pedal:** Ctrl+U

**Troubleshooting Quick Fixes:**
- Connection failed → Check cables, power cycle
- No MIDI ports → Refresh (Ctrl+R), check drivers
- Upload failed → Verify connection, retry
- Strange sound → Re-upload preset

---

## Appendix B: Resources

**Documentation:**
- [EFFECT_REFERENCE.md](../EFFECT_REFERENCE.md) - Complete effect parameter guide
- [MIDI_PROTOCOL.md](../MIDI_PROTOCOL.md) - MIDI protocol specification
- [docs/06-sysex-formats.md](06-sysex-formats.md) - SysEx format details

**Online Resources:**
- Official Website: https://novasystemmanager.com
- User Forum: https://forum.novasystemmanager.com
- GitHub Repository: https://github.com/novasystemmanager
- Video Tutorials: https://youtube.com/novasystemmanager

**TC Electronic Resources:**
- Nova System Manual: Available from TC Electronic website
- Firmware Updates: https://www.tcelectronic.com/support
- Nova System Support: https://www.tcelectronic.com/support/nova-system

**Community:**
- Preset Sharing: https://presets.novasystemmanager.com
- User Contributions: GitHub Discussions
- Bug Reports: GitHub Issues

---

## Appendix C: Glossary

**Bank:** Collection of 60 user presets (00-1 to 19-3)

**CC (Control Change):** MIDI message type for real-time parameter control

**Checksum:** Error-detection value ensuring data integrity

**Device ID:** MIDI identifier (0-126) allowing multiple devices on one MIDI chain

**Expression Pedal:** Foot-controlled analog pedal for real-time parameter changes

**Factory Preset:** Read-only presets (F0-1 to F9-3) stored in Nova System ROM

**MIDI:** Musical Instrument Digital Interface - standard protocol for music devices

**Nibble:** 4-bit data unit; Nova System uses nibble encoding for parameters

**Preset:** Complete configuration of all effects and parameters

**Program Change (PC):** MIDI message for selecting presets (0-126)

**Routing:** Signal path configuration (Serial, Semi-Parallel, Parallel)

**Spillover:** Effect tails (delay/reverb) continuing after preset change

**SysEx (System Exclusive):** MIDI message type for device-specific data

**User Preset:** Writable presets (00-1 to 19-3) stored in Nova System RAM

---

**End of User Manual**

For additional support, visit https://novasystemmanager.com/support

Document version 1.0 - February 3, 2026
