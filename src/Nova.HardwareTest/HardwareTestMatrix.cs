using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Nova.Domain.Midi;
using System.Diagnostics.CodeAnalysis;

namespace Nova.HardwareTest;

public static class HardwareTestMatrix
{
    public static bool TryResolveDevices(
        string? midiInName,
        string? midiOutName,
        int? midiInIndex,
        int? midiOutIndex,
        [NotNullWhen(true)] out InputDevice? input,
        [NotNullWhen(true)] out OutputDevice? output)
    {
        input = null;
        output = null;

        var inputs = InputDevice.GetAll().ToList();
        var outputs = OutputDevice.GetAll().ToList();

        if (inputs.Count == 0)
        {
            Console.WriteLine("❌ No MIDI input devices found.");
            return false;
        }

        if (outputs.Count == 0)
        {
            Console.WriteLine("❌ No MIDI output devices found.");
            return false;
        }

        if (midiInIndex.HasValue)
        {
            if (midiInIndex.Value < 0 || midiInIndex.Value >= inputs.Count)
            {
                Console.WriteLine($"❌ MIDI IN index {midiInIndex.Value} out of range.");
                return false;
            }

            input = inputs[midiInIndex.Value];
        }
        else if (!string.IsNullOrWhiteSpace(midiInName))
        {
            input = inputs.FirstOrDefault(d => d.Name.Equals(midiInName, StringComparison.OrdinalIgnoreCase))
                ?? inputs.FirstOrDefault(d => d.Name.IndexOf(midiInName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (input == null)
            {
                Console.WriteLine($"❌ MIDI IN device matching '{midiInName}' not found.");
                return false;
            }
        }

        if (midiOutIndex.HasValue)
        {
            if (midiOutIndex.Value < 0 || midiOutIndex.Value >= outputs.Count)
            {
                Console.WriteLine($"❌ MIDI OUT index {midiOutIndex.Value} out of range.");
                return false;
            }

            output = outputs[midiOutIndex.Value];
        }
        else if (!string.IsNullOrWhiteSpace(midiOutName))
        {
            output = outputs.FirstOrDefault(d => d.Name.Equals(midiOutName, StringComparison.OrdinalIgnoreCase))
                ?? outputs.FirstOrDefault(d => d.Name.IndexOf(midiOutName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (output == null)
            {
                Console.WriteLine($"❌ MIDI OUT device matching '{midiOutName}' not found.");
                return false;
            }
        }

        if (input == null || output == null)
        {
            Console.WriteLine("❌ You must provide both --midi-in and --midi-out (or indexes). Use --list-devices to inspect names.");
            return false;
        }

        return true;
    }

    public static Task PairTest(InputDevice input, OutputDevice output)
    {
        Console.WriteLine($"✅ Pair test opening IN='{input.Name}' / OUT='{output.Name}'");
        try
        {
            input.StartEventsListening();
            input.StopEventsListening();
            Console.WriteLine("✅ Pair test complete (ports opened successfully)");
        }
        catch (MidiDeviceException ex)
        {
            Console.WriteLine($"❌ Failed to open MIDI IN: {ex.Message}");
            Console.WriteLine("   Close other MIDI apps, then retry.");
        }
        return Task.CompletedTask;
    }

    public static async Task RequestSystemDump(InputDevice input, OutputDevice output, int timeoutMs)
    {
        Console.WriteLine("📥 Requesting System Dump...");
        var request = SysExBuilder.BuildSystemDumpRequest();
        var response = await SendRequestAndWaitForSysEx(input, output, request, timeoutMs);
        if (response == null)
        {
            Console.WriteLine("❌ Timeout: no System Dump received.");
            return;
        }

        Console.WriteLine($"✅ System Dump received ({response.Length} bytes)");
    }

    public static async Task RequestBankDump(InputDevice input, OutputDevice output, int timeoutMs)
    {
        Console.WriteLine("📥 Requesting User Bank Dump (60 presets)...");
        var request = SysExBuilder.BuildBankDumpRequest();
        var messages = await SendRequestAndCollectSysEx(input, output, request, timeoutMs, expectedCount: 60);
        if (messages.Count == 0)
        {
            Console.WriteLine("❌ Timeout: no bank dump messages received.");
            return;
        }

        Console.WriteLine($"✅ Bank dump received: {messages.Count} SysEx message(s)");
        if (messages.Count < 60)
        {
            Console.WriteLine("⚠️ Expected 60 presets. Check MIDI IN/OUT pairing and pedal state.");
        }
    }

    public static async Task RoundtripBank(InputDevice input, OutputDevice output, int timeoutMs, int delayMs)
    {
        Console.WriteLine("🔁 Roundtrip Bank: request → send → verify");

        var request = SysExBuilder.BuildBankDumpRequest();
        var original = await SendRequestAndCollectSysEx(input, output, request, timeoutMs, expectedCount: 60);
        if (original.Count == 0)
        {
            Console.WriteLine("❌ Failed to receive initial bank dump.");
            return;
        }

        Console.WriteLine($"✅ Received {original.Count} messages. Sending back to pedal...");
        foreach (var message in original)
        {
            var dataWithout = message.Length > 2 && message[0] == 0xF0 && message[^1] == 0xF7
                ? message[1..^1]
                : message;
            output.SendEvent(new NormalSysExEvent(dataWithout));
            await Task.Delay(delayMs);
        }

        Console.WriteLine("📥 Verifying by requesting bank dump again...");
        var verify = await SendRequestAndCollectSysEx(input, output, request, timeoutMs, expectedCount: 60);
        if (verify.Count == 0)
        {
            Console.WriteLine("❌ Verification failed: no messages received.");
            return;
        }

        Console.WriteLine($"✅ Verification received {verify.Count} message(s)");
        if (verify.Count < 60)
        {
            Console.WriteLine("⚠️ Verification count below 60. Check pairing and ensure dump completed.");
        }

        if (original.Count == verify.Count)
        {
            var firstMatch = original[0].SequenceEqual(verify[0]);
            var lastMatch = original[^1].SequenceEqual(verify[^1]);
            Console.WriteLine($"Compare first/last preset: {(firstMatch && lastMatch ? "MATCH" : "MISMATCH")}");
        }
        else
        {
            Console.WriteLine("⚠️ Message counts differ between send and verify.");
        }
    }

    public static async Task CcLearn(InputDevice input, int timeoutMs)
    {
        Console.WriteLine("🎛️ CC Learn: move pedal or send CC message...");

        var tcs = new TaskCompletionSource<ControlChangeEvent>();
        void Handler(object? sender, MidiEventReceivedEventArgs e)
        {
            if (e.Event is ControlChangeEvent cc)
            {
                tcs.TrySetResult(cc);
            }
        }

        input.EventReceived += Handler;
        try
        {
            input.StartEventsListening();
        }
        catch (MidiDeviceException ex)
        {
            input.EventReceived -= Handler;
            Console.WriteLine($"❌ Failed to open MIDI IN: {ex.Message}");
            Console.WriteLine("   Close other MIDI apps or reconnect the interface, then retry.");
            return;
        }

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
        input.EventReceived -= Handler;
        try
        {
            input.StopEventsListening();
        }
        catch (MidiDeviceException)
        {
            // Ignore stop errors after a failed open.
        }

        if (completed == tcs.Task)
        {
            var cc = tcs.Task.Result;
            Console.WriteLine($"✅ CC received: number={cc.ControlNumber} value={cc.ControlValue} channel={cc.Channel}");
        }
        else
        {
            Console.WriteLine("❌ Timeout: no CC message received.");
        }
    }

    public static async Task DisconnectReconnect(string midiInName, string midiOutName, int delayMs)
    {
        Console.WriteLine("🔌 Disconnect/Reconnect test...");

        for (var i = 0; i < 2; i++)
        {
            Console.WriteLine($"Cycle {i + 1}: opening ports...");
            using var input = InputDevice.GetAll().First(d => d.Name.Equals(midiInName, StringComparison.OrdinalIgnoreCase));
            using var output = OutputDevice.GetAll().First(d => d.Name.Equals(midiOutName, StringComparison.OrdinalIgnoreCase));

            input.StartEventsListening();
            await Task.Delay(delayMs);
            input.StopEventsListening();
            await Task.Delay(delayMs);
        }

        Console.WriteLine("✅ Disconnect/Reconnect complete");
    }

    private static async Task<byte[]?> SendRequestAndWaitForSysEx(
        InputDevice input,
        OutputDevice output,
        byte[] request,
        int timeoutMs)
    {
        var tcs = new TaskCompletionSource<byte[]>();

        void Handler(object? sender, MidiEventReceivedEventArgs e)
        {
            if (e.Event is NormalSysExEvent sysEx)
            {
                var data = new byte[sysEx.Data.Length + 2];
                data[0] = 0xF0;
                Array.Copy(sysEx.Data, 0, data, 1, sysEx.Data.Length);
                data[^1] = 0xF7;
                tcs.TrySetResult(data);
            }
        }

        input.EventReceived += Handler;
        try
        {
            input.StartEventsListening();
        }
        catch (MidiDeviceException ex)
        {
            input.EventReceived -= Handler;
            Console.WriteLine($"❌ Failed to open MIDI IN: {ex.Message}");
            Console.WriteLine("   Close other MIDI apps or reconnect the interface, then retry.");
            return null;
        }

        var requestWithout = request.Length > 2 && request[0] == 0xF0 && request[^1] == 0xF7
            ? request[1..^1]
            : request;
        output.SendEvent(new NormalSysExEvent(requestWithout));

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));

        input.EventReceived -= Handler;
        try
        {
            input.StopEventsListening();
        }
        catch (MidiDeviceException)
        {
            // Ignore stop errors after a failed open.
        }

        return completed == tcs.Task ? tcs.Task.Result : null;
    }

    private static async Task<List<byte[]>> SendRequestAndCollectSysEx(
        InputDevice input,
        OutputDevice output,
        byte[] request,
        int timeoutMs,
        int expectedCount)
    {
        var messages = new List<byte[]>();
        var tcs = new TaskCompletionSource<bool>();

        void Handler(object? sender, MidiEventReceivedEventArgs e)
        {
            if (e.Event is NormalSysExEvent sysEx)
            {
                var data = new byte[sysEx.Data.Length + 2];
                data[0] = 0xF0;
                Array.Copy(sysEx.Data, 0, data, 1, sysEx.Data.Length);
                data[^1] = 0xF7;
                messages.Add(data);
                if (messages.Count >= expectedCount)
                {
                    tcs.TrySetResult(true);
                }
            }
        }

        input.EventReceived += Handler;
        try
        {
            input.StartEventsListening();
        }
        catch (MidiDeviceException ex)
        {
            input.EventReceived -= Handler;
            Console.WriteLine($"❌ Failed to open MIDI IN: {ex.Message}");
            Console.WriteLine("   Close other MIDI apps or reconnect the interface, then retry.");
            return messages;
        }

        var requestWithout = request.Length > 2 && request[0] == 0xF0 && request[^1] == 0xF7
            ? request[1..^1]
            : request;
        output.SendEvent(new NormalSysExEvent(requestWithout));

        var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));

        input.EventReceived -= Handler;
        try
        {
            input.StopEventsListening();
        }
        catch (MidiDeviceException)
        {
            // Ignore stop errors after a failed open.
        }

        return completed == tcs.Task ? messages : messages;
    }
}
