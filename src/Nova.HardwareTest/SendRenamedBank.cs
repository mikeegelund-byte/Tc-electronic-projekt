using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using Nova.Domain.Midi;
using Nova.Domain.Models;

namespace Nova.HardwareTest;

public static class SendRenamedBank
{
    /// <summary>
    /// Reads SysEx preset files from the Dumps directory, renames each to "Preset {i}",
    /// updates slot number and checksum, and sends them to the Nova System pedal.
    /// </summary>
    public static async Task Execute(string? dumpsDir = null, int delayMs = 1000, bool dryRun = false, string? midiOutName = null, bool verify = false, string? midiInName = null, int? midiOutIndex = null, int? midiInIndex = null)
    {
        dumpsDir ??= Path.Combine(Environment.CurrentDirectory, "Dumps");

        if (string.IsNullOrWhiteSpace(midiOutName) && !midiOutIndex.HasValue)
        {
            Console.WriteLine("❌ You must provide --midi-out or --midi-out-index.");
            return;
        }

        if (verify && string.IsNullOrWhiteSpace(midiInName) && !midiInIndex.HasValue)
        {
            Console.WriteLine("❌ Verification requires --midi-in or --midi-in-index.");
            return;
        }

        if (!Directory.Exists(dumpsDir))
        {
            Console.WriteLine($"❌ Dumps directory not found: {dumpsDir}");
            return;
        }

        var files = Directory.GetFiles(dumpsDir, "nova-dump-*-msg*.syx")
            .OrderBy(f => f)
            .ToArray();

        if (files.Length == 0)
        {
            Console.WriteLine("❌ No dump files found in Dumps/ - run the listener to capture them first.");
            return;
        }

        Console.WriteLine($"Found {files.Length} dump files. Preparing to send renamed bank (1..{files.Length}).");

        // Choose output device (allow specifying --midi-out name OR --midi-out-index)
        var outputDevices = OutputDevice.GetAll().ToList();
        Console.WriteLine("\nAvailable MIDI Output Devices:");
        for (int i = 0; i < outputDevices.Count; i++) Console.WriteLine($"  [{i}] {outputDevices[i].Name}");

        OutputDevice? chosen = null;
        if (midiOutIndex.HasValue)
        {
            if (midiOutIndex.Value < 0 || midiOutIndex.Value >= outputDevices.Count)
            {
                Console.WriteLine($"\n❌ MIDI OUT index {midiOutIndex.Value} out of range");
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

        Console.WriteLine($"✅ Using output device: {chosen.Name}");

        using (var output = chosen)
        {
            for (int i = 0; i < files.Length && i < 60; i++)
            {
                int slotNumber = i + 1; // 1-based slot
                var path = files[i];
                var sysexFull = await File.ReadAllBytesAsync(path);

                Console.WriteLine($"\n📂 File {i + 1}: {Path.GetFileName(path)} ({sysexFull.Length} bytes)");

                // Ensure it has delimiters
                if (sysexFull[0] != 0xF0 || sysexFull[sysexFull.Length - 1] != 0xF7)
                {
                    Console.WriteLine("   ⚠️ File missing F0/F7 delimiters - skipping");
                    continue;
                }

                // Create a mutable copy
                var sysex = (byte[])sysexFull.Clone();

                // Update slot number (byte 8)
                sysex[8] = (byte)slotNumber;

                // Set name (bytes 10-33 = 24 ASCII chars; byte 9 reserved)
                var newName = $"Preset {slotNumber}";
                var nameBytes = System.Text.Encoding.ASCII.GetBytes(newName.PadRight(24));
                Array.Copy(nameBytes, 0, sysex, 10, 24);

                // Recalculate checksum (sum bytes 34-517 & 0x7F) -> byte 518
                int checksum = 0;
                for (int b = 34; b <= 517; b++)
                {
                    checksum += sysex[b];
                }
                sysex[518] = (byte)(checksum & 0x7F);

                // Validate by parsing into Preset (this also confirms format)
                var parsed = Preset.FromSysEx(sysex);
                if (parsed.IsFailed)
                {
                    Console.WriteLine($"   ❌ Failed to parse modified preset: {string.Join("; ", parsed.Errors.Select(e=>e.Message))}");
                    continue;
                }

                Console.WriteLine($"   ✅ Prepared preset slot {slotNumber} - Name: '{parsed.Value.Name}'");

                if (dryRun)
                {
                    Console.WriteLine("   (dry-run) Skipping actual send.");
                }
                else
                {
                    // Strip F0/F7 and send
                    byte[] dataWithout = sysex.Skip(1).Take(sysex.Length - 2).ToArray();
                    try
                    {
                        output.SendEvent(new NormalSysExEvent(dataWithout));
                        Console.WriteLine($"   📤 Sent preset to slot {slotNumber} (waiting {delayMs}ms)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ❌ Error sending SysEx: {ex.Message}");
                    }

                    // Wait a bit to allow hardware processing
                    await Task.Delay(delayMs);
                }
            }
        }

        Console.WriteLine("\n=== SendRenamedBank Complete ===");

        if (verify)
        {
            Console.WriteLine("\n🔁 Starting verification: requesting each slot and reading name back...");

            // Select input device for verification (allow index override)
            var inputDevices = InputDevice.GetAll().ToList();
            InputDevice? inDevice = null;
            if (midiInIndex.HasValue)
            {
                if (midiInIndex.Value < 0 || midiInIndex.Value >= inputDevices.Count)
                {
                    Console.WriteLine($"\n❌ MIDI IN index {midiInIndex.Value} out of range - skipping verification.");
                    return;
                }
                inDevice = inputDevices[midiInIndex.Value];
            }
            else if (!string.IsNullOrEmpty(midiInName))
            {
                inDevice = inputDevices.FirstOrDefault(d => d.Name.Equals(midiInName, StringComparison.OrdinalIgnoreCase))
                    ?? inputDevices.FirstOrDefault(d => d.Name.IndexOf(midiInName, StringComparison.OrdinalIgnoreCase) >= 0);
                if (inDevice == null)
                {
                    Console.WriteLine($"\n❌ MIDI IN device matching '{midiInName}' not found. Skipping verification.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("\n❌ MIDI IN not specified. Skipping verification.");
                return;
            }

            if (inDevice == null)
            {
                Console.WriteLine("\n❌ No MIDI input device available for verification. Skipping verification.");
                return;
            }

            Console.WriteLine($"Using MIDI IN for verification: {inDevice.Name}");

            // Find an output device to send requests from
            var outputDevices2 = OutputDevice.GetAll().ToList();
            var outDevice = outputDevices2.FirstOrDefault(d => d.Name.Equals(chosen.Name, StringComparison.OrdinalIgnoreCase))
                ?? outputDevices2.FirstOrDefault(d => d.Name.Contains("USB MIDI", StringComparison.OrdinalIgnoreCase))
                ?? outputDevices2.FirstOrDefault();

            if (outDevice == null)
            {
                Console.WriteLine("\n❌ No MIDI output device available to send verification requests. Skipping verification.");
                return;
            }

            using (var input = inDevice)
            using (var output = outDevice)
            {
                input.StartEventsListening();

                int mismatches = 0;
                for (int slot = 1; slot <= Math.Min(files.Length, 60); slot++)
                {
                    var expectedName = $"Preset {slot}";
                    var presetNumber = (byte)(slot + 30); // user preset mapping
                    var request = Nova.Domain.Midi.SysExBuilder.BuildPresetRequest(presetNumber);

                    var tcs = new TaskCompletionSource<byte[]>();
                    void Handler(object? s, Melanchall.DryWetMidi.Multimedia.MidiEventReceivedEventArgs e)
                    {
                        if (e.Event is NormalSysExEvent r)
                        {
                            var resp = new byte[] { 0xF0 }.Concat(r.Data).Concat(new byte[] { 0xF7 }).ToArray();
                            // quick check for preset response
                            if (resp.Length == 521)
                            {
                                tcs.TrySetResult(resp);
                            }
                        }
                    }

                    input.EventReceived += Handler;

                    // send request
                    byte[] requestNoDel = request.Skip(1).Take(request.Length - 2).ToArray();
                    output.SendEvent(new NormalSysExEvent(requestNoDel));

                    var completed = await Task.WhenAny(tcs.Task, Task.Delay(2000));
                    input.EventReceived -= Handler;

                    if (completed == tcs.Task)
                    {
                        var resp = tcs.Task.Result;
                        var parsed = Nova.Domain.Models.Preset.FromSysEx(resp);
                        if (parsed.IsSuccess)
                        {
                            var actualName = parsed.Value.Name;
                            Console.WriteLine($"Slot {slot}: got name '{actualName}' (expected '{expectedName}')");
                            if (!string.Equals(actualName, expectedName, StringComparison.Ordinal))
                            {
                                Console.WriteLine($"   ❌ Name mismatch on slot {slot}");
                                mismatches++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Slot {slot}: failed to parse response");
                            mismatches++;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Slot {slot}: no response (timeout)");
                        mismatches++;
                    }

                    await Task.Delay(200); // small delay between requests
                }

                Console.WriteLine($"\nVerification complete. Mismatches: {mismatches}");
            }
        }
    }
}
