# TC Electronic Nova System - MIDI Protocol Specification

**Based on:** Nova System Sysex Map.pdf (reverse-engineered from original documentation)

## Overview

The Nova System uses standard MIDI for bidirectional communication:
- **Program Change**: Preset selection (0-126)
- **Control Change**: Real-time parameter control
- **SysEx**: Full preset/system dumps and requests

---

## SysEx Message Structure

### Universal Format
```
F0 [Manufacturer] [DeviceID] [Model] [MessageID] [DataType] [Data...] [Checksum] F7
```

### Header Bytes
| Byte Position | Hex Value | Description |
|---------------|-----------|-------------|
| 0 | F0 | SysEx Start |
| 1-3 | 00 20 1F | TC Electronic Manufacturer ID |
| 4 | 00-7F / All | Device ID (0-126 or broadcast) |
| 5 | 63 | Model ID: Nova System |
| 6 | 20 / 45 | Message Type (20=Dump, 45=Request) |
| 7 | 01 / 02 | Data Type (01=Preset, 02=System) |

---

## Preset Dump Format (519 bytes data + header/footer)

### Complete Preset SysEx Structure
**Total Length:** 520 bytes (0x208 hex)

```
Offset  Hex    Len  Value        Description
------  -----  ---  -----------  ----------------------------------
0       0x000  1    F0           SysEx Begin
1       0x001  3    00 20 1F     TC Electronic Manufacturer ID
4       0x004  1    00-7F        Device ID
5       0x005  1    63           Model ID: Nova System
6       0x006  1    20           Message ID: Dump
7       0x007  1    01           Data Type: Preset
8       0x008  1    01-5A        Preset Number (1-30 Factory, 31-90 User)
9       0x009  1    (void)       Reserved
10      0x00A  24   ASCII        Preset Name (24 characters max)
34      0x022  4    64 00 01 00  Tap Tempo (100-3000 ms, 1ms resolution)
38      0x026  4    00-02        Routing (0=Semi-par, 1=Serial, 2=Parallel)
...     ...    ...  ...          [Effect block parameters]
518     0x206  1    00-7F        Checksum (7 LSB of sum from byte 34-517)
519     0x207  1    F7           SysEx End
```

### Checksum Calculation
```
sum = 0
for byte in data[34:518]:  # From offset 0x022 to 0x205
    sum += byte
checksum = sum & 0x7F  # Keep only 7 least significant bits
```

---

## Effect Block Parameter Map

All effect parameters use **4-byte encoding** in SysEx. Parameter offsets are relative to byte 34 (0x022).

### Nibble Encoding for Signed Values
TC uses nibble encoding for values that need more than 7 bits:
- **Negative values**: Encoded as 4 bytes (e.g., -20 dB = `6C 7F 7F 0F`)
- **Positive values**: Standard or 4-byte format depending on range

**Example Decoding:**
```python
def decode_nibble(b1, b2, b3, b4):
    value = (b4 << 21) | (b3 << 14) | (b2 << 7) | b1
    if value > 0x7FFFFFF:  # Negative value
        value = value - 0x10000000
    return value
```

---

## Global Parameters (Bytes 34-69)

| Offset | Hex  | Len | Range | Description |
|--------|------|-----|-------|-------------|
| 34-37 | 0x022 | 4 | 100-3000 | **Tap Tempo** (milliseconds) |
| 38-41 | 0x026 | 4 | 0-2 | **Routing** (0=Semi-parallel, 1=Serial, 2=Parallel) |
| 42-45 | 0x02A | 4 | -100 to 0 dB | **Output Level Left** |
| 46-49 | 0x02E | 4 | -100 to 0 dB | **Output Level Right** |
| 54-57 | 0x036 | 4 | 0-??? | **Map Parameter** (pedal assignment) |
| 58-61 | 0x03A | 4 | 0-100% | **Map Min** (pedal minimum) |
| 62-65 | 0x03E | 4 | 0-100% | **Map Mid** (pedal midpoint) |
| 66-69 | 0x042 | 4 | 0-100% | **Map Max** (pedal maximum) |

