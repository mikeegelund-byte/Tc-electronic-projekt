using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class UpdateCCMappingUseCaseTests
{
    private readonly UpdateCCMappingUseCase _useCase;

    public UpdateCCMappingUseCaseTests()
    {
        _useCase = new UpdateCCMappingUseCase();
    }

    private SystemDump CreateValidSystemDump()
    {
        var validSystemDump = new byte[526];
        validSystemDump[0] = 0xF0;  // SysEx start
        validSystemDump[1] = 0x00;  // TC Electronic
        validSystemDump[2] = 0x20;
        validSystemDump[3] = 0x1F;
        validSystemDump[4] = 0x04;  // Device ID
        validSystemDump[5] = 0x63;  // Nova System model ID
        validSystemDump[6] = 0x20;  // Message ID (Dump)
        validSystemDump[7] = 0x02;  // Data Type (System Dump)
        validSystemDump[525] = 0xF7;  // SysEx end

        return SystemDump.FromSysEx(validSystemDump).Value;
    }

    [Fact]
    public async Task ExecuteAsync_WhenSystemDumpIsNull_ReturnsFailure()
    {
        // Act
        var result = await _useCase.ExecuteAsync(null!, 0, 1);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("System Dump cannot be null");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCCIndexIsOutOfRange_ReturnsFailure()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act - Test negative index
        var result1 = await _useCase.ExecuteAsync(systemDump, -1, 1);

        // Act - Test index too high
        var result2 = await _useCase.ExecuteAsync(systemDump, 11, 1);

        // Assert
        result1.IsFailed.Should().BeTrue();
        result1.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("CC index must be between 0 and 10");

        result2.IsFailed.Should().BeTrue();
        result2.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("CC index must be between 0 and 10");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCCNumberIsOutOfRange_ReturnsFailure()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act - CC number 128 is out of MIDI spec range
        var result = await _useCase.ExecuteAsync(systemDump, 0, 128);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("CC number must be between 0 and 127");
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidInput_UpdatesSystemDumpBytes()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        int ccIndex = 1; // Drive
        int ccNumber = 20;     // CC 20

        // Act
        var result = await _useCase.ExecuteAsync(systemDump, ccIndex, ccNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var mappingResult = systemDump.GetCCMapping(ccIndex);
        mappingResult.IsSuccess.Should().BeTrue();

        var updatedMapping = mappingResult.Value;
        updatedMapping.CCNumber.Should().Be(ccNumber);
        updatedMapping.IsAssigned.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSettingUnassignedMapping_SetsOff()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        int ccIndex = 2; // Compressor

        // Act - Set to off (null means Off)
        var result = await _useCase.ExecuteAsync(systemDump, ccIndex, null);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var mappingResult = systemDump.GetCCMapping(ccIndex);
        mappingResult.IsSuccess.Should().BeTrue();

        var updatedMapping = mappingResult.Value;
        updatedMapping.CCNumber.Should().BeNull();
        updatedMapping.IsAssigned.Should().BeFalse();
    }
}
