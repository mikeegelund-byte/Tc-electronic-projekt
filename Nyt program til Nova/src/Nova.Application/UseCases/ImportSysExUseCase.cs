using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public class ImportSysExUseCase : IImportSysExUseCase
{
    private readonly IDetectSysExTypeUseCase _detectUseCase;

    public ImportSysExUseCase()
    {
        _detectUseCase = new DetectSysExTypeUseCase();
    }

    public async Task<Result<object>> ExecuteAsync(string filePath)
    {
        var detectResult = await _detectUseCase.ExecuteAsync(filePath);
        if (detectResult.IsFailed)
            return Result.Fail<object>(detectResult.Errors);

        var data = await File.ReadAllBytesAsync(filePath);

        switch (detectResult.Value)
        {
            case SysExType.Preset:
                var presetResult = Preset.FromSysEx(data);
                return presetResult.IsSuccess 
                    ? Result.Ok<object>(presetResult.Value) 
                    : Result.Fail<object>(presetResult.Errors);

            case SysExType.SystemDump:
                var systemResult = SystemDump.FromSysEx(data);
                return systemResult.IsSuccess
                    ? Result.Ok<object>(systemResult.Value)
                    : Result.Fail<object>(systemResult.Errors);

            case SysExType.UserBank:
                var bank = UserBankDump.Empty();
                int presetCount = data.Length / 521;
                for (int i = 0; i < presetCount && i < 60; i++)
                {
                    var presetData = new byte[521];
                    Array.Copy(data, i * 521, presetData, 0, 521);
                    var bankPresetResult = Preset.FromSysEx(presetData);
                    if (bankPresetResult.IsSuccess)
                    {
                        var bankUpdateResult = bank.WithPreset(bankPresetResult.Value.Number, bankPresetResult.Value);
                        if (bankUpdateResult.IsSuccess)
                        {
                            bank = bankUpdateResult.Value;
                        }
                    }
                }
                return Result.Ok<object>(bank);

            default:
                return Result.Fail<object>("Unknown or invalid SysEx file type");
        }
    }
}