---

## Effect Block Definitions

### 1. COMPRESSOR (Bytes 70-133)

**Type Offset:** 70-73 (0x046)
- `00` = Percussive
- `01` = Sustaining  
- `02` = Advanced

#### Percussive/Sustaining Mode
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 90-93 | Response | 1-10 | Release time (lower = more audible compression) |
| 94-97 | Drive | 1-20 | Threshold+Ratio combined |
| 98-101 | Level | -12 to +12 dB | Output level adjustment |

#### Advanced Mode
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 74-77 | Threshold | -30 to 0 dB | Activation point |
| 78-81 | Ratio | 0-15 | 0=Off, 1=1.12:1, ..., 15=Infinite:1 |
| 82-85 | Attack | 0-16 | 0=0.3ms, 16=140ms (see lookup table) |
| 86-89 | Release | 13-23 | 13=50ms, 23=2000ms (see lookup table) |
| 98-101 | Level | -12 to +12 dB | Output level adjustment |

**On/Off:** Byte 130-133

---

### 2. DRIVE (Bytes 134-197)

**Type Offset:** 134-137 (0x086)
- `00` = Overdrive
- `01` = Distortion

| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 138-141 | Gain | 0-100% | Distortion/overdrive amount |
| 142-145 | Tone | 0-100% | High-frequency content |
| 182-185 | Boost Level | 0-10 dB | Boost amount (within Boost Max) |
| 186-189 | Boost On/Off | 0-1 | Boost enable |
| 190-193 | Drive Level | -100 to 0 dB | Output level |
| 194-197 | Drive On/Off | 0-1 | Drive enable |

**Note:** Drive uses analog NDT™ (Nova Drive Technology) - 100% analog circuit with digital control

---

### 3. MODULATION (Bytes 198-261)

**Type Offset:** 198-201 (0x0C6)
- `00` = Chorus
- `01` = Flanger
- `02` = Vibrato
- `03` = Phaser
- `04` = Tremolo
- `05` = Panner

#### Common Parameters
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 202-205 | Speed | 0x00-0x50 0x01 | 0.050 Hz to 20 Hz (lookup table) |
| 206-209 | Depth | 0-100% | Modulation intensity |
| 210-213 | Tempo | 0-16 | 0=Ignore, 1=2 bars, 16=1/32T |
| 214-217 | Hi-Cut | 0-61 | 20 Hz to 20 kHz (lookup table) |
| 218-221 | Feedback | -100 to +100% | Flanger/Phaser resonance |
| 222-225 | Delay/Range | 0-50ms / Low-High | Chorus/Flanger delay OR Phaser range |
| 238-241 | Width | 0-100% | Tremolo pulse width |
| 250-253 | Mix | 0-100% | Dry/wet balance |
| 258-261 | Mod On/Off | 0-1 | Effect enable |

---

### 4. DELAY (Bytes 262-325)

**Type Offset:** 262-265 (0x106)
- `00` = Clean
- `01` = Analog
- `02` = Tape
- `03` = Dynamic
- `04` = Dual
- `05` = Ping-Pong

#### Common Parameters
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 266-269 | Delay Time | 0-1800 ms | Primary delay time |
| 270-273 | Delay 2 | 0-1800 ms | Dual delay: second tap |
| 274-277 | Tempo | 0-16 | 0=Ignore, 1=2 bars, 16=1/32T |
| 278-281 | Tempo 2 | 0-16 | Dual delay: second tempo division |
| 282-285 | Feedback | 0-120% | Repeat count (>100% = infinite) |
| 286-289 | Clip/Feedback2 | 0-24dB / 0-120% | Analog/Tape drive OR Dual FB2 |
| 290-293 | Hi-Cut | 0-61 | 20 Hz to 20 kHz |
| 294-297 | Lo-Cut | 0-61 | 20 Hz to 20 kHz |
| 298-301 | Offset/Pan1 | -200 to +200ms / -50 to +50 | Dynamic offset OR Dual pan1 |
| 302-305 | Sense/Pan2 | -50 to 0 dB / -50 to +50 | Dynamic threshold OR Dual pan2 |
| 306-309 | Damp | 0-100 dB | Dynamic: attenuation amount |
| 310-313 | Release | 20-1000 ms | Dynamic: release time |
| 314-317 | Mix | 0-100% | Dry/wet balance |
| 322-325 | Delay On/Off | 0-1 | Effect enable |

