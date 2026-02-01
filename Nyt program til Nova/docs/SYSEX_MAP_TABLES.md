# Nova System SysEx Map - LLM Reference Tables

## 1. Message Structure (520 bytes total)

| Offset | Hex | Length | Field | Description |
|--------|-----|--------|-------|-------------|
| 0 | 000 | 1 | Start | F0 (SysEx Begin) |
| 1-3 | 001 | 3 | Manufacturer | 00 20 1F (TC Electronic) |
| 4 | 004 | 1 | Device ID | 00-7F (0-126, or 7F=All) |
| 5 | 005 | 1 | Model ID | 63 (Nova System) |
| 6 | 006 | 1 | Message ID | 20=Dump, 45=Request |
| 7 | 007 | 1 | Data Type | 01=Preset, 02=System |
| 8 | 008 | 1 | Preset Number | 01-1E (Factory), 1F-5A (User 31-90) |
| 9 | 009 | 1 | Reserved | void |
| 10-33 | 00A | 24 | Preset Name | ASCII (24 chars) |
| 34-517 | 022 | 484 | Parameters | 4-byte encoded values |
| 518 | 206 | 1 | Checksum | (sum bytes 34-517) & 0x7F |
| 519 | 207 | 1 | End | F7 (SysEx End) |

---

## 2. Global Parameters (Bytes 34-69)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 38-41 | 026 | Tap Tempo | 64-38 17 | 100-3000 ms | by 1ms |
| 42-45 | 02A | Routing | 00-02 | 0-2 | 0=Semi-par, 1=Serial, 2=Parallel |
| 46-49 | 02E | Level Out L | 1C 7F 7F 07 - 00 | -100 to 0 dB | Signed, by 1dB |
| 50-53 | 032 | Level Out R | 1C 7F 7F 07 - 00 | -100 to 0 dB | Signed, by 1dB |
| 54-57 | 036 | Map Parameter | - | - | Pedal parameter list |
| 58-61 | 03A | Map Min | 00-64 | 0-100% | by 1% |
| 62-65 | 03E | Map Mid | 00-64 | 0-100% | by 1% |
| 66-69 | 042 | Map Max | 00-64 | 0-100% | by 1% |

---

## 3. COMP Block (Bytes 70-133)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 70-73 | 046 | Type | 00-02 | 0-2 | 0=perc, 1=sustain, 2=advanced |
| 74-77 | 04A | Threshold (adv) | E2 7F 7F 0F - 00 | -30 to 0 dB | Signed |
| 78-81 | 04E | Ratio (adv) | 00-0F | 0-15 | See Comp Ratio table |
| 82-85 | 052 | Attack (adv) | 00-10 | 0-16 | See Attack table (0.3-140ms) |
| 86-89 | 056 | Release (adv) | 0D-17 | 13-23 | See COMP Time table (50-2000ms) |
| 90-93 | 05A | Response (perc/sustain) | 01-0A | 1-10 | |
| 94-97 | 05E | Drive (perc/sustain) | 01-14 | 1-20 | |
| 98-101 | 062 | Level | 74 7F 7F 0F - 0C | -12 to +12 dB | Signed |
| 130-133 | 082 | On/Off | 00-01 | 0-1 | 0=Off, 1=On |

---

## 4. DRIVE Block (Bytes 134-193)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 134-137 | 086 | Type | 00-01 | 0-1 | 0=overdrive, 1=distortion |
| 138-141 | 08A | Gain | ? | ? | Unknown range |
| 142-145 | 08E | Tone | 00-64 | 0-100% | |
| 182-185 | 0B6 | Boost Level | 00-0A | 0-10 dB | by 1dB |
| 186-189 | 0BA | Boost On/Off | 00-01 | 0-1 | |
| 190-193 | 0BE | Drive Level | ? | ? | Unknown range |
| 194-197 | 0C2 | On/Off | 00-01 | 0-1 | |

---

