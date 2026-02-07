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
        var action = () => vm.Decay = 0;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Decay must be between 1 and 200*");
    }

    [Fact]
    public void Decay_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.Decay = 201;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Decay must be between 1 and 200*");
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
            .WithMessage("*PreDelay must be between 0 and 100 ms*");
    }

    [Fact]
    public void PreDelay_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.PreDelay = 101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*PreDelay must be between 0 and 100 ms*");
    }

    [Fact]
    public void ReverbLevel_WithValidValue_Sets()
    {
        var vm = new ReverbBlockViewModel();
        vm.ReverbLevel = -50;
        vm.ReverbLevel.Should().Be(-50);
    }

    [Fact]
    public void ReverbLevel_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.ReverbLevel = -101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Reverb Level must be between -100 and 0 dB*");
    }

    [Fact]
    public void ReverbLevel_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new ReverbBlockViewModel();
        var action = () => vm.ReverbLevel = 1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Reverb Level must be between -100 and 0 dB*");
    }
}
