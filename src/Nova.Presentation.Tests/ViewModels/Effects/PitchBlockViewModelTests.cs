using System;
using FluentAssertions;
using Nova.Presentation.ViewModels.Effects;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels.Effects;

public class PitchBlockViewModelTests
{
    [Fact]
    public void Type_WithValidValue_Sets()
    {
        var vm = new PitchBlockViewModel();
        vm.Type = 2;
        vm.Type.Should().Be(2);
    }

    [Fact]
    public void Type_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Type = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Type must be between 0 and 4*");
    }

    [Fact]
    public void Type_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Type = 5;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Type must be between 0 and 4*");
    }

    [Fact]
    public void Voice1_WithValidValue_Sets()
    {
        var vm = new PitchBlockViewModel();
        vm.Voice1 = 50;
        vm.Voice1.Should().Be(50);
    }

    [Fact]
    public void Voice1_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Voice1 = -101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Voice1 must be between -100 and +100 cents*");
    }

    [Fact]
    public void Voice1_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Voice1 = 101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Voice1 must be between -100 and +100 cents*");
    }

    [Fact]
    public void Voice2_WithValidValue_Sets()
    {
        var vm = new PitchBlockViewModel();
        vm.Voice2 = -25;
        vm.Voice2.Should().Be(-25);
    }

    [Fact]
    public void Voice2_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Voice2 = -101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Voice2 must be between -100 and +100 cents*");
    }

    [Fact]
    public void Voice2_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Voice2 = 101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Voice2 must be between -100 and +100 cents*");
    }

    [Fact]
    public void Mix_WithValidValue_Sets()
    {
        var vm = new PitchBlockViewModel();
        vm.Mix = 75;
        vm.Mix.Should().Be(75);
    }

    [Fact]
    public void Mix_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Mix = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Mix must be between 0 and 100%*");
    }

    [Fact]
    public void Mix_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PitchBlockViewModel();
        var action = () => vm.Mix = 101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Mix must be between 0 and 100%*");
    }
}
