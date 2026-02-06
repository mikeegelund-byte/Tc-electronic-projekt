using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class GetCCMappingsUseCaseTests
{
    private readonly GetCCMappingsUseCase _useCase;

    public GetCCMappingsUseCaseTests()
    {
        _useCase = new GetCCMappingsUseCase();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSystemDumpIsNull_ReturnsFailure()
    {
        // Act
        var result = await _useCase.ExecuteAsync(null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("System Dump cannot be null");
    }

    [Fact]
    public async Task ExecuteAsync_WhenSystemDumpIsValid_Returns64Mappings()
    {
        // Arrange
        byte[] validSystemDump = new byte[527];
        validSystemDump[0] = 0xF0;  // SysEx start
        validSystemDump[1] = 0x00;  // TC Electronic
        validSystemDump[2] = 0x20;
        validSystemDump[3] = 0x1F;
        validSystemDump[4] = 0x04;  // Bank number
        validSystemDump[5] = 0x63;  // Nova System model ID
        validSystemDump[6] = 0x20;  // Message ID (Dump)
        validSystemDump[7] = 0x02;  // Data Type (System Dump)
        validSystemDump[526] = 0xF7; // SysEx end

        // Fill CC mapping data (offset 34, 64 mappings × 2 bytes)
        for (int i = 0; i < 64; i++)
        {
            validSystemDump[34 + (i * 2)] = (byte)(i + 1);     // CC number
            validSystemDump[34 + (i * 2) + 1] = (byte)(i + 10); // Parameter ID
        }

        var systemDump = SystemDump.FromSysEx(validSystemDump).Value;

        // Act
        var result = await _useCase.ExecuteAsync(systemDump);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(64);
        result.Value[0].CCNumber.Should().Be(1);
        result.Value[0].ParameterId.Should().Be(10);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSystemDumpIsValid_AllMappingsAreAssigned()
    {
        // Arrange
        byte[] validSystemDump = new byte[527];
        validSystemDump[0] = 0xF0;
        validSystemDump[1] = 0x00;
        validSystemDump[2] = 0x20;
        validSystemDump[3] = 0x1F;
        validSystemDump[4] = 0x04;
        validSystemDump[5] = 0x63;
        validSystemDump[6] = 0x20;
        validSystemDump[7] = 0x02;
        validSystemDump[526] = 0xF7;

        // Fill all CC mappings with assigned values (not 0xFF)
        for (int i = 0; i < 64; i++)
        {
            validSystemDump[34 + (i * 2)] = (byte)(i + 1);
            validSystemDump[34 + (i * 2) + 1] = (byte)(i + 10);
        }

        var systemDump = SystemDump.FromSysEx(validSystemDump).Value;

        // Act
        var result = await _useCase.ExecuteAsync(systemDump);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().AllSatisfy(mapping => mapping.IsAssigned.Should().BeTrue());
    }

    [Fact]
    public async Task ExecuteAsync_WithUnassignedMappings_ReturnsCorrectIsAssignedStatus()
    {
        // Arrange
        byte[] validSystemDump = new byte[527];
        validSystemDump[0] = 0xF0;
        validSystemDump[1] = 0x00;
        validSystemDump[2] = 0x20;
        validSystemDump[3] = 0x1F;
        validSystemDump[4] = 0x04;
        validSystemDump[5] = 0x63;
        validSystemDump[6] = 0x20;
        validSystemDump[7] = 0x02;
        validSystemDump[526] = 0xF7;

        // First 5 mappings assigned, rest unassigned (0xFF)
        for (int i = 0; i < 5; i++)
        {
            validSystemDump[34 + (i * 2)] = (byte)(i + 1);
            validSystemDump[34 + (i * 2) + 1] = (byte)(i + 10);
        }
        for (int i = 5; i < 64; i++)
        {
            validSystemDump[34 + (i * 2)] = 0xFF;
            validSystemDump[34 + (i * 2) + 1] = 0xFF;
        }

        var systemDump = SystemDump.FromSysEx(validSystemDump).Value;

        // Act
        var result = await _useCase.ExecuteAsync(systemDump);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Take(5).Should().AllSatisfy(mapping => mapping.IsAssigned.Should().BeTrue());
        result.Value.Skip(5).Should().AllSatisfy(mapping => mapping.IsAssigned.Should().BeFalse());
    }
}
