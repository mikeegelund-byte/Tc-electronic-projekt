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
        var validSystemDump = new byte[527];
        validSystemDump[0] = 0xF0;  // SysEx start
        validSystemDump[1] = 0x00;  // TC Electronic
        validSystemDump[2] = 0x20;
        validSystemDump[3] = 0x1F;
        validSystemDump[4] = 0x04;  // Bank number
        validSystemDump[5] = 0x63;  // Nova System model ID
        validSystemDump[6] = 0x20;  // Message ID (Dump)
        validSystemDump[7] = 0x02;  // Data Type (System Dump)
        
        // Initialize CC mappings (bytes 34-161)
        for (int i = 0; i < 64; i++)
        {
            validSystemDump[34 + i * 2] = (byte)i;        // CC number
            validSystemDump[34 + i * 2 + 1] = 0x00;        // Parameter ID
        }
        
        validSystemDump[526] = 0xF7;  // SysEx end
        
        return SystemDump.FromSysEx(validSystemDump).Value;
    }

    [Fact]
    public async Task ExecuteAsync_WhenSystemDumpIsNull_ReturnsFailure()
    {
        // Act
        var result = await _useCase.ExecuteAsync(null!, 0, 0x01, 0x10);

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
        var result1 = await _useCase.ExecuteAsync(systemDump, -1, 0x01, 0x10);
        
        // Act - Test index too high
        var result2 = await _useCase.ExecuteAsync(systemDump, 64, 0x01, 0x10);

        // Assert
        result1.IsFailed.Should().BeTrue();
        result1.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("CC index must be between 0 and 63");
        
        result2.IsFailed.Should().BeTrue();
        result2.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("CC index must be between 0 and 63");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCCNumberIsOutOfRange_ReturnsFailure()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act - CC number 128 is out of MIDI spec range
        var result = await _useCase.ExecuteAsync(systemDump, 0, 128, 0x10);

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
        int ccIndex = 5;
        byte ccNumber = 0x14;     // CC 20
        byte parameterId = 0x2A;  // Parameter 42

        // Act
        var result = await _useCase.ExecuteAsync(systemDump, ccIndex, ccNumber, parameterId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify bytes were updated correctly
        var mappingResult = systemDump.GetCCMapping(ccIndex);
        mappingResult.IsSuccess.Should().BeTrue();
        
        var updatedMapping = mappingResult.Value;
        updatedMapping.CCNumber.Should().Be(ccNumber);
        updatedMapping.ParameterId.Should().Be(parameterId);
        updatedMapping.IsAssigned.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSettingUnassignedMapping_SetsFF()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        int ccIndex = 10;

        // Act - Set to unassigned (0xFF means unassigned)
        var result = await _useCase.ExecuteAsync(systemDump, ccIndex, 0xFF, 0xFF);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var mappingResult = systemDump.GetCCMapping(ccIndex);
        mappingResult.IsSuccess.Should().BeTrue();
        
        var updatedMapping = mappingResult.Value;
        updatedMapping.CCNumber.Should().Be(0xFF);
        updatedMapping.ParameterId.Should().Be(0xFF);
        updatedMapping.IsAssigned.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesCorrectByteOffset()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        int ccIndex = 0;
        byte ccNumber = 0x7F;     // CC 127
        byte parameterId = 0x64;  // Parameter 100

        // Act
        var result = await _useCase.ExecuteAsync(systemDump, ccIndex, ccNumber, parameterId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // CC_MAPPING_START_OFFSET is 34
        // Byte offset = 34 + (ccIndex * 2) = 34 + 0 = 34
        var mappingResult = systemDump.GetCCMapping(ccIndex);
        mappingResult.IsSuccess.Should().BeTrue();
        
        var mapping = mappingResult.Value;
        mapping.CCNumber.Should().Be(ccNumber);
        mapping.ParameterId.Should().Be(parameterId);
    }
}
