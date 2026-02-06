using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using Nova.Domain.Midi;
using Nova.HardwareTest;
using System.Linq;

if (args.Length == 0)
{
    PrintUsage();
    return;
}

string? midiInName = GetArgValue(args, "--midi-in=");
string? midiOutName = GetArgValue(args, "--midi-out=");
int? midiInIndex = GetIntArg(args, "--midi-in-index=");
int? midiOutIndex = GetIntArg(args, "--midi-out-index=");
int timeoutMs = GetIntArg(args, "--timeout=") ?? 30000;
int delayMs = GetIntArg(args, "--delay=") ?? 500;
string? dumpsDir = GetArgValue(args, "--dumps=");

// Send renamed bank: dotnet run -- --send-renamed-bank [--delay=ms] [--dry-run] [--dumps=path] --midi-out=name [--verify --midi-in=name]
if (args.Contains("--send-renamed-bank"))
{
    bool dryRun = args.Contains("--dry-run");
    bool verify = args.Contains("--verify");

    if (!RequirePorts(midiInName, midiOutName, midiInIndex, midiOutIndex))
    {
        return;
    }

    await SendRenamedBank.Execute(
        dumpsDir,
        delayMs,
        dryRun,
        midiOutName,
        verify,
        midiInName,
        midiOutIndex,
        midiInIndex);
    return;
}

// List devices and quick diagnostics
if (args.Contains("--list-devices"))
{
    var inputs = InputDevice.GetAll().ToList();
    var outputs = OutputDevice.GetAll().ToList();

    Console.WriteLine("\n=== MIDI INPUT DEVICES ===");
    for (int i = 0; i < inputs.Count; i++)
    {
        var n = inputs[i];
        Console.WriteLine($"  [{i}] {n.Name} (normalized: '{HardwareUtils.NormalizeDeviceName(n.Name)}'){(HardwareUtils.IsBlacklisted(n.Name) ? "  <-- BLACKLISTED" : "")}");
    }

    Console.WriteLine("\n=== MIDI OUTPUT DEVICES ===");
    for (int i = 0; i < outputs.Count; i++)
    {
        var o = outputs[i];
        Console.WriteLine($"  [{i}] {o.Name} (normalized: '{HardwareUtils.NormalizeDeviceName(o.Name)}'){(HardwareUtils.IsBlacklisted(o.Name) ? "  <-- BLACKLISTED" : "")}");
    }

    var pair = HardwareUtils.ResolvePair(null, null);
    Console.WriteLine($"\nSuggested pairing: IN='{pair.input?.Name ?? "(none)"}' OUT='{pair.output?.Name ?? "(none)"}'");

    return;
}

if (args.Contains("--pair-test"))
{
    if (!HardwareTestMatrix.TryResolveDevices(midiInName, midiOutName, midiInIndex, midiOutIndex, out var input, out var output))
    {
        return;
    }

    using (input)
    using (output)
    {
        await HardwareTestMatrix.PairTest(input, output);
    }

    return;
}

if (args.Contains("--request-system-dump"))
{
    if (!HardwareTestMatrix.TryResolveDevices(midiInName, midiOutName, midiInIndex, midiOutIndex, out var input, out var output))
    {
        return;
    }

    using (input)
    using (output)
    {
        await HardwareTestMatrix.RequestSystemDump(input, output, timeoutMs);
    }

    return;
}

if (args.Contains("--request-bank-dump"))
{
    if (!HardwareTestMatrix.TryResolveDevices(midiInName, midiOutName, midiInIndex, midiOutIndex, out var input, out var output))
    {
        return;
    }

    using (input)
    using (output)
    {
        await HardwareTestMatrix.RequestBankDump(input, output, timeoutMs);
    }

    return;
}

if (args.Contains("--roundtrip-bank"))
{
    if (!HardwareTestMatrix.TryResolveDevices(midiInName, midiOutName, midiInIndex, midiOutIndex, out var input, out var output))
    {
        return;
    }

    using (input)
    using (output)
    {
        await HardwareTestMatrix.RoundtripBank(input, output, timeoutMs, delayMs);
    }

    return;
}

