using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Nova.MinimalMidiTest;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Nova System MIDI Test (Python Copy) ===");
        Console.WriteLine();

        InputDevice? midiIn = null;
        OutputDevice? midiOut = null;

        try
        {
            // Step 1: Open MIDI devices with EXACT names
            Console.WriteLine("Opening MIDI devices...");

            midiIn = InputDevice.GetByName("USB MIDI Interface 0");
            Console.WriteLine("✓ Input:  USB MIDI Interface 0");

            midiOut = OutputDevice.GetByName("USB MIDI Interface 1");
            Console.WriteLine("✓ Output: USB MIDI Interface 1");
            Console.WriteLine();

            // Step 2: Setup event handler for SysEx events
            var receivedData = new List<byte[]>();

            midiIn.EventReceived += (sender, e) =>
            {
                // Check for NormalSysExEvent
                if (e.Event is NormalSysExEvent sysEx)
                {
                    receivedData.Add(sysEx.Data);
                    Console.WriteLine($"Received #{receivedData.Count}: {sysEx.Data.Length} bytes");
                }
            };

            // Step 3: Start listening - CRITICAL!
            midiIn.StartEventsListening();
            Console.WriteLine("✓ Started listening for SysEx events");
            Console.WriteLine();

            // Step 4: Wait for user to dump from pedal (PASSIVE receive)
            Console.WriteLine("=== STEP 1: RECEIVE BANK DUMP ===");
            Console.WriteLine("Go to pedal: UTILITY → MIDI → Send Dump → User Bank");
            Console.Write("Press ENTER when dump is complete...");
            Console.ReadLine();
            Console.WriteLine();

            // Validate received data
            Console.WriteLine($"✓ Received {receivedData.Count} presets");
            Console.WriteLine();

            if (receivedData.Count == 0)
            {
                Console.WriteLine("ERROR: No data received. Make sure you performed the dump on the pedal.");
                return;
            }

            // Step 5: Send first preset back
            Console.WriteLine("=== STEP 2: SEND PRESET BACK ===");
            Console.WriteLine("Sending preset #1 back to pedal...");

            var sysExEvent = new NormalSysExEvent(receivedData[0]);
            midiOut.SendEvent(sysExEvent);

            Console.WriteLine("✓ Sent preset #1 back!");
            Console.WriteLine();

            // Step 6: Optional - send all presets back
            Console.WriteLine("=== STEP 3: SEND ALL PRESETS BACK (OPTIONAL) ===");
            Console.Write("Send all {0} presets back? (y/n): ", receivedData.Count);
            var response = Console.ReadLine()?.Trim().ToLower();

            if (response == "y" || response == "yes")
            {
                Console.WriteLine();
                Console.WriteLine("Sending all presets back...");

                for (int i = 0; i < receivedData.Count; i++)
                {
                    var sysEx = new NormalSysExEvent(receivedData[i]);
                    midiOut.SendEvent(sysEx);
                    Console.WriteLine($"Sent preset #{i + 1}/{receivedData.Count}");

                    // 50ms delay between messages
                    Thread.Sleep(50);
                }

                Console.WriteLine();
                Console.WriteLine($"✓ Sent all {receivedData.Count} presets back!");
            }
            else
            {
                Console.WriteLine("Skipped sending all presets.");
            }

            Console.WriteLine();
            Console.WriteLine("=== TEST COMPLETE ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Common issues:");
            Console.WriteLine("- Make sure USB MIDI Interface is connected");
            Console.WriteLine("- Check that device names are exactly: 'USB MIDI Interface 0' and 'USB MIDI Interface 1'");
            Console.WriteLine("- Restart the Nova System pedal if MIDI communication is blocked");
        }
        finally
        {
            // Step 7: Cleanup
            if (midiIn != null)
            {
                midiIn.StopEventsListening();
                midiIn.Dispose();
            }

            midiOut?.Dispose();
        }
    }
}
