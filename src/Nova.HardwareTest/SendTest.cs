using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

namespace Nova.HardwareTest;

public class SendTest
{
    public static async Task SendSysExFile(string[] args)
    {
        Console.WriteLine("=== Nova System MIDI Sender ===\n");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run --no-build -- <path-to-syx-file>");
            Console.WriteLine("Example: dotnet run --no-build -- nova-dump-20260131-181507-msg001.syx");
            return;
        }

        var filePath = args[0];
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"❌ File not found: {filePath}");
            return;
        }

        // Read SysEx file
        var sysexData = await File.ReadAllBytesAsync(filePath);
        Console.WriteLine($"📂 Loaded: {filePath} ({sysexData.Length} bytes)");
        Console.WriteLine($"   First bytes: {BitConverter.ToString(sysexData.Take(10).ToArray())}");

        // Parse optional --midi-out=<name> flag
        var midiOutArg = args.FirstOrDefault(a => a.StartsWith("--midi-out="));
        string? midiOutName = midiOutArg != null ? midiOutArg.Split('=')[1] : null;
        var midiOutIndexArg = args.FirstOrDefault(a => a.StartsWith("--midi-out-index="));
        int? midiOutIndex = midiOutIndexArg != null && int.TryParse(midiOutIndexArg.Split('=')[1], out var mo) ? mo : null;

        if (string.IsNullOrWhiteSpace(midiOutName) && !midiOutIndex.HasValue)
        {
            Console.WriteLine("\n❌ You must provide --midi-out or --midi-out-index.");
            return;
        }

        // Enumerate output devices
        var outputDevices = OutputDevice.GetAll().ToList();
        Console.WriteLine("\nAvailable MIDI Output Devices:");
        for (int i = 0; i < outputDevices.Count; i++)
        {
            Console.WriteLine($"  [{i}] {outputDevices[i].Name}");
        }

        OutputDevice? chosen = null;
        if (midiOutIndex.HasValue)
        {
            if (midiOutIndex.Value < 0 || midiOutIndex.Value >= outputDevices.Count)
            {
                Console.WriteLine($"\n❌ MIDI OUT index {midiOutIndex.Value} out of range.");
                return;
            }
            chosen = outputDevices[midiOutIndex.Value];
        }
        else if (!string.IsNullOrEmpty(midiOutName))
        {
            chosen = outputDevices.FirstOrDefault(d => d.Name.Equals(midiOutName, StringComparison.OrdinalIgnoreCase))
                ?? outputDevices.FirstOrDefault(d => d.Name.IndexOf(midiOutName, StringComparison.OrdinalIgnoreCase) >= 0);
            if (chosen == null)
            {
                Console.WriteLine($"\n❌ MIDI OUT device matching '{midiOutName}' not found.");
                return;
            }
        }
        else
        {
            Console.WriteLine("\n❌ MIDI OUT not specified.");
            return;
        }

        if (chosen == null)
        {
            Console.WriteLine("\n❌ No MIDI output devices available. Please connect a MIDI output and retry.");
            return;
        }

        Console.WriteLine($"\n✅ Using: {chosen.Name}");

        // Send SysEx
        using (var output = chosen)
        {
            Console.WriteLine("\n📤 Sending SysEx to Nova System...");

            // DryWetMIDI expects data WITHOUT F0/F7 delimiters
            byte[] dataWithoutDelimiters;
            if (sysexData[0] == 0xF0 && sysexData[sysexData.Length - 1] == 0xF7)
            {
                dataWithoutDelimiters = sysexData.Skip(1).Take(sysexData.Length - 2).ToArray();
                Console.WriteLine($"   Stripped F0/F7 delimiters: {dataWithoutDelimiters.Length} bytes");
            }
            else
            {
                dataWithoutDelimiters = sysexData;
                Console.WriteLine($"   Data already without delimiters: {dataWithoutDelimiters.Length} bytes");
            }

            try
            {
                output.SendEvent(new NormalSysExEvent(dataWithoutDelimiters));
                Console.WriteLine("✅ SysEx sent successfully!");
                Console.WriteLine("\n🎸 Check the Nova System pedal to see if preset was loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending SysEx: {ex.Message}");
                Console.WriteLine($"   {ex.GetType().Name}");
            }
        }

        Console.WriteLine("\n=== Send Complete ===");
    }
}
