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

        // Find USB MIDI Interface output
        var outputDevices = OutputDevice.GetAll().ToList();
        Console.WriteLine("\nAvailable MIDI Output Devices:");
        for (int i = 0; i < outputDevices.Count; i++)
        {
            Console.WriteLine($"  [{i}] {outputDevices[i].Name}");
        }

        var usbOutput = outputDevices.FirstOrDefault(d =>
            d.Name.Contains("USB MIDI", StringComparison.OrdinalIgnoreCase));

        if (usbOutput == null)
        {
            Console.WriteLine("\n❌ USB MIDI Interface not found!");
            return;
        }

        Console.WriteLine($"\n✅ Using: {usbOutput.Name}");

        // Send SysEx
        using (var output = usbOutput)
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