## 5. MOD Block (Bytes 198-261)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 198-201 | 0C6 | Type | 00-05 | 0-5 | 0=chorus, 1=flanger, 2=vibrato, 3=phaser, 4=tremolo, 5=panner |
| 202-205 | 0CA | Speed | 00-50 01 | 0-81 | See Speed table (0.050-20 Hz) |
| 206-209 | 0CE | Depth | 00-64 | 0-100% | |
| 210-213 | 0D2 | Tempo | 00-10 | 0-16 | 0=Ignore, 1=2bars...16=1/32T |
| 214-217 | 0D6 | Hi Cut | 00-3D | 0-61 | See HiCut table (20Hz-20kHz) |
| 218-221 | 0DA | Feedback (fla,pha) | 40 7F 7F 07 - 64 | -100 to +100% | Signed |
| 222-225 | 0DE | Delay/Range/Type | 00-74 03 / 0C 00-0C 01 | varies | Type-dependent (see below) |
| 238-241 | 0EE | Width (tremolo) | 00-64 | 0-100% | |
| 250-253 | 0FA | Mix | 00-64 | 0-100% | |
| 258-261 | 102 | On/Off | 00-01 | 0-1 | |

### MOD Multi-function Parameter (222-225)
| Type | Parameter | Range |
|------|-----------|-------|
| Chorus | Delay | 0-50ms (0.1ms steps) |
| Flanger | Delay | 0-50ms (0.1ms steps) |
| Phaser | Range | 0=Low, 1=High |
| Tremolo | Type | 0=Soft, 1=Hard |

---

## 6. DELAY Block (Bytes 262-325)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 262-265 | 106 | Type | 00-05 | 0-5 | 0=clean, 1=analog, 2=tape, 3=dynamic, 4=dual, 5=ping-pong |
| 266-269 | 10A | Delay Time | 00-08 0E | 0-1800 ms | |
| 270-273 | 10E | Delay 2 (dual) | 00-08 0E | 0-1800 ms | |
| 274-277 | 112 | Tempo | 00-10 | 0-16 | See Tempo table |
| 278-281 | 116 | Tempo2/Width | 00-10 / 00-64 | varies | Type-dependent |
| 282-285 | 11A | Feedback | 00-78 | 0-120% | |
| 286-289 | 11E | Clip/Feedback2 | 00-18 / 00-78 | varies | Type-dependent |
| 290-293 | 122 | Hi Cut | 00-3D | 0-61 | See HiCut table |
| 294-297 | 126 | Lo Cut | 00-3D | 0-61 | See HiCut table |
| 298-301 | 12A | Offset/Pan1 | 38 7E 7F 07-48 01 / 4E 7F 7F 07-32 | -200 to +200ms / -50 to +50 | Signed, type-dependent |
| 302-305 | 12E | Sense/Pan2 | 4E 7F 7F 07-00 / 4E 7F 7F 07-32 | -50 to 0dB / -50 to +50 | Signed, type-dependent |
| 306-309 | 132 | Damp (dynamic) | 00-64 | 0-100 dB | |
| 310-313 | 136 | Release (dynamic) | 0B-15 | 11-21 | See Release table (20-1000ms) |
| 314-317 | 13A | Mix | 00-64 | 0-100% | |
| 322-325 | 142 | On/Off | 00-01 | 0-1 | |

### DELAY Multi-function Parameters
| Type | Offset 278 | Offset 286 | Offset 298 | Offset 302 |
|------|------------|------------|------------|------------|
| Clean | - | - | - | - |
| Analog | - | Clip (0-24dB) | - | - |
| Tape | - | Clip (0-24dB) | - | - |
| Dynamic | - | - | Offset (-200 to +200ms) | Sense (-50 to 0dB) |
| Dual | Tempo2 (0-16) | Feedback2 (0-120%) | Pan1 (-50 to +50) | Pan2 (-50 to +50) |
| Ping-Pong | Width (0-100%) | - | - | - |

---

