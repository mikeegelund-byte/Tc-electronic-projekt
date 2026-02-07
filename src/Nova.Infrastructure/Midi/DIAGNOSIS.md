# DryWetMidiPort Diagnosis

## Checklist - Compare with Working Test

### Device Opening
- [x] Uses exact same device names? - YES, uses GetByName (working test) vs FindInputDevice/FindOutputDevice (DryWetMidiPort)
- [x] Opens input device correctly? - YES, both open InputDevice
- [x] Opens output device correctly? - YES, both open OutputDevice

### Event Subscription
- [x] Subscribes to EventReceived? - YES, both subscribe to EventReceived
- [x] Checks for NormalSysExEvent type? - YES, both check for NormalSysExEvent
- [x] Calls StartEventsListening()? - **YES, BUT CRITICAL TIMING DIFFERENCE!**

### Receiving SysEx
- [x] Returns IAsyncEnumerable or similar? - Working test uses List<byte[]>, DryWetMidiPort uses IAsyncEnumerable via Channel
- [x] Buffers received data? - Working test uses List, DryWetMidiPort uses Channel
- [x] Handles multiple rapid messages? - Both should handle this

### Sending SysEx
- [x] Creates NormalSysExEvent? - YES, both create NormalSysExEvent
- [x] Calls SendEvent()? - YES, both call SendEvent
- [x] Handles errors? - YES, DryWetMidiPort wraps in try/catch with Result

### Threading
- [x] Event handler runs on correct thread? - Both use lambda/method event handlers
- [x] No race conditions in buffer? - DryWetMidiPort uses thread-safe Channel
- [x] Proper async/await? - DryWetMidiPort uses async IAsyncEnumerable

### Cleanup
- [x] StopEventsListening() called? - YES, both call StopEventsListening in cleanup
- [x] Devices disposed properly? - YES, both dispose devices

## Findings

### Critical Differences Found:

#### 1. **CRITICAL: Event Subscription Timing Order**

**Working Test (Program.cs):**
```csharp
// Lines 31-39: Subscribe to EventReceived FIRST
midiIn.EventReceived += (sender, e) =>
{
    if (e.Event is NormalSysExEvent sysEx)
    {
        receivedData.Add(sysEx.Data);
    }
};

// Line 42: THEN start listening
midiIn.StartEventsListening();
```

**DryWetMidiPort.cs:**
```csharp
// Line 75: Calls StartEventsListening() in ConnectAsync()
_inputDevice.StartEventsListening();

// BUT event handler is NOT subscribed yet!

// Lines 167-171: Event handler is only subscribed later when ReceiveSysExAsync() is called
if (!_handlersSubscribed)
{
    _inputDevice.EventReceived += OnEventReceived;
    _handlersSubscribed = true;
}
```

**Problem:** StartEventsListening() is called BEFORE the event handler is subscribed. This means:
- Events start being processed immediately
- But there's no handler attached yet to receive them
- Events arriving between StartEventsListening() and ReceiveSysExAsync() are LOST

#### 2. **Lazy Event Handler Subscription**

**Working Test:**
- Event handler is subscribed immediately during setup (line 31)
- StartEventsListening() is called after handler is ready (line 42)
- This ensures NO events are missed

**DryWetMidiPort:**
- Event handler is only subscribed when ReceiveSysExAsync() is called (line 169)
- StartEventsListening() was already called during ConnectAsync() (line 75)
- There's a **time window** where events can arrive but won't be captured

#### 3. **Device Name Matching**

**Working Test:**
```csharp
// Line 21: Uses exact name match
midiIn = InputDevice.GetByName("USB MIDI Interface 0");
```

**DryWetMidiPort:**
```csharp
// Lines 254-258: Uses fuzzy matching with fallback
return devices.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
    ?? devices.FirstOrDefault(d => d.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
```

**Impact:** Minor - fuzzy matching is actually more robust, but could match wrong device if names are similar

#### 4. **SysEx Data Framing**

**Working Test:**
```csharp
// Line 36: Uses raw SysEx.Data (no F0/F7 framing added on receive)
receivedData.Add(sysEx.Data);

// Line 67: Sends raw SysEx.Data (no F0/F7 stripping on send)
var sysExEvent = new NormalSysExEvent(receivedData[0]);
```

