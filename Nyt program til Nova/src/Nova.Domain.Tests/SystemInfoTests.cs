namespace Nova.Domain.Tests;

using Nova.Common;

public class SystemInfoTests
{
    [Fact]
    public void SystemInfo_Returns_Correct_Project_Name()
    {
        // Arrange
        var systemInfo = new SystemInfo();

        // Act
        var projectName = systemInfo.GetProjectName();

        // Assert
        Assert.Equal("Nova System MIDI Control", projectName);
    }

    [Fact]
    public void SystemInfo_Returns_Valid_Version()
    {
        // Arrange
        var systemInfo = new SystemInfo();

        // Act
        var version = systemInfo.GetVersion();

        // Assert
        Assert.NotNull(version);
        Assert.NotEmpty(version);
        Assert.StartsWith("0.", version);
    }

    [Fact]
    public void SystemInfo_Reports_System_Ready()
    {
        // Arrange
        var systemInfo = new SystemInfo();

        // Act
        var isReady = systemInfo.IsSystemReady();

        // Assert
        Assert.True(isReady, "System should be ready for development");
    }
}
