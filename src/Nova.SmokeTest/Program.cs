using Nova.Domain.Models;
using Nova.Application.UseCases;

// ===========================================
// SMOKE TEST: Exercises real code with real data
// No mocks, no unit test framework — just the actual code paths.
// ===========================================

Console.WriteLine("=== Nova Smoke Test ===\n");

// ---- Test 1: Load a single real hardware dump preset ----
Console.WriteLine("--- Test 1: Single preset from hardware dump (521 bytes) ---");
var dumpDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Nova.HardwareTest", "Dumps");
dumpDir = Path.GetFullPath(dumpDir);
var firstFile = Directory.GetFiles(dumpDir, "nova-dump-*-msg001.syx").FirstOrDefault();
if (firstFile != null)
{
    var raw = File.ReadAllBytes(firstFile);
    Console.WriteLine($"  File: {Path.GetFileName(firstFile)}");
    Console.WriteLine($"  Size: {raw.Length} bytes");
    Console.WriteLine($"  Last 3 bytes: {raw[^3]:X2} {raw[^2]:X2} {raw[^1]:X2}");

    var result = Preset.FromSysEx(raw);
    if (result.IsSuccess)
        Console.WriteLine($"  PASS: Parsed preset #{result.Value.Number} '{result.Value.Name}'");
    else
        Console.WriteLine($"  FAIL: {string.Join("; ", result.Errors.Select(e => e.Message))}");

    // Try trimming to 520
    if (raw.Length == 521 && raw[519] == 0xF7 && raw[520] == 0xF7)
    {
        var trimmed = raw[..520];
        var result2 = Preset.FromSysEx(trimmed);
        if (result2.IsSuccess)
            Console.WriteLine($"  PASS (trimmed to 520): Parsed preset #{result2.Value.Number} '{result2.Value.Name}'");
        else
            Console.WriteLine($"  FAIL (trimmed to 520): {string.Join("; ", result2.Errors.Select(e => e.Message))}");
    }
}
else
{
    Console.WriteLine("  SKIP: No hardware dump files found");
}

// ---- Test 2: Load ALL 60 hardware dump presets ----
Console.WriteLine("\n--- Test 2: All 60 hardware dump presets ---");
var allFiles = Directory.GetFiles(dumpDir, "nova-dump-*-msg*.syx")
    .Where(f => !f.Contains("182108")) // Exclude System Dump
    .OrderBy(f => f)
    .ToList();
Console.WriteLine($"  Found {allFiles.Count} preset files");

int successCount = 0;
int failCount = 0;
var failMessages = new List<string>();
foreach (var file in allFiles)
{
    var raw = File.ReadAllBytes(file);
    // Trim legacy double-F7
    if (raw.Length == 521 && raw[519] == 0xF7 && raw[520] == 0xF7)
        raw = raw[..520];

    var result = Preset.FromSysEx(raw);
    if (result.IsSuccess)
    {
        successCount++;
    }
    else
    {
        failCount++;
        var msg = $"    {Path.GetFileName(file)}: {string.Join("; ", result.Errors.Select(e => e.Message))}";
        failMessages.Add(msg);
        Console.WriteLine(msg);
    }
}
Console.WriteLine($"  Result: {successCount} passed, {failCount} failed out of {allFiles.Count}");
// Show all failures

// ---- Test 3: ToSysEx round-trip ----
Console.WriteLine("\n--- Test 3: FromSysEx -> ToSysEx round-trip ---");
if (firstFile != null)
{
    var raw = File.ReadAllBytes(firstFile);
    if (raw.Length == 521 && raw[519] == 0xF7 && raw[520] == 0xF7)
        raw = raw[..520];

    var parseResult = Preset.FromSysEx(raw);
    if (parseResult.IsSuccess)
    {
        var toResult = parseResult.Value.ToSysEx();
        if (toResult.IsSuccess)
        {
            Console.WriteLine($"  ToSysEx produced {toResult.Value.Length} bytes");
            Console.WriteLine($"  First byte: {toResult.Value[0]:X2}, Last byte: {toResult.Value[^1]:X2}");

            // Verify it can be parsed back
            var roundTrip = Preset.FromSysEx(toResult.Value);
            if (roundTrip.IsSuccess)
                Console.WriteLine($"  PASS: Round-trip preserved preset #{roundTrip.Value.Number} '{roundTrip.Value.Name}'");
            else
                Console.WriteLine($"  FAIL round-trip: {string.Join("; ", roundTrip.Errors.Select(e => e.Message))}");
        }
        else
        {
            Console.WriteLine($"  FAIL ToSysEx: {string.Join("; ", toResult.Errors.Select(e => e.Message))}");
        }
    }
    else
    {
        Console.WriteLine($"  SKIP: Could not parse first preset");
    }
}

// ---- Test 4: Simulate Load Bank from concatenated file ----
Console.WriteLine("\n--- Test 4: Simulate LoadBank (concatenated .syx file) ---");
{
    // Create a temporary concatenated bank file like LoadBankUseCase expects
    var bankData520 = new List<byte>();
    var bankData521 = new List<byte>();
    int parsedForBank = 0;
    foreach (var file in allFiles.Take(60))
    {
        var raw = File.ReadAllBytes(file);
        bankData521.AddRange(raw); // Original 521-byte format

        // Trim for 520
        if (raw.Length == 521 && raw[519] == 0xF7 && raw[520] == 0xF7)
            raw = raw[..520];
        bankData520.AddRange(raw);
        parsedForBank++;
    }
    Console.WriteLine($"  Concatenated {parsedForBank} presets:");
    Console.WriteLine($"    521-byte format: {bankData521.Count} bytes (LoadBank expects {60 * 520})");
    Console.WriteLine($"    520-byte format: {bankData520.Count} bytes");

    if (bankData521.Count == 60 * 521)
        Console.WriteLine($"  BUG CONFIRMED: Old bank files are {60 * 521} bytes, LoadBankUseCase rejects them!");
    if (bankData520.Count == 60 * 520)
        Console.WriteLine($"  520-byte bank would be accepted: {bankData520.Count} bytes");
}

// ---- Test 5: SysExValidator ----
Console.WriteLine("\n--- Test 5: SysExValidator ---");
if (firstFile != null)
{
    var raw = File.ReadAllBytes(firstFile);
    if (raw.Length == 521 && raw[519] == 0xF7 && raw[520] == 0xF7)
        raw = raw[..520];

    var checksumOk = Nova.Domain.Midi.SysExValidator.ValidateChecksum(raw);
    Console.WriteLine($"  Checksum valid: {checksumOk}");
    if (!checksumOk)
    {
        // Show what the checksum bytes look like
        Console.WriteLine($"  Byte[518] (checksum position): {raw[518]:X2}");
        // Calculate what it should be
        int sum = 0;
        for (int j = 34; j <= 517; j++) sum += raw[j];
        var expected = (byte)(sum & 0x7F);
        Console.WriteLine($"  Expected checksum (sum bytes 34-517 & 0x7F): {expected:X2}");
    }
}

Console.WriteLine("\n=== Smoke Test Complete ===");