if (args.Contains("--cc-learn"))
{
    if (!HardwareTestMatrix.TryResolveDevices(midiInName, midiOutName, midiInIndex, midiOutIndex, out var input, out var output))
    {
        return;
    }

    using (input)
    using (output)
    {
        await HardwareTestMatrix.CcLearn(input, timeoutMs);
    }

    return;
}

if (args.Contains("--disconnect-reconnect"))
{
    if (!HardwareTestMatrix.TryResolveDevices(midiInName, midiOutName, midiInIndex, midiOutIndex, out var input, out var output))
    {
        return;
    }

    var inputName = input.Name;
    var outputName = output.Name;
    input.Dispose();
    output.Dispose();

    await HardwareTestMatrix.DisconnectReconnect(inputName, outputName, delayMs);
    return;
}

// Listen with required --midi-in and --midi-out
if (args.Contains("--listen"))
{
    if (!HardwareTestMatrix.TryResolveDevices(midiInName, midiOutName, midiInIndex, midiOutIndex, out var input, out var output))
    {
        return;
    }

    using (output)
    {
        await Listen(input);
    }

    return;
}

// Otherwise treat args as SendTest file path + optional flags
await SendTest.SendSysExFile(args);

static async Task Listen(InputDevice input)
{
    Console.WriteLine("=== Nova System MIDI Listener ===\n");
    Console.WriteLine("This program will record SysEx dumps sent FROM the Nova System pedal.\n");

    Console.WriteLine($"✅ Using MIDI Input: {input.Name}");

    // Step 3: Start listening for SysEx data
    var receivedMessages = new List<byte[]>();
    var receiveComplete = new TaskCompletionSource<bool>();

    using (input)
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

            var dumpDir = Path.Combine(Environment.CurrentDirectory, "Dumps");
            Directory.CreateDirectory(dumpDir);

            // Save each message
            for (int i = 0; i < receivedMessages.Count; i++)
            {
                var data = receivedMessages[i];
                var filename = $"nova-dump-{DateTime.Now:yyyyMMdd-HHmmss}-msg{i + 1:D3}.syx";
                var outputPath = Path.Combine(dumpDir, filename);

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
}

static string? GetArgValue(string[] args, string prefix)
    => args.FirstOrDefault(a => a.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))?.Split('=')[1];

static int? GetIntArg(string[] args, string prefix)
{
    var arg = GetArgValue(args, prefix);
    return arg != null && int.TryParse(arg, out var value) ? value : null;
}

static bool RequirePorts(string? midiInName, string? midiOutName, int? midiInIndex, int? midiOutIndex)
{
    if ((string.IsNullOrWhiteSpace(midiInName) && !midiInIndex.HasValue)
        || (string.IsNullOrWhiteSpace(midiOutName) && !midiOutIndex.HasValue))
    {
        Console.WriteLine("❌ You must provide both --midi-in and --midi-out (or indexes). Use --list-devices to inspect names.");
        return false;
    }

    return true;
}

static void PrintUsage()
{
    Console.WriteLine("Nova Hardware Tests (require --midi-in and --midi-out for all tests)\n");
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --list-devices");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --pair-test --midi-in=\"MIDI 1\" --midi-out=\"MIDI 0\"");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --request-system-dump --midi-in=\"MIDI 1\" --midi-out=\"MIDI 0\"");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --request-bank-dump --midi-in=\"MIDI 1\" --midi-out=\"MIDI 0\"");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --roundtrip-bank --midi-in=\"MIDI 1\" --midi-out=\"MIDI 0\"");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --cc-learn --midi-in=\"MIDI 1\" --midi-out=\"MIDI 0\"");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --disconnect-reconnect --midi-in=\"MIDI 1\" --midi-out=\"MIDI 0\"");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --listen --midi-in=\"MIDI 1\" --midi-out=\"MIDI 0\"");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- --send-renamed-bank --midi-out=\"MIDI 0\" --midi-in=\"MIDI 1\" --verify");
    Console.WriteLine("  dotnet run --project src/Nova.HardwareTest -- <path-to-syx-file> --midi-out=\"MIDI 0\"");
}
