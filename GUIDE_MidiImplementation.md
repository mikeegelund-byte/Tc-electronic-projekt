# TC ELECTRONIC NOVA SYSTEM - MIDI KOMMUNIKATIONS PROTOKOL
## Komplet Guide til C# Implementation

---

## INDHOLDSFORTEGNELSE
1. Hardware Setup & Forbindelse
2. MIDI Kommunikations Mekanik
3. Python Test - Hvad Vi Lærte
4. C# Implementation Guide
5. Kritiske Detaljer & Faldgruber
6. Test Procedure

---

## 1. HARDWARE SETUP & FORBINDELSE

### Fysisk Setup
- **Pedal**: TC Electronic Nova System
- **Interface**: USB MIDI Interface
- **PC**: Windows med DryWetMIDI library (C#) eller mido (Python)

### MIDI Device Navne
```
INPUT:  USB MIDI Interface 0
OUTPUT: USB MIDI Interface 1
```

**KRITISK ADVARSEL:**
- Der findes OGSÅ "Microsoft GS Wavetable Synth 0" som OUTPUT
- DENNE MÅ ALDRIG VÆLGES - den blokerer MIDI fuldstændigt!
- Hvis forkert device vælges: Pedalen skal genstartes!

---

## 2. MIDI KOMMUNIKATIONS MEKANIK

### Grundlæggende Koncept
Nova System kommunikerer via **SysEx (System Exclusive)** beskeder:
- SysEx = MIDI besked type der indeholder rå binær data
- Hver SysEx besked = én komplet preset ELLER system konfiguration
- Data format: Byte array af variable længder

### Kommunikations Retninger

#### A. PEDAL → PC (Dump Mode)
Pedalen sender data når brugeren manuelt aktiverer dump:
- **"Dump Bank"**: Sender 60 separate SysEx beskeder (ét preset ad gangen)
- **"Dump System"**: Sender 1 SysEx besked (system konfiguration)

**Vigtig detalje:**
- Beskederne sendes hurtigt efter hinanden (RAP succession)
- PC'en skal være klar til at modtage INDEN brugeren trykker dump
- Beskeder kan IKKE "trækkes" fra PC - kun aktiveres fra pedal

#### B. PC → PEDAL (Load Mode)
PC'en kan sende data når som helst:
- Send enkelt preset via 1 SysEx besked
- Send 60 presets via 60 separate SysEx beskeder
- Send system konfiguration via 1 SysEx besked

**Vigtig detalje:**
- Lille pause (50-100ms) mellem hver besked anbefales
- Ingen bekræftelse fra pedal - "fire and forget"
- Pedalen opdaterer automatisk når besked modtages

---

## 3. PYTHON TEST - HVAD VI LÆRTE

### Test Flow (Valideret 2026-02-06)

#### Test 1: Modtag Bank Dump (60 presets)
```python
# Setup callback til at opsamle SysEx
received_data = []

def receive_callback(msg):
    if msg.type == 'sysex':
        received_data.append(msg.data)
        print(f"Modtog #{len(received_data)}: {len(msg.data)} bytes")

midi_in.callback = receive_callback

# Vent på bruger trykker "Dump Bank"
input("Tryk ENTER når færdig...")

# Validér antal
if len(received_data) == 60:
    print("SUCCESS!")
```

**Observationer:**
- Hver preset = 518 bytes SysEx data
- Alle 60 beskeder ankommer indenfor få sekunder
- Data gemmes som `bytes` arrays i Python (tilsvarende `byte[]` i C#)

#### Test 2: Send Preset Tilbage
```python
# Send første modtagne preset tilbage
msg = mido.Message('sysex', data=received_data[0])
midi_out.send(msg)
```

**Observationer:**
- Øjeblikkelig transmission
- Pedalen opdaterer automatisk
- Ingen fejl eller timeout

#### Test 3: Modtag System Dump
```python
received_data.clear()  # Ryd buffer
# Vent på "Dump System"
input("Tryk ENTER når færdig...")

if len(received_data) == 1:
    print("SUCCESS!")
```

**Observationer:**
- Kun 1 besked modtages
- Større end preset (system indeholder alle globale settings)

#### Test 4: Send System Tilbage
```python
msg = mido.Message('sysex', data=received_data[0])
midi_out.send(msg)
```

**Observationer:**
- Fungerer identisk med preset send
- Pedalen genindlæser system konfiguration

---

## 4. C# IMPLEMENTATION GUIDE

### DryWetMIDI Library Setup

#### 1. Åbn MIDI Devices
```csharp
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

// Åbn input (fra pedal)
var midiIn = InputDevice.GetByName("USB MIDI Interface 0");

// Åbn output (til pedal)  
var midiOut = OutputDevice.GetByName("USB MIDI Interface 1");
```

**KRITISK:** Brug PRÆCIS disse navne, IKKE "Microsoft GS Wavetable Synth"!

#### 2. Modtag SysEx Beskeder
```csharp
var receivedData = new List<byte[]>();

// Setup event handler
midiIn.EventReceived += (sender, e) =>
{
    if (e.Event is NormalSysExEvent sysEx)
    {
        receivedData.Add(sysEx.Data);
        Console.WriteLine($"Modtog #{receivedData.Count}: {sysEx.Data.Length} bytes");
    }
};

// Start listening
midiIn.StartEventsListening();

// Nu er PC klar til at modtage - bruger kan trykke "Dump Bank" på pedal

// Vent til brugeren er færdig
Console.ReadLine();

// Valider antal
if (receivedData.Count == 60)
{
    Console.WriteLine("SUCCESS! Modtog komplet bank");
}
```

#### 3. Send SysEx Beskeder
```csharp
// Send enkelt preset
byte[] presetData = receivedData[0];
var sysExEvent = new NormalSysExEvent(presetData);
midiOut.SendEvent(sysExEvent);

// Send alle 60 presets
for (int i = 0; i < receivedData.Count; i++)
{
    var sysEx = new NormalSysExEvent(receivedData[i]);
    midiOut.SendEvent(sysEx);
    
    Thread.Sleep(50); // Lille pause mellem beskeder
}
```

#### 4. Gem/Load Fra Disk
```csharp
// Gem preset
File.WriteAllBytes("Preset_01.syx", receivedData[0]);

// Load preset
byte[] presetData = File.ReadAllBytes("Preset_01.syx");
var sysEx = new NormalSysExEvent(presetData);
midiOut.SendEvent(sysEx);
```

---

## 5. KRITISKE DETALJER & FALDGRUBER

### Device Selection
❌ **FORKERT:**
```csharp
var midiOut = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
// Dette blokerer MIDI fuldstændigt!
```

✅ **KORREKT:**
```csharp
var midiOut = OutputDevice.GetByName("USB MIDI Interface 1");
```

### Event Type Checking
❌ **FORKERT:**
```csharp
midiIn.EventReceived += (sender, e) =>
{
    // Dette fanger ALLE events (noter, CC, osv.)
    receivedData.Add(e.Event);
};
```

✅ **KORREKT:**
```csharp
midiIn.EventReceived += (sender, e) =>
{
    // Kun SysEx events er relevante
    if (e.Event is NormalSysExEvent sysEx)
    {
        receivedData.Add(sysEx.Data);
    }
};
```

### Timing
- **Bank Dump**: Beskeder kommer hurtigt - ingen manuel timing nødvendig
- **Send til Pedal**: 50-100ms pause mellem beskeder anbefales (ikke kritisk, men pænt)
- **Timeout**: Ingen timeout nødvendig - SysEx transmission er øjeblikkelig

### Data Format
- Python: `bytes` object → `list[int]` når læst
- C#: `byte[]` array direkte
- **INGEN** konvertering nødvendig mellem modtag og send!

---

## 6. TEST PROCEDURE

### Minimal Test (Valider Funktionalitet)

```csharp
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;

class NovaSystemMinimalTest
{
    static void Main()
    {
        // 1. Åbn devices
        var midiIn = InputDevice.GetByName("USB MIDI Interface 0");
        var midiOut = InputDevice.GetByName("USB MIDI Interface 1");
        
        var receivedData = new List<byte[]>();
        
        // 2. Setup modtagelse
        midiIn.EventReceived += (s, e) =>
        {
            if (e.Event is NormalSysExEvent sysEx)
            {
                receivedData.Add(sysEx.Data);
                Console.WriteLine($"Modtog #{receivedData.Count}");
            }
        };
        
        midiIn.StartEventsListening();
        
        // 3. Modtag bank dump
        Console.WriteLine("Tryk 'Dump Bank' på pedalen...");
        Console.ReadLine();
        
        Console.WriteLine($"Modtog {receivedData.Count} presets");
        
        // 4. Send første preset tilbage
        if (receivedData.Count > 0)
        {
            var sysEx = new NormalSysExEvent(receivedData[0]);
            midiOut.SendEvent(sysEx);
            Console.WriteLine("Sendt preset #1 tilbage!");
        }
        
        // 5. Cleanup
        midiIn.StopEventsListening();
        midiIn.Dispose();
        midiOut.Dispose();
    }
}
```

### Forventet Output
```
Tryk 'Dump Bank' på pedalen...
Modtog #1
Modtog #2
...
Modtog #60
Modtog 60 presets
Sendt preset #1 tilbage!
```

---

## 7. INTEGRATION I NOVASISYSTEMCONFIGURATOR APP

### Architecture Forslag

```csharp
public class NovaSystemMidiService
{
    private InputDevice _midiIn;
    private OutputDevice _midiOut;
    
    public event Action<int, byte[]> PresetReceived;
    public event Action<byte[]> SystemDumpReceived;
    
    public void Initialize()
    {
        _midiIn = InputDevice.GetByName("USB MIDI Interface 0");
        _midiOut = OutputDevice.GetByName("USB MIDI Interface 1");
        
        _midiIn.EventReceived += OnMidiEventReceived;
        _midiIn.StartEventsListening();
    }
    
    private void OnMidiEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        if (e.Event is NormalSysExEvent sysEx)
        {
            // Dispatch til UI eller storage
            PresetReceived?.Invoke(_counter++, sysEx.Data);
        }
    }
    
    public void SendPreset(byte[] presetData)
    {
        var sysEx = new NormalSysExEvent(presetData);
        _midiOut.SendEvent(sysEx);
    }
    
    public void SendBank(List<byte[]> presets)
    {
        foreach (var preset in presets)
        {
            SendPreset(preset);
            Thread.Sleep(50);
        }
    }
    
    public void Dispose()
    {
        _midiIn?.StopEventsListening();
        _midiIn?.Dispose();
        _midiOut?.Dispose();
    }
}
```

### UI Integration (Avalonia)

```csharp
// ViewModel
public class MidiViewModel : ViewModelBase
{
    private NovaSystemMidiService _midiService;
    
    public ObservableCollection<PresetViewModel> ReceivedPresets { get; }
    
    public ICommand StartReceivingCommand { get; }
    public ICommand SendPresetCommand { get; }
    
    private void OnPresetReceived(int index, byte[] data)
    {
        // Dispatcher til UI thread
        Dispatcher.UIThread.Post(() =>
        {
            ReceivedPresets.Add(new PresetViewModel 
            { 
                Number = index,
                Data = data,
                Size = data.Length
            });
        });
    }
}
```

---

## 8. PERSISTENCE STRATEGI

### File Structure
```
AppData/
  NovaSystem/
    Banks/
      Bank_01/
        Preset_01.syx
        Preset_02.syx
        ...
        Preset_60.syx
      Bank_02/
        ...
    System/
      SystemDump_2026-02-06.syx
```

### Save/Load Implementation
```csharp
public class NovaSystemStorage
{
    private const string BASE_PATH = "AppData/NovaSystem";
    
    public void SaveBank(string bankName, List<byte[]> presets)
    {
        var bankPath = Path.Combine(BASE_PATH, "Banks", bankName);
        Directory.CreateDirectory(bankPath);
        
        for (int i = 0; i < presets.Count; i++)
        {
            var fileName = $"Preset_{i+1:D2}.syx";
            File.WriteAllBytes(Path.Combine(bankPath, fileName), presets[i]);
        }
    }
    
    public List<byte[]> LoadBank(string bankName)
    {
        var bankPath = Path.Combine(BASE_PATH, "Banks", bankName);
        var files = Directory.GetFiles(bankPath, "*.syx")
                            .OrderBy(f => f)
                            .ToList();
        
        return files.Select(File.ReadAllBytes).ToList();
    }
}
```

---

## 9. AFSLUTTENDE NOTER

### Hvad Vi Har Lært
✅ MIDI kommunikation med Nova System er **simpel** og **pålidelig**
✅ SysEx beskeder er **rå binær data** - ingen parsing nødvendig
✅ Device selection er **KRITISK** - forkert device = total blokering
✅ Python og C# implementation er **næsten identiske** i struktur

### Næste Skridt
1. Implementer `NovaSystemMidiService` klasse i C# projektet
2. Test med minimal console app først (som Python test)
3. Integrer i Avalonia UI når MIDI fungerer stabilt
4. Tilføj persistence (gem/load .syx filer)
5. Implementer LLM-baseret preset generation feature

### Test Checklist
- [ ] Kan åbne korrekte MIDI devices
- [ ] Kan modtage Bank Dump (60 presets)
- [ ] Kan sende enkelt preset tilbage
- [ ] Kan modtage System Dump (1 besked)
- [ ] Kan sende System Dump tilbage
- [ ] Kan gemme presets til disk
- [ ] Kan loade presets fra disk
- [ ] Kan sende komplet bank (60 presets)

---

## APPENDIX: PYTHON TEST KODE (Reference)

```python
import mido

midi_in = mido.open_input('USB MIDI Interface 0')
midi_out = mido.open_output('USB MIDI Interface 1')

received_data = []

def receive_callback(msg):
    if msg.type == 'sysex':
        received_data.append(msg.data)

midi_in.callback = receive_callback

# Modtag
input("Tryk 'Dump Bank' på pedalen...")
print(f"Modtog {len(received_data)} presets")

# Send retur
if len(received_data) > 0:
    msg = mido.Message('sysex', data=received_data[0])
    midi_out.send(msg)
    print("Sendt!")

midi_in.close()
midi_out.close()
```

---

**Dokument oprettet: 2026-02-06**
**Valideret via Python test på Windows PC med Nova System hardware**
**Klar til C# implementation i NovaSystemConfigurator projekt**
