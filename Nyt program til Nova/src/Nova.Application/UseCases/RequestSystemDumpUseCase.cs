using FluentResults;
using Nova.Domain.Midi;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

public sealed class RequestSystemDumpUseCase : IRequestSystemDumpUseCase
{
    private readonly IMidiPort _midiPort;

    public RequestSystemDumpUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort ?? throw new ArgumentNullException(nameof(midiPort));
    }

    /// <summary>
    /// Requests system dump from Nova System pedal.
    /// </summary>
    /// <param name="timeoutMs">Timeout in milliseconds for receiving response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing parsed SystemDump or error details</returns>
    public async Task<Result<SystemDump>> ExecuteAsync(
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Build system dump request
            var request = SysExBuilder.BuildSystemDumpRequest();
            Log.Information("Built system dump request ({ByteCount} bytes)", request.Length);

            // 2. Send request
            var sendResult = await _midiPort.SendSysExAsync(request);
            if (sendResult.IsFailed)
            {
                Log.Error("Failed to send system dump request: {Error}", sendResult.Errors.First().Message);
                return Result.Fail<SystemDump>(sendResult.Errors.First().Message);
            }

            Log.Information("System dump request sent successfully");

            // 3. Wait for response using async enumeration with timeout
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                cts.CancelAfter(timeoutMs);

                try
                {
                    byte[]? systemDumpSysex = null;

                    await foreach (var sysex in _midiPort.ReceiveSysExAsync(cts.Token))
                    {
                        // Check if this is a system dump response (byte[6]=0x20, byte[7]=0x02)
                        if (sysex.Length >= 8 && sysex[6] == 0x20 && sysex[7] == 0x02)
                        {
                            systemDumpSysex = sysex;
                            break;
                        }
                    }

                    if (systemDumpSysex == null)
                    {
                        Log.Error("No system dump response received");
                        return Result.Fail("No system dump response received within timeout");
                    }

                    Log.Information("Received system dump response ({ByteCount} bytes)", systemDumpSysex.Length);

                    // 4. Parse response
                    var parseResult = SystemDump.FromSysEx(systemDumpSysex);

                    // 5. Log and return
                    if (parseResult.IsSuccess)
                    {
                        Log.Information("System dump parsed successfully");
                        return parseResult;
                    }
                    else
                    {
                        var errorMsg = parseResult.Errors.First().Message;
                        Log.Error("Failed to parse system dump: {Error}", errorMsg);
                        return parseResult;
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Error("System dump request timed out after {TimeoutMs}ms", timeoutMs);
                    return Result.Fail($"System dump request timed out after {timeoutMs}ms");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error during system dump request");
            return Result.Fail(ex.Message);
        }
    }
}
