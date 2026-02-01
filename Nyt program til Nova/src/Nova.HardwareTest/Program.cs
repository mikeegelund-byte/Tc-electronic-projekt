using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using Nova.Domain.Midi;
using Nova.HardwareTest;

// Check if user wants to send a file instead of listen
if (args.Length > 0)
{
    await SendTest.SendSysExFile(args);
    return;
}

Console.WriteLine("=== Nova System MIDI Listener ===\n");
Console.WriteLine("This program will capture SysEx dumps sent FROM the Nova System pedal.\n");

// Step 1: List all available MIDI input devices
Console.WriteLine("Available MIDI Input Devices:");
var inputDevices = InputDevice.GetAll().ToList();
for (int i = 0; i < inputDevices.Count; i++)
{
    Console.WriteLine($"  [{i}] {inputDevices[i].Name}");
}

// Step 2: Find USB MIDI Interface (Nova System connection)
var novaInput = inputDevices.FirstOrDefault(d =>
    d.Name.Contains("USB MIDI", StringComparison.OrdinalIgnoreCase));

if (novaInput == null)
{
    Console.WriteLine("\n❌ USB MIDI Interface not found! Please:");
    Console.WriteLine("   1. Connect Nova System via USB-MIDI cable");
    Console.WriteLine("   2. Ensure MIDI drivers are installed");
    Console.WriteLine("   3. Power on the Nova System");
    return;
}

Console.WriteLine($"\n✅ Found USB MIDI Interface: {novaInput.Name}");

// Step 3: Start listening for SysEx data
var receivedMessages = new List<byte[]>();
var receiveComplete = new TaskCompletionSource<bool>();

using (var input = novaInput)
{
    input.EventReceived += (sender, e) =>
    {
        if (e.Event is NormalSysExEvent sysExEvent)
        {
            var data = sysExEvent.Data;
            Console.WriteLine($"\n📥 Received SysEx: {data.Length} bytes");
            Console.WriteLine($"   First bytes: {BitConverter.ToString(data.Take(10).ToArray())}");

            receivedMessages.Add(data);

            // Check message type: byte 6 = 0x02 means System Dump (single file)
            bool isSystemDump = data.Length > 6 && data[6] == 0x02;

            if (isSystemDump)
            {
                Console.WriteLine($"\n✅ System Dump received - complete!");
                receiveComplete.TrySetResult(true);
            }
            // Check if this looks like a complete bank dump (multiple presets)
            else if (receivedMessages.Count >= 60 || data.Length > 30000)
            {
                Console.WriteLine($"\n✅ Received {receivedMessages.Count} messages - looks complete!");
                receiveComplete.TrySetResult(true);
            }
        }
    };

    input.StartEventsListening();

Console.WriteLine("\n📡 Listening for MIDI data...");
Console.WriteLine("\n🎸 ON THE PEDAL:");
Console.WriteLine("   1. Press UTILITY button");
    Console.WriteLine("   2. Navigate to MIDI menu");
    Console.WriteLine("   3. Select 'Send Dump' or 'Transmit Bank'");
    Console.WriteLine("   4. Confirm to send data");
    Console.WriteLine("\nWaiting for data... (Press Ctrl+C to stop)");

    // Wait for data (60 second timeout)
    var completed = await Task.WhenAny(receiveComplete.Task, Task.Delay(60000));

    if (completed == receiveComplete.Task)
    {
        Console.WriteLine($"\n✅ Capture complete! Received {receivedMessages.Count} SysEx messages");

        var captureDir = Path.Combine(Environment.CurrentDirectory, "Captures");
        Directory.CreateDirectory(captureDir);

        // Save each message
        for (int i = 0; i < receivedMessages.Count; i++)
        {
            var data = receivedMessages[i];
            var filename = $"nova-dump-{DateTime.Now:yyyyMMdd-HHmmss}-msg{i + 1:D3}.syx";
            var outputPath = Path.Combine(captureDir, filename);

            // Add F0 and F7 delimiters for standard .syx file format
            var fullSysEx = new byte[] { 0xF0 }.Concat(data).Concat(new byte[] { 0xF7 }).ToArray();
            await File.WriteAllBytesAsync(outputPath, fullSysEx);

            Console.WriteLine($"\n💾 Message {i + 1}: {outputPath} ({data.Length} bytes)");
            Console.WriteLine($"   Start: {BitConverter.ToString(data.Take(8).ToArray())}");

            // Validate if it's a preset (518 bytes without delimiters)
            if (data.Length == 518)
            {
                // Add back F0/F7 for validation
                var withDelimiters = new byte[] { 0xF0 }.Concat(data).Concat(new byte[] { 0xF7 }).ToArray();
                var isValid = SysExValidator.ValidateChecksum(withDelimiters);
                Console.WriteLine($"   Checksum: {(isValid ? "✅ VALID" : "❌ INVALID")}");
            }
        }
    }
    else
    {
        Console.WriteLine("\n⏱️ Timeout - no data received after 60 seconds");
        Console.WriteLine("   Make sure you:");
        Console.WriteLine("   - Sent the dump from the pedal");
        Console.WriteLine("   - USB cable is connected properly");
        Console.WriteLine("   - MIDI settings on pedal: SysEx ID = 0");
    }
}

Console.WriteLine("\n=== Test Complete ===");

Console.WriteLine("\n=== Test Complete ===");
