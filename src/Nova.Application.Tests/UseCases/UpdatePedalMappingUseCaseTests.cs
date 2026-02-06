using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class UpdatePedalMappingUseCaseTests
{
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
        validSystemDump[526] = 0xF7;  // SysEx end
        
        return SystemDump.FromSysEx(validSystemDump).Value;
    }

    [Fact]
    public async Task UpdateAsync_ValidValues_UpdatesAndSaves()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var mockSaveUseCase = new Mock<ISaveSystemDumpUseCase>();
        mockSaveUseCase.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
            .ReturnsAsync(Result.Ok());

        var useCase = new UpdatePedalMappingUseCase(systemDump, mockSaveUseCase.Object);

        // Act
        var result = await useCase.UpdateAsync(parameter: 20, min: 10, mid: 60, max: 90);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(20, systemDump.GetPedalParameter());
        Assert.Equal(10, systemDump.GetPedalMin());
        Assert.Equal(60, systemDump.GetPedalMid());
        Assert.Equal(90, systemDump.GetPedalMax());
        mockSaveUseCase.Verify(x => x.ExecuteAsync(systemDump), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_InvalidParameter_Fails()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var mockSaveUseCase = new Mock<ISaveSystemDumpUseCase>();

        var useCase = new UpdatePedalMappingUseCase(systemDump, mockSaveUseCase.Object);

        // Act
        var result = await useCase.UpdateAsync(parameter: 200, min: 50, mid: 50, max: 50);

        // Assert
        Assert.True(result.IsFailed);
        mockSaveUseCase.Verify(x => x.ExecuteAsync(It.IsAny<SystemDump>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_InvalidMin_Fails()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var mockSaveUseCase = new Mock<ISaveSystemDumpUseCase>();

        var useCase = new UpdatePedalMappingUseCase(systemDump, mockSaveUseCase.Object);

        // Act
        var result = await useCase.UpdateAsync(parameter: 20, min: 150, mid: 50, max: 50);

        // Assert
        Assert.True(result.IsFailed);
        mockSaveUseCase.Verify(x => x.ExecuteAsync(It.IsAny<SystemDump>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_SaveFails_ReturnsSaveError()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var mockSaveUseCase = new Mock<ISaveSystemDumpUseCase>();
        mockSaveUseCase.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
            .ReturnsAsync(Result.Fail("MIDI port not connected"));

        var useCase = new UpdatePedalMappingUseCase(systemDump, mockSaveUseCase.Object);

        // Act
        var result = await useCase.UpdateAsync(parameter: 20, min: 10, mid: 60, max: 90);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("MIDI port not connected", result.Errors[0].Message);
    }
}