## 7. REVERB Block (Bytes 326-389)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 326-329 | 146 | Type | 00-03 | 0-3 | 0=spring, 1=hall, 2=room, 3=plate |
| 330-333 | 14A | Decay | 01-48 01 | 1-200 | 0.1-20 sec (by 0.1s) |
| 334-337 | 14E | Pre Delay | 00-64 | 0-100 ms | by 1ms |
| 338-341 | 152 | Shape | 00-02 | 0-2 | 0=Round, 1=Curved, 2=Square |
| 342-345 | 156 | Size | 00-07 | 0-7 | 0=Box...7=Huge |
| 346-349 | 15A | Hi Color | 00-06 | 0-6 | 0=Wool...6=Glass |
| 350-353 | 15E | Hi Level | 67 7F 7F 07 - 19 | -25 to +25 dB | Signed |
| 354-357 | 162 | Lo Color | 00-06 | 0-6 | 0=Thick...6=NoBass |
| 358-361 | 166 | Lo Level | 67 7F 7F 07 - 19 | -25 to +25 dB | Signed |
| 362-365 | 16A | Room Level | 1C 7F 7F 07 - 00 | -100 to 0 dB | Signed |
| 366-369 | 16E | Reverb Level | 1C 7F 7F 07 - 00 | -100 to 0 dB | Signed |
| 370-373 | 172 | Diffuse | 67 7F 7F 07 - 19 | -25 to +25 dB | Signed |
| 374-377 | 176 | Mix | 00-64 | 0-100% | |
| 386-389 | 182 | On/Off | 00-01 | 0-1 | |

---

## 8. EQ/GATE Block (Bytes 390-453)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 390-393 | 186 | Gate Type | 00-01 | 0-1 | 0=Hard, 1=Soft |
| 394-397 | 18A | Gate Threshold | 44 7F 7F 0F - 00 | -60 to 0 dB | Signed |
| 398-401 | 18E | Gate Damp | 00-5A | 0-90 dB | |
| 402-405 | 192 | Gate Release | 00-48 01 | 0-200 | dB/s |
| 406-409 | 196 | EQ On/Off | 00-01 | 0-1 | |
| 410-413 | 19A | EQ Freq1 | 19-71 01 | 25-113 | See EQ Freq table (41Hz-20kHz) |
| 414-417 | 19E | EQ Gain1 | 74 7F 7F 0F - 0C | -12 to +12 dB | Signed |
| 418-421 | 1A2 | EQ Width1 | 05-0C | 5-12 | See EQ Width table (0.3-1.6 oct) |
| 422-425 | 1A6 | EQ Freq2 | 19-71 01 | 25-113 | See EQ Freq table |
| 426-429 | 1AA | EQ Gain2 | 74 7F 7F 0F - 0C | -12 to +12 dB | Signed |
| 430-433 | 1AE | EQ Width2 | 05-0C | 5-12 | See EQ Width table |
| 434-437 | 1B2 | EQ Freq3 | 19-71 01 | 25-113 | See EQ Freq table |
| 438-441 | 1B6 | EQ Gain3 | 74 7F 7F 0F - 0C | -12 to +12 dB | Signed |
| 442-445 | 1BA | EQ Width3 | 05-0C | 5-12 | See EQ Width table |
| 450-453 | 1C2 | Gate On/Off | 00-01 | 0-1 | |

---

## 9. PITCH Block (Bytes 454-517)

| Offset | Hex | Parameter | Raw Range | Decoded Range | Notes |
|--------|-----|-----------|-----------|---------------|-------|
| 454-457 | 1C6 | Type | 00-04 | 0-4 | 0=shifter, 1=octaver, 2=whammy, 3=detune, 4=intelligent |
| 458-461 | 1CA | Voice 1 | 1C 7F 7F 07-64 / 74 7F 7F 07-0C | varies | Type-dependent |
| 462-465 | 1CE | Voice 2 | 1C 7F 7F 07-64 / 74 7F 7F 07-0C | varies | Type-dependent |
| 466-469 | 1D2 | Pan 1 | 4E 7F 7F 07 - 32 | -50 to +50 | Signed (50L to 50R) |
| 470-473 | 1D6 | Pan 2 | 4E 7F 7F 07 - 32 | -50 to +50 | Signed (50L to 50R) |
| 474-477 | 1DA | Delay 1 | 00-32 | 0-50 ms | |
| 478-481 | 1DE | Delay 2 | 00-32 | 0-50 ms | |
| 482-485 | 1E2 | Feedback1/Key | 00-64 / 00-0C | 0-100% / 0-12 | Type-dependent |
| 486-489 | 1E6 | Feedback2/Scale | 00-64 / 00-0D | 0-100% / 0-13 | Type-dependent |
| 490-493 | 1EA | Level 1 | 1C 7F 7F 07 - 00 | -100 to 0 dB | Signed |
| 494-497 | 1EE | Level 2/Direction | 1C 7F 7F 07-00 / 00-01 | -100 to 0dB / 0-1 | Type-dependent |
| 498-501 | 1F2 | Range (oct,wham) | 01-02 | 1-2 | 1oct, 2oct |
| 502-505 | 1F6 | Mix | 00-64 | 0-100% | |
| 514-517 | 202 | On/Off | 00-01 | 0-1 | |