**Spillover:** Controlled by FX Mute parameter (Utility menu)
- **Soft:** Delays ring out on preset change
- **Hard:** Delays mute immediately

---

### 5. REVERB (Bytes 326-389)

**Type Offset:** 326-329 (0x146)
- `00` = Spring
- `01` = Hall
- `02` = Room
- `03` = Plate

#### Parameters
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 330-333 | Decay | 0.1-20 seconds | Reverb length (60 dB decay time) |
| 334-337 | Pre-Delay | 0-100 ms | Initial delay before reverb |
| 338-341 | Shape | 0-2 | 0=Round, 1=Curved, 2=Square |
| 342-345 | Size | 0-7 | 0=Box, 1=Tiny, ..., 7=Huge |
| 346-349 | Hi Color | 0-6 | 0=Wool, 1=Warm, ..., 6=Glass |
| 350-353 | Hi Level | -25 to +25 dB | High color emphasis |
| 354-357 | Lo Color | 0-6 | 0=Thick, 1=Round, ..., 6=NoBass |
| 358-361 | Lo Level | -25 to +25 dB | Low color emphasis |
| 362-365 | Room Level | -100 to 0 dB | Early reflections level |
| 366-369 | Reverb Level | -100 to 0 dB | Diffuse field level |
| 370-373 | Diffuse | -25 to +25 dB | Density adjustment |
| 374-377 | Mix | 0-100% | Dry/wet balance |
| 386-389 | Reverb On/Off | 0-1 | Effect enable |

---

### 6. EQ + NOISE GATE (Bytes 390-453)

#### Noise Gate
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 390-393 | Gate Type | 0-1 | 0=Hard, 1=Soft |
| 394-397 | Threshold | -60 to 0 dB | Gate activation point |
| 398-401 | Damp | 0-90 dB | Attenuation amount |
| 402-405 | Release | 0-200 dB/sec | Release speed |
| 450-453 | Gate On/Off | 0-1 | Gate enable |

#### 3-Band Parametric EQ
Each band (1, 2, 3) has identical parameter structure:

**Band 1:**
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 410-413 | Freq 1 | 41 Hz - 20 kHz | Center frequency (lookup table) |
| 414-417 | Gain 1 | -12 to +12 dB | Boost/cut amount |
| 418-421 | Width 1 | 0.3-1.6 octaves | Bandwidth (lookup table) |

**Band 2:** Offsets 422-433  
**Band 3:** Offsets 434-445

**EQ On/Off:** Byte 406-409

**EQ Lock:** Global parameter (Utility menu) - locks EQ across all presets

---

### 7. PITCH (Bytes 454-517)

**Type Offset:** 454-457 (0x1C6)
- `00` = Pitch Shifter
- `01` = Octaver
- `02` = Whammy
- `03` = Detune
- `04` = Intelligent Pitch Shifter

#### Pitch Shifter Mode
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 458-461 | Voice 1 | -1200 to +1200 cents | First voice pitch (100 cents = 1 semitone) |
| 462-465 | Voice 2 | -1200 to +1200 cents | Second voice pitch |
| 466-469 | Pan 1 | -50 to +50 | Voice 1 stereo position |
| 470-473 | Pan 2 | -50 to +50 | Voice 2 stereo position |
| 474-477 | Delay 1 | 0-50 ms | Voice 1 delay |
| 478-481 | Delay 2 | 0-50 ms | Voice 2 delay |
| 482-485 | Feedback 1 | 0-100% | Voice 1 repeat count |
| 486-489 | Feedback 2 | 0-100% | Voice 2 repeat count |
| 490-493 | Level 1 | -100 to 0 dB | Voice 1 output level |
| 494-497 | Level 2 | -100 to 0 dB | Voice 2 output level |

