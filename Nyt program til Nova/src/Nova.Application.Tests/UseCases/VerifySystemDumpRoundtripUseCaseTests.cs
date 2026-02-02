using FluentAssertions;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class VerifySystemDumpRoundtripUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WhenDataMatches_ReturnsSuccess()
    {
        // Arrange
        var originalSysEx = CreateValidSystemDumpSysEx();
        var originalDump = SystemDump.FromSysEx(originalSysEx).Value;
        
        var mockSave = new Mock<ISaveSystemDumpUseCase>();
        var mockRequest = new Mock<IRequestSystemDumpUseCase>();
        
        mockSave.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
                .ReturnsAsync(FluentResults.Result.Ok());
        
        mockRequest.Setup(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(FluentResults.Result.Ok(originalDump));
        
        var useCase = new VerifySystemDumpRoundtripUseCase(mockSave.Object, mockRequest.Object);

        // Act
        var result = await useCase.ExecuteAsync(originalDump, waitMilliseconds: 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        mockSave.Verify(x => x.ExecuteAsync(originalDump), Times.Once);
        mockRequest.Verify(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSaveFails_ReturnsFailure()
    {
        // Arrange
        var originalSysEx = CreateValidSystemDumpSysEx();
        var originalDump = SystemDump.FromSysEx(originalSysEx).Value;
        
        var mockSave = new Mock<ISaveSystemDumpUseCase>();
        var mockRequest = new Mock<IRequestSystemDumpUseCase>();
        
        mockSave.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
                .ReturnsAsync(FluentResults.Result.Fail("Save failed"));
        
        var useCase = new VerifySystemDumpRoundtripUseCase(mockSave.Object, mockRequest.Object);

        // Act
        var result = await useCase.ExecuteAsync(originalDump, waitMilliseconds: 10);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Save failed");
        mockRequest.Verify(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRequestFails_ReturnsFailure()
    {
        // Arrange
        var originalSysEx = CreateValidSystemDumpSysEx();
        var originalDump = SystemDump.FromSysEx(originalSysEx).Value;
        
        var mockSave = new Mock<ISaveSystemDumpUseCase>();
        var mockRequest = new Mock<IRequestSystemDumpUseCase>();
        
        mockSave.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
                .ReturnsAsync(FluentResults.Result.Ok());
        
        mockRequest.Setup(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(FluentResults.Result.Fail<SystemDump>("Request failed"));
        
        var useCase = new VerifySystemDumpRoundtripUseCase(mockSave.Object, mockRequest.Object);

        // Act
        var result = await useCase.ExecuteAsync(originalDump, waitMilliseconds: 10);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Request failed");
    }

    [Fact]
    public async Task ExecuteAsync_WhenDataDiffers_ReturnsFailure()
    {
        // Arrange
        var originalSysEx = CreateValidSystemDumpSysEx();
        var originalDump = SystemDump.FromSysEx(originalSysEx).Value;
        
        var differentSysEx = CreateValidSystemDumpSysEx();
        differentSysEx[10] = 0x42; // Change a byte
        var differentDump = SystemDump.FromSysEx(differentSysEx).Value;
        
        var mockSave = new Mock<ISaveSystemDumpUseCase>();
        var mockRequest = new Mock<IRequestSystemDumpUseCase>();
        
        mockSave.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
                .ReturnsAsync(FluentResults.Result.Ok());
        
        mockRequest.Setup(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(FluentResults.Result.Ok(differentDump));
        
        var useCase = new VerifySystemDumpRoundtripUseCase(mockSave.Object, mockRequest.Object);

        // Act
        var result = await useCase.ExecuteAsync(originalDump, waitMilliseconds: 10);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("mismatch");
    }

    private byte[] CreateValidSystemDumpSysEx()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x02;
        sysex[526] = 0xF7;
        return sysex;
    }
}