**DryWetMidiPort:**
```csharp
// Lines 183-186: ADDS F0/F7 framing on receive
var data = new byte[sysEx.Data.Length + 2];
data[0] = 0xF0;
Array.Copy(sysEx.Data, 0, data, 1, sysEx.Data.Length);
data[^1] = 0xF7;

// Lines 140-142: STRIPS F0/F7 framing on send
var dataWithoutFrame = sysex.Length > 2 && sysex[0] == 0xF0 && sysex[^1] == 0xF7
    ? sysex[1..^1]
    : sysex;
```

**Impact:** This is actually CORRECT behavior - DryWetMidiPort compensates for DryWetMIDI's internal behavior. The working test works because it uses the same data format consistently (no framing).

#### 5. **Buffering Strategy**

**Working Test:**
```csharp
// Line 29: Simple List collection
var receivedData = new List<byte[]>();
```

**DryWetMidiPort:**
```csharp
// Line 163: Channel-based async streaming
_sysExChannel = Channel.CreateUnbounded<byte[]>();
```

**Impact:** Minor - Channel is thread-safe and more suitable for async streaming, but adds complexity

### Root Cause Hypothesis:

**PRIMARY ISSUE:** The event handler subscription happens TOO LATE.

When using DryWetMidiPort:
1. ConnectAsync() is called
2. Devices are opened
3. **StartEventsListening() is called** (line 75)
4. DryWetMIDI starts capturing MIDI events internally
5. User later calls ReceiveSysExAsync()
6. **Event handler is subscribed** (line 169)
7. BUT events that arrived in step 4-6 are ALREADY LOST

The working test avoids this by:
1. Opening devices
2. **Subscribing event handler FIRST**
3. **THEN calling StartEventsListening()**
4. All events are captured from the moment listening starts

**SECONDARY ISSUE:** There's a potential race condition if the caller:
1. Calls ConnectAsync() - starts listening
2. Doesn't immediately call ReceiveSysExAsync() - handler not subscribed yet
3. User performs dump on pedal - events arrive and are LOST
4. Then calls ReceiveSysExAsync() - too late, events already missed

### Fixes Required:

1. **CRITICAL: Move event handler subscription to ConnectAsync()**
   - Subscribe to EventReceived BEFORE calling StartEventsListening()
   - Initialize _sysExChannel and _ccChannel in ConnectAsync()
   - Set _handlersSubscribed = true in ConnectAsync()
   - Remove lazy initialization from ReceiveSysExAsync() and ReceiveCCAsync()

2. **Verify StartEventsListening() is called AFTER subscription**
   - Current code has it in the right place (line 75) but handler not yet subscribed
   - Move subscription logic before line 75

3. **Ensure channels are initialized early**
   - Initialize _sysExChannel and _ccChannel during ConnectAsync()
   - Not during first ReceiveSysExAsync()/ReceiveCCAsync() call

4. **Optional: Consider removing lazy initialization pattern entirely**
   - Current lazy pattern creates the timing window issue
   - Eager initialization would be safer

### Implementation Plan for Task 3:

Move this code block:
```csharp
// FROM ReceiveSysExAsync() lines 160-171
if (_sysExChannel == null)
{
    _sysExChannel = Channel.CreateUnbounded<byte[]>();
}

if (!_handlersSubscribed)
{
    _inputDevice.EventReceived += OnEventReceived;
    _handlersSubscribed = true;
}
```

TO ConnectAsync() BEFORE line 75:
```csharp
// After line 74 (_outputDevice = output;)
// Initialize channels
_sysExChannel = Channel.CreateUnbounded<byte[]>();
_ccChannel = Channel.CreateUnbounded<byte[]>();

// Subscribe event handler BEFORE starting
_inputDevice.EventReceived += OnEventReceived;
_handlersSubscribed = true;

// NOW safe to start listening (line 75)
_inputDevice.StartEventsListening();
```

Also update ReceiveSysExAsync() and ReceiveCCAsync() to remove lazy initialization and just verify state:
```csharp
public IAsyncEnumerable<byte[]> ReceiveSysExAsync(CancellationToken cancellationToken = default)
{
    if (_inputDevice == null || _sysExChannel == null)
        throw new InvalidOperationException("Not connected");

    return ReadSysExAsync(cancellationToken);
}
```

### Testing Strategy:

After implementing fixes:
1. Build and run existing app with fixed DryWetMidiPort
2. Connect to MIDI devices
3. Immediately perform bank dump from pedal (don't wait)
4. Verify all SysEx messages are received
5. Verify messages can be sent back successfully
6. Compare behavior with working MinimalMidiTest