#### Octaver Mode
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 494-497 | Direction | 0-1 | 0=Down, 1=Up |
| 498-501 | Range | 1-2 | 1=One octave, 2=Two octaves |

#### Whammy Mode
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 458-461 | Pitch | 0-100% | Expression pedal position |
| 494-497 | Direction | 0-1 | 0=Down, 1=Up |
| 498-501 | Range | 1-2 | 1=One octave, 2=Two octaves |

#### Detune Mode
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 458-461 | Voice 1 | -100 to +100 cents | First voice detune |
| 462-465 | Voice 2 | -100 to +100 cents | Second voice detune |
| 474-477 | Delay 1 | 0-50 ms | Voice 1 delay |
| 478-481 | Delay 2 | 0-50 ms | Voice 2 delay |

#### Intelligent Pitch Shifter Mode
| Offset | Parameter | Range | Description |
|--------|-----------|-------|-------------|
| 458-461 | Voice 1 | -13 to +13 scale degrees | Harmony voice 1 interval |
| 462-465 | Voice 2 | -13 to +13 scale degrees | Harmony voice 2 interval |
| 466-469 | Pan 1 | -50 to +50 | Voice 1 stereo position |
| 470-473 | Pan 2 | -50 to +50 | Voice 2 stereo position |
| 474-477 | Delay 1 | 0-50 ms | Voice 1 delay (for natural feel) |
| 478-481 | Delay 2 | 0-50 ms | Voice 2 delay |
| 482-485 | Key | 0-11 | 0=C, 1=C#, ..., 11=B |
| 486-489 | Scale | 0-13 | See scale lookup table |
| 490-493 | Level 1 | -100 to 0 dB | Voice 1 output level |
| 494-497 | Level 2 | -100 to 0 dB | Voice 2 output level |

**Pitch On/Off:** Byte 514-517

**Mix (all modes):** Byte 502-505

---

## Lookup Tables

### Tempo Subdivisions (Offset value 0-16)
```
0x00 = Ignore (use fixed Speed/Delay Time)
0x01 = 2 bars
0x02 = 1 bar
0x03 = 1/2
0x04 = 1/2T (triplet)
0x05 = 1/4
0x06 = 1/4D (dotted)
0x07 = 1/4T
0x08 = 1/8
0x09 = 1/8D
0x0A = 1/8T
0x0B = 1/16
0x0C = 1/16D
0x0D = 1/16T
0x0E = 1/32
0x0F = 1/32D
0x10 = 1/32T
```

### Musical Scales (Intelligent Pitch Shifter)
```
0x00 = Ionian (Major)          1,2,3,4,5,6,7
0x01 = Dorian                  1,2,b3,4,5,6,b7
0x02 = Phrygian                1,b2,b3,4,5,b6,b7
0x03 = Lydian                  1,2,3,#4,5,6,7
0x04 = Mixolydian              1,2,3,4,5,6,b7
0x05 = Aeolian (Natural Minor) 1,2,b3,4,5,b6,b7
0x06 = Locrian                 1,b2,b3,4,b5,b6,b7
0x07 = Pentatonic Minor        1,b3,4,5,b7
0x08 = Pentatonic Major        1,2,3,5,6
0x09 = Blues                   1,b3,4,b5,5,b7
0x0A = Diminished Whole        1,2,b3,4,b5,b6,6,7
0x0B = Whole Tone              1,2,3,#4,#5,b7
0x0C = Harmonic Minor          1,2,b3,4,5,b6,7
```

### Compressor Ratio Table
```
0x00 = Off
0x01 = 1.12:1
0x02 = 1.25:1
0x03 = 1.41:1
0x04 = 1.58:1
0x05 = 1.78:1
0x06 = 2:1
0x07 = 2.24:1
0x08 = 2.51:1
0x09 = 2.82:1
0x0A = 3.16:1
0x0B = 3.55:1
0x0C = 3.98:1
0x0D = 5:1
0x0E = 10:1
0x0F = Infinite:1 (Limiter)
```

