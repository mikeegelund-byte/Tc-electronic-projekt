using FluentAssertions;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;

namespace Nova.Presentation.Tests.ViewModels;

public class PresetSummaryViewModelTests
{
    [Theory]
    [InlineData(31, "00-1")] // First preset in bank 0, slot 1
    [InlineData(32, "00-2")] // Bank 0, slot 2
    [InlineData(33, "00-3")] // Bank 0, slot 3
    [InlineData(34, "01-1")] // Bank 1, slot 1
    [InlineData(37, "02-1")] // Bank 2, slot 1
    [InlineData(90, "19-3")] // Last preset in bank 19, slot 3
    public void FromPreset_CalculatesCorrectPosition(int presetNumber, string expectedPosition)
    {
        // Arrange
        var preset = CreateTestPreset(presetNumber, "Test Name");

        // Act
        var viewModel = PresetSummaryViewModel.FromPreset(preset);

        // Assert
        viewModel.Position.Should().Be(expectedPosition);
    }

    [Fact]
    public void FromPreset_MapsPresetNumber()
    {
        // Arrange
        var preset = CreateTestPreset(45, "Test Preset");

        // Act
        var viewModel = PresetSummaryViewModel.FromPreset(preset);

        // Assert
        viewModel.Number.Should().Be(45);
    }

    [Fact]
    public void FromPreset_MapsPresetName()
    {
        // Arrange
        var preset = CreateTestPreset(31, "My Custom Preset");

        // Act
        var viewModel = PresetSummaryViewModel.FromPreset(preset);

        // Assert
        viewModel.Name.Should().Be("My Custom Preset");
    }

    [Fact]
    public void FromPreset_CalculatesBankGroup()
    {
        // Arrange - Preset 34 should be in bank group 1
        var preset = CreateTestPreset(34, "Test");

        // Act
        var viewModel = PresetSummaryViewModel.FromPreset(preset);

        // Assert
        viewModel.BankGroup.Should().Be(1);
    }

    [Theory]
    [InlineData(31, 0)]  // First bank
    [InlineData(40, 3)]  // Bank 3
    [InlineData(88, 19)] // Last bank
    public void FromPreset_CalculatesCorrectBankGroup(int presetNumber, int expectedBankGroup)
    {
        // Arrange
        var preset = CreateTestPreset(presetNumber, "Test");

        // Act
        var viewModel = PresetSummaryViewModel.FromPreset(preset);

        // Assert
        viewModel.BankGroup.Should().Be(expectedBankGroup);
    }

    private static Preset CreateTestPreset(int number, string name)
    {
        return TestHelpers.CreateValidPreset(number, name);
    }
}