### PITCH Multi-function Parameters
| Type | Voice 1/2 | Feedback1/2 | Level2/Direction |
|------|-----------|-------------|------------------|
| Shifter | -100 to +100 cents | 0-100% | -100 to 0dB |
| Octaver | - | - | Direction (0=Down, 1=Up) |
| Whammy | - | - | Direction (0=Down, 1=Up) |
| Detune | -100 to +100 cents | - | - |
| Intelligent | -13 to +13 degrees | Key (0-12) / Scale (0-13) | -100 to 0dB |

---

## 10. Lookup Tables

### Attack Table (COMP)
| Index | Attack (ms) |
|-------|-------------|
| 0 | 0.3 |
| 1 | 0.5 |
| 2 | 0.7 |
| 3 | 1.0 |
| 4 | 1.4 |
| 5 | 2.0 |
| 6 | 3 |
| 7 | 5 |
| 8 | 7 |
| 9 | 10 |
| 10 | 14 |
| 11 | 20 |
| 12 | 30 |
| 13 | 50 |
| 14 | 70 |
| 15 | 100 |
| 16 | 140 |

### Degrees Table (Intelligent Pitch)
| Index | Degrees |
|-------|---------|
| 0 | Unison |
| 1 | 2 |
| 2 | 3 |
| 3 | 4 |
| 4 | 5 |
| 5 | 6 |
| 6 | 7 |
| 7 | 1oct |
| 8 | 9 |
| 9 | 10 |
| 10 | 11 |
| 11 | 12 |
| 12 | 13 |

### Key Table (Intelligent Pitch)
| Index | Key |
|-------|-----|
| 0 | C |
| 1 | C# |
| 2 | D |
| 3 | D# |
| 4 | E |
| 5 | F |
| 6 | F# |
| 7 | G |
| 8 | G# |
| 9 | A |
| 10 | A# |
| 11 | B |

### Scale Table (Intelligent Pitch)
| Index | Scale |
|-------|-------|
| 0 | Ionian |
| 1 | Dorian |
| 2 | Phrygian |
| 3 | Lydian |
| 4 | Mixolydian |
| 5 | Aeolian |
| 6 | Locrian |
| 7 | PntMin (Pentatonic Minor) |
| 8 | PntMaj (Pentatonic Major) |
| 9 | Blues |
| 10 | DimWhl (Diminished Whole) |
| 11 | Whole |
| 12 | HrmMin (Harmonic Minor) |

### Shape Table (Reverb)
| Index | Shape |
|-------|-------|
| 0 | Round |
| 1 | Curved |
| 2 | Square |

### Size Table (Reverb)
| Index | Size |
|-------|------|
| 0 | Box |
| 1 | Tiny |
| 2 | Small |
| 3 | Medium |
| 4 | Large |
| 5 | XL |
| 6 | Grand |
| 7 | Huge |

### Hi Color Table (Reverb)
| Index | Color |
|-------|-------|
| 0 | Wool |
| 1 | Warm |
| 2 | Real |
| 3 | Clear |
| 4 | Bright |
| 5 | Crisp |
| 6 | Glass |

### Lo Color Table (Reverb)
| Index | Color |
|-------|-------|
| 0 | Thick |
| 1 | Round |
| 2 | Real |
| 3 | Light |
| 4 | Tight |
| 5 | Thin |
| 6 | NoBass |

### EQ Width Table
| Index | Width (octaves) |
|-------|-----------------|
| 5 | 0.3 |
| 6 | 0.4 |
| 7 | 0.5 |
| 8 | 0.7 |
| 9 | 0.9 |
| 10 | 1.1 |
| 11 | 1.4 |
| 12 | 1.6 |

---

## 11. 4-Byte Encoding (Nibble Format)