### Compressor Attack Table (ms)
```
0x00 = 0.3    0x08 = 7
0x01 = 0.5    0x09 = 10
0x02 = 0.7    0x0A = 14
0x03 = 1.0    0x0B = 20
0x04 = 1.4    0x0C = 30
0x05 = 2.0    0x0D = 50
0x06 = 3.0    0x0E = 70
0x07 = 5.0    0x0F = 100
                0x10 = 140
```

### Compressor Release Table (ms)
```
0x0D = 50     0x14 = 350
0x0E = 75     0x15 = 500
0x0F = 100    0x16 = 700
0x10 = 140    0x17 = 2000
0x11 = 200
0x12 = 250
```

---

## MIDI Control Change Map

The Nova System responds to MIDI CC messages for real-time control. CC numbers are **user-assignable** via the MIDI CC menu.

### Controllable Parameters
| Function | Default CC | Values | Description |
|----------|------------|--------|-------------|
| Tap Tempo | Assignable | 0-127 | Tap tempo trigger |
| Drive On/Off | Assignable | 0=Off, 127=On | Drive block bypass |
| Comp On/Off | Assignable | 0=Off, 127=On | Compressor bypass |
| Noise Gate On/Off | Assignable | 0=Off, 127=On | Gate bypass |
| EQ On/Off | Assignable | 0=Off, 127=On | EQ bypass |
| Boost On/Off | Assignable | 0=Off, 127=On | Boost enable |
| Mod On/Off | Assignable | 0=Off, 127=On | Modulation bypass |
| Pitch On/Off | Assignable | 0=Off, 127=On | Pitch bypass |
| Delay On/Off | Assignable | 0=Off, 127=On | Delay bypass |
| Reverb On/Off | Assignable | 0=Off, 127=On | Reverb bypass |
| Expression Pedal | Assignable | 0-127 | Mapped parameter control |

---

## Program Change Implementation

### Preset Numbering
- **Factory Presets:** Program Change 0-29 (F0-1 through F9-3)
- **User Presets:** Program Change 30-89 (00-1 through 19-3)

### Program Map Feature
The Nova System includes a **Program Map** that allows remapping incoming PC numbers to any preset. This is configured via the MIDI Setup menu and stored in the System Dump.

**Example:**
```
Incoming PC #1 → Preset F0-2 (Factory Bank 0, Preset 2)
Incoming PC #127 → Preset 15-1 (User Bank 15, Preset 1)
```

**Program Change Out:**
When a preset is recalled locally, the Nova System can send the corresponding PC number on MIDI Out (configurable on/off).

---

## System Dump Format

**Message Structure:**
```
F0 00 20 1F [DeviceID] 63 20 02 [System Data] [Checksum] F7
```

**Data Type:** `02` (System)

### System Dump Contents
- Global parameters (tuner reference, MIDI channel, etc.)
- Program Map (PC remapping)
- MIDI CC assignments
- Utility settings (FX Mute, Tap Master, etc.)
- **Does NOT include:** User presets (use User Bank Dump)

---

## User Bank Dump Format

**Message Structure:**
```
F0 00 20 1F [DeviceID] 63 20 03 [60 Presets] [Checksum] F7
```

**Data Type:** `03` (User Bank)

Contains all 60 user presets (00-1 through 19-3) in sequential preset dump format.

---

## MIDI Implementation Chart

| Function | Transmitted | Recognized | Remarks |
|----------|-------------|------------|---------|
| Basic Channel | 1-16 | 1-16, Omni | User-assignable |
| Mode | X | X | - |
| Note Number | X | X | - |
| Velocity | X | X | - |
| Aftertouch | X | X | - |
| Pitch Bend | X | X | - |
| Program Change | 0-126 | 0-126 | Program Map applies |
| Control Change | User-assigned | User-assigned | See CC map |
| System Exclusive | ✓ | ✓ | Preset/System dumps |
| System Common | X | X | - |
| System Real Time | ✓ | ✓ | MIDI Clock for tempo |

**Legend:**
- ✓ = Supported
- X = Not supported

---

## Inquiry Messages (MIDI Device Identity)

### Request
```
F0 7E [DeviceID] 06 01 F7
```

