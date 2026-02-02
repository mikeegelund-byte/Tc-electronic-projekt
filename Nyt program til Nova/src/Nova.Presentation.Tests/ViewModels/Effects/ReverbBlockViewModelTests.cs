using System;
using FluentAssertions;
using Nova.Presentation.ViewModels.Effects;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels.Effects;

public class ReverbBlockViewModelTests
{
    [Fact]
    public void Decay_WithValidValue_Sets()
    {
        var vm = new ReverbBlockViewModel();
        vm.Decay = 50;
        vm.Decay.Should().Be(50);
    }

    [Fact]
    public void Decay_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.Decay = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Decay must be between 0 and 100%*");
    }

    [Fact]
    public void Decay_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.Decay = 101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Decay must be between 0 and 100%*");
    }

    [Fact]
    public void PreDelay_WithValidValue_Sets()
    {
        var vm = new ReverbBlockViewModel();
        vm.PreDelay = 100;
        vm.PreDelay.Should().Be(100);
    }

    [Fact]
    public void PreDelay_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.PreDelay = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*PreDelay must be between 0 and 200 ms*");
    }

    [Fact]
    public void PreDelay_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.PreDelay = 201;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*PreDelay must be between 0 and 200 ms*");
    }

    [Fact]
    public void Level_WithValidValue_Sets()
    {
        var vm = new ReverbBlockViewModel();
        vm.Level = 75;
        vm.Level.Should().Be(75);
    }

    [Fact]
    public void Level_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.Level = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Level must be between 0 and 100%*");
    }

    [Fact]
    public void Level_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.Level = 101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Level must be between 0 and 100%*");
    }
}