### Encoding Formula
All parameters use 4-byte little-endian 7-bit encoding:
```
byte[0] = value & 0x7F          // LSB (bits 0-6)
byte[1] = (value >> 7) & 0x7F   // bits 7-13
byte[2] = (value >> 14) & 0x7F  // bits 14-20
byte[3] = (value >> 21) & 0x7F  // MSB (bits 21-27)
```

### Decoding Formula
```
value = byte[0] | (byte[1] << 7) | (byte[2] << 14) | (byte[3] << 21)
```

### Signed Value Encoding Examples
| Decimal | Hex Bytes (4-byte) | Notes |
|---------|-------------------|-------|
| -1 | 7F 7F 7F 07 | Two's complement in 28-bit |
| -12 | 74 7F 7F 0F | EQ Gain minimum |
| -25 | 67 7F 7F 07 | Reverb Level minimum |
| -30 | 62 7F 7F 07 | COMP Threshold minimum |
| -50 | 4E 7F 7F 07 | Pan minimum (50L) |
| -100 | 1C 7F 7F 07 | Level minimum |
| -200 | 38 7E 7F 07 | Delay Offset minimum |
| 0 | 00 00 00 00 | Zero |
| +12 | 0C 00 00 00 | EQ Gain maximum |
| +25 | 19 00 00 00 | Reverb Level maximum |
| +50 | 32 00 00 00 | Pan maximum (50R) |
| +100 | 64 00 00 00 | 100% |
| +200 | 48 01 00 00 | Delay Offset maximum |

---

## 12. SysEx Commands

### Request Preset
```
F0 00 20 1F [DevID] 63 45 01 [Preset#] 00 F7
```
- DevID: 00-7E (specific) or 7F (all)
- Preset#: 00=Current, 01-1E=Factory, 1F-5A=User

### Request System
```
F0 00 20 1F [DevID] 63 45 02 00 00 F7
```

### Identity Request (Who Are You)
```
F0 7E [DevID] 06 01 F7
```

### Identity Reply
```
F0 7E [DevID] 06 02 00 20 1F 63 00 00 00 00 00 [MajVer] [MinVer] F7
```

---

## 13. Summary: Signed Parameter Offsets

Parameters that use signed encoding (require offset decoding):

| Offset | Parameter | Range | Encoding Type |
|--------|-----------|-------|---------------|
| 46-49 | Level Out L | -100 to 0 dB | Simple offset |
| 50-53 | Level Out R | -100 to 0 dB | Simple offset |
| 74-77 | COMP Threshold | -30 to 0 dB | Simple offset |
| 98-101 | COMP Level | -12 to +12 dB | Symmetric |
| 218-221 | MOD Feedback | -100 to +100% | Symmetric |
| 298-301 | Delay Offset | -200 to +200 ms | Symmetric |
| 302-305 | Delay Sense | -50 to 0 dB | Simple offset |
| 350-353 | Reverb Hi Level | -25 to +25 dB | Symmetric |
| 358-361 | Reverb Lo Level | -25 to +25 dB | Symmetric |
| 362-365 | Reverb Room Level | -100 to 0 dB | Simple offset |
| 366-369 | Reverb Level | -100 to 0 dB | Simple offset |
| 370-373 | Reverb Diffuse | -25 to +25 dB | Symmetric |
| 394-397 | Gate Threshold | -60 to 0 dB | Simple offset |
| 414-417 | EQ Gain 1 | -12 to +12 dB | Symmetric |
| 426-429 | EQ Gain 2 | -12 to +12 dB | Symmetric |
| 438-441 | EQ Gain 3 | -12 to +12 dB | Symmetric |
| 458-461 | Pitch Voice 1 | -100 to +100 cents | Symmetric |
| 462-465 | Pitch Voice 2 | -100 to +100 cents | Symmetric |
| 466-469 | Pitch Pan 1 | -50 to +50 | Symmetric |
| 470-473 | Pitch Pan 2 | -50 to +50 | Symmetric |
| 490-493 | Pitch Level 1 | -100 to 0 dB | Simple offset |
| 494-497 | Pitch Level 2 | -100 to 0 dB | Simple offset |

---

*Generated from: Nova System Sysex Map.pdf (TC Electronic)*