### Reply from Nova System
```
F0 7E [DeviceID] 06 02 00 20 1F 63 00 00 00 00 00 01 [Minor] F7
```

**Breakdown:**
- `00 20 1F` = TC Electronic Manufacturer ID
- `63 00` = Product ID (Nova System)
- `00 00 00 00` = (void)
- `01` = Major version
- `[Minor]` = Minor version (e.g., `0D` = 13, meaning v1.13)

---

## Best Practices for MIDI Implementation

### Preset Loading Workflow
1. Send Program Change OR Request Preset via SysEx
2. Nova System responds with full preset SysEx dump (if requested)
3. Parse 520-byte message
4. Update UI to reflect all parameters
5. Listen for unsolicited CC messages (expression pedal, real-time changes)

### Bidirectional Sync
- **Enable PC Out:** Nova System sends PC when presets change locally
- **Enable MIDI Echo:** CC changes send confirmation back to controller
- **Use Device ID:** In multi-device setups, assign unique IDs

### Checksum Validation
```python
def validate_preset_sysex(data):
    if data[0] != 0xF0 or data[-1] != 0xF7:
        return False
    if data[1:4] != [0x00, 0x20, 0x1F]:
        return False  # Not TC Electronic
    if data[5] != 0x63:
        return False  # Not Nova System
    
    calculated_checksum = sum(data[34:518]) & 0x7F
    received_checksum = data[518]
    return calculated_checksum == received_checksum
```

### Error Handling
- **Invalid checksum:** Request preset again
- **Incomplete SysEx:** Implement timeout (500ms recommended)
- **Buffer overflow:** Queue incoming SysEx, process asynchronously
- **MIDI clock jitter:** Smooth tempo changes over 100-200ms

---

## Reference Implementation Notes

### Nibble Encoding (from Java `Nibble.class`)
```java
// Encode value to 4 bytes
public static byte[] encode(int value) {
    byte[] bytes = new byte[4];
    bytes[0] = (byte)(value & 0x7F);
    bytes[1] = (byte)((value >> 7) & 0x7F);
    bytes[2] = (byte)((value >> 14) & 0x7F);
    bytes[3] = (byte)((value >> 21) & 0x7F);
    return bytes;
}

// Decode 4 bytes to value
public static int decode(byte b1, byte b2, byte b3, byte b4) {
    int value = (b4 << 21) | (b3 << 14) | (b2 << 7) | b1;
    // Sign extension for negative values
    if ((value & 0x8000000) != 0) {
        value |= 0xF0000000;
    }
    return value;
}
```

### Asynchronous I/O Pattern (from Java)
```java
// Input buffer for unsolicited dumps
MidiInputBuffer buffer = new MidiInputBuffer();

// On MIDI receive
void onMidiMessage(byte[] data) {
    if (data[0] == 0xF0) {  // SysEx start
        buffer.append(data);
        if (data[data.length-1] == 0xF7) {  // Complete message
            processSysEx(buffer.getData());
            buffer.clear();
        }
    }
}
```

---

## Appendix: Complete Byte Map Example

**Factory Preset F0-1 "Stevie's Blues"** (Partial decode):
```
Byte    Value   Parameter
------  -----   ---------------------------------
0-8     (header)
9       0x00    (void)
10-33   "Stevie's Blues\0\0\0\0\0\0\0\0\0"  (24 chars)
34-37   0x64 0x00 0x01 0x00  = 384 ms Tap Tempo
38-41   0x00 0x00 0x00 0x00  = Semi-parallel routing
...
130-133 0x01 0x00 0x00 0x00  = Comp ON
194-197 0x01 0x00 0x00 0x00  = Drive ON (Overdrive)
322-325 0x00 0x00 0x00 0x00  = Delay OFF
386-389 0x01 0x00 0x00 0x00  = Reverb ON (Plate)
...
518     0x28    = Checksum
519     0xF7    = SysEx End
```

---

**Document Version:** 1.0  
**Last Updated:** January 30, 2026  
**Source:** Nova System Sysex Map.pdf + NovaManager Java class analysis
