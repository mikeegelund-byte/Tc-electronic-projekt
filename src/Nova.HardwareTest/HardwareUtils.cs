using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System.Text.RegularExpressions;

namespace Nova.HardwareTest;

public static class HardwareUtils
{
    private static readonly string[] BlacklistPatterns = new[]
    {
        "Microsoft GS Wavetable",
        "Microsoft GS",
        "Wavetable",
        "Realtek", // OS synth or audio devices
    };

    // Normalizes device names by removing trailing index tokens like " [0]", " [1]" or " (0)".
    public static string NormalizeDeviceName(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;
        // Remove bracketed indices or trailing numbers: " [0]", " [1]", " (0)" etc.
        var result = Regex.Replace(name, "\\s*\\[\\d+\\]$", "", RegexOptions.Compiled);
        result = Regex.Replace(result, "\\s*\\(\\d+\\)$", "", RegexOptions.Compiled);
        return result.Trim();
    }

    public static bool IsBlacklisted(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        foreach (var p in BlacklistPatterns)
        {
            if (name.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0) return true;
        }
        return false;
    }

    // Try to find an input device that matches the output device's normalized base name.
    public static InputDevice? FindMatchingInputForOutput(OutputDevice output)
    {
        var inputs = InputDevice.GetAll().ToList();
        var normalized = NormalizeDeviceName(output.Name);
        // Prefer exact normalized match
        var exact = inputs.FirstOrDefault(i => NormalizeDeviceName(i.Name).Equals(normalized, StringComparison.OrdinalIgnoreCase));
        if (exact != null && !IsBlacklisted(exact.Name)) return exact;
        // Otherwise, find any input that contains the normalized string
        var contains = inputs.FirstOrDefault(i => NormalizeDeviceName(i.Name).IndexOf(normalized, StringComparison.OrdinalIgnoreCase) >= 0);
        if (contains != null && !IsBlacklisted(contains.Name)) return contains;
        // Finally, return first non-blacklisted input
        return inputs.FirstOrDefault(i => !IsBlacklisted(i.Name));
    }

    // Try to find an output device that matches the input device's normalized base name.
    public static OutputDevice? FindMatchingOutputForInput(InputDevice input)
    {
        var outputs = OutputDevice.GetAll().ToList();
        var normalized = NormalizeDeviceName(input.Name);
        var exact = outputs.FirstOrDefault(o => NormalizeDeviceName(o.Name).Equals(normalized, StringComparison.OrdinalIgnoreCase));
        if (exact != null && !IsBlacklisted(exact.Name)) return exact;
        var contains = outputs.FirstOrDefault(o => NormalizeDeviceName(o.Name).IndexOf(normalized, StringComparison.OrdinalIgnoreCase) >= 0);
        if (contains != null && !IsBlacklisted(contains.Name)) return contains;
        return outputs.FirstOrDefault(o => !IsBlacklisted(o.Name));
    }

    // Resolve pair given optional requested names. Returns (input, output)
    public static (InputDevice? input, OutputDevice? output) ResolvePair(string? midiInName, string? midiOutName)
    {
        InputDevice? input = null;
        OutputDevice? output = null;

        var inputs = InputDevice.GetAll().ToList();
        var outputs = OutputDevice.GetAll().ToList();

        if (!string.IsNullOrEmpty(midiInName))
        {
            input = inputs.FirstOrDefault(i => i.Name.Equals(midiInName, StringComparison.OrdinalIgnoreCase))
                ?? inputs.FirstOrDefault(i => i.Name.IndexOf(midiInName, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        if (!string.IsNullOrEmpty(midiOutName))
        {
            output = outputs.FirstOrDefault(o => o.Name.Equals(midiOutName, StringComparison.OrdinalIgnoreCase))
                ?? outputs.FirstOrDefault(o => o.Name.IndexOf(midiOutName, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // If both unspecified, attempt smart pairing based on normalized names
        if (input == null && output == null)
        {
            // Prefer explicit MIDI 0/1 naming if present
            var midi0 = outputs.FirstOrDefault(o => o.Name.IndexOf("MIDI 0", StringComparison.OrdinalIgnoreCase) >= 0);
            var midi1 = inputs.FirstOrDefault(i => i.Name.IndexOf("MIDI 1", StringComparison.OrdinalIgnoreCase) >= 0);
            if (midi0 != null && midi1 != null)
            {
                output = midi0;
                input = midi1;
            }

            if (input == null && output == null)
            {
                // Try to find a usb prefixed name present in both lists
                var usbCandidates = outputs.Select(o => NormalizeDeviceName(o.Name))
                    .Intersect(inputs.Select(i => NormalizeDeviceName(i.Name)))
                    .ToList();
                // Prefer candidates containing "USB"
                var usbPreferred = usbCandidates.FirstOrDefault(n => n.IndexOf("USB", StringComparison.OrdinalIgnoreCase) >= 0);
                string? chosenName = usbPreferred ?? usbCandidates.FirstOrDefault();

                if (!string.IsNullOrEmpty(chosenName))
                {
                    output = outputs.FirstOrDefault(o => NormalizeDeviceName(o.Name).Equals(chosenName, StringComparison.OrdinalIgnoreCase));
                    input = inputs.FirstOrDefault(i => NormalizeDeviceName(i.Name).Equals(chosenName, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        // Fallbacks: pick non-blacklisted first devices
        if (output == null) output = outputs.FirstOrDefault(o => !IsBlacklisted(o.Name)) ?? outputs.FirstOrDefault();
        if (input == null) input = inputs.FirstOrDefault(i => !IsBlacklisted(i.Name)) ?? inputs.FirstOrDefault();

        return (input, output);
    }
}
