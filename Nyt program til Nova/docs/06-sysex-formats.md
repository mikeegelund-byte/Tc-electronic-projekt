# SysEx Formater (Koncise reference)

**Fuld dokumentation:** se `..\MIDI_PROTOCOL.md` (520+ linjer)

---

## Preset Dump (520 bytes)

**Format:**
```
F0 00 20 1F [ID] 63 20 01 [PresetNum] [Name:24] [Data:480] [Checksum] F7
```

**Bytearkitektur:**
| Offset | Length | Content |
|--------|--------|---------|
| 0 | 1 | F0 (Start) |
| 1-3 | 3 | 00 20 1F (TC Electronic ID) |
| 4 | 1 | Device ID (0-126 or 127=ALL) |
| 5 | 1 | 63 (Nova System model) |
| 6 | 1 | 20 (Dump message) |
| 7 | 1 | 01 (Preset type) |
| 8 | 1 | Preset number (01-1E factory, 1F-5A user) |
| 9 | 1 | Reserved |
| 10-33 | 24 | Preset name (ASCII padded with 0x00) |
| 34-517 | 484 | Effect block parameters (nibble-encoded) |
| 518 | 1 | Checksum: `(sum of 34..517) & 0x7F` |
| 519 | 1 | F7 (End) |

---

## User Bank Dump (variable, ~31KB)

**Format:**
```
F0 00 20 1F [ID] 63 20 03 [Preset1..Preset60] [Checksum] F7
```

- 60 × full preset dumps concatenated
- Single checksum for entire bank
- Sent as one continuous SysEx

**Size:** ~31,200 bytes (60 × 520)

---

## System Dump (variable, ~1KB)

**Format:**
```
F0 00 20 1F [ID] 63 20 02 [System data...] [Checksum] F7
```

**Contents:**
- Global parameters
- Program Map (PC remapping table)
- MIDI CC assignments
- Utility settings

**Size:** ~1,000 bytes (estimate)

---

## Checksum algorithm

```csharp
public static byte CalculateChecksum(byte[] sysexData, int startOffset, int endOffset)
{
    int sum = 0;
    for (int i = startOffset; i < endOffset; i++)
    {
        sum += sysexData[i];
    }
    return (byte)(sum & 0x7F);  // Keep 7 LSBs
}

// Usage: Preset dump
var checksum = CalculateChecksum(data, 34, 518);
```

---

## Nibble encoding (signed values)

**Why:** SysEx is 7-bit, but parameters range over 16-bit or signed.

**Encoding:** 4 bytes per value
```csharp
// Encode 16-bit value
public static byte[] EncodeNibble(int value)
{
    return new byte[]
    {
        (byte)(value & 0x7F),
        (byte)((value >> 7) & 0x7F),
        (byte)((value >> 14) & 0x7F),
        (byte)((value >> 21) & 0x7F)
    };
}

// Decode
public static int DecodeNibble(byte b1, byte b2, byte b3, byte b4)
{
    // Combine 4x7-bit values into single int (little-endian)
    int rawValue = (b4 << 21) | (b3 << 14) | (b2 << 7) | b1;
    return rawValue;
}

// Global/Hybrid Offset Strategy (TC Electronic Specific)
// Used for signed dB values (e.g. -100dB, -30dB, etc.)
public static int DecodeSignedDbValue(int rawValue)
{
    const int LARGE_OFFSET = 16777216; // 2^24

    // Heuristic: If value is ~16M, it uses the large offset.
    // If value is small (positive), it is a direct integer.
    if (rawValue > 16000000)
    {
        return rawValue - LARGE_OFFSET;
    }
    return rawValue;
}
```

---

## Preset number mapping

| Range | Type | Example |
|-------|------|---------|
| 0x01-0x1E (1-30) | Factory | F0-1, F9-3 |
| 0x1F-0x5A (31-90) | User | 00-1, 19-3 |

**Storage:** User bank = 60 presets (slots 00-1 to 19-3)

---

## Quick reference
- Preset: single SysEx (~520 bytes)
- Bank: 60 × preset (~31KB)
- System: global config (~1KB)
- Checksum: `(sum & 0x7F)`
- Encoding: nibble (4 bytes per value)

**See `..\MIDI_PROTOCOL.md` for complete effect parameter offset map.**
