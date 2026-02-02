using System;
using FluentAssertions;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public sealed class PresetDetailViewModelGlobalTests
{
    [Fact]
    public void TapTempo_WithValidValue_Sets()
    {
        var vm = new PresetDetailViewModel();
        vm.TapTempo = 120;
        vm.TapTempo.Should().Be(120);
    }

    [Fact]
    public void TapTempo_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.TapTempo = -1;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*TapTempo must be between 0 and 255*");
    }

    [Fact]
    public void TapTempo_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.TapTempo = 256;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*TapTempo must be between 0 and 255*");
    }

    [Fact]
    public void Routing_WithValidValue_Sets()
    {
        var vm = new PresetDetailViewModel();
        vm.Routing = 3;
        vm.Routing.Should().Be(3);
    }

    [Fact]
    public void Routing_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.Routing = -1;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Routing must be between 0 and 7*");
    }

    [Fact]
    public void Routing_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.Routing = 8;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Routing must be between 0 and 7*");
    }

    [Fact]
    public void LevelOutLeft_WithValidValue_Sets()
    {
        var vm = new PresetDetailViewModel();
        vm.LevelOutLeft = 5;
        vm.LevelOutLeft.Should().Be(5);
    }

    [Fact]
    public void LevelOutLeft_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.LevelOutLeft = -21;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*LevelOutLeft must be between -20 and 20*");
    }

    [Fact]
    public void LevelOutLeft_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.LevelOutLeft = 21;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*LevelOutLeft must be between -20 and 20*");
    }

    [Fact]
    public void LevelOutRight_WithValidValue_Sets()
    {
        var vm = new PresetDetailViewModel();
        vm.LevelOutRight = -10;
        vm.LevelOutRight.Should().Be(-10);
    }

    [Fact]
    public void LevelOutRight_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.LevelOutRight = -21;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*LevelOutRight must be between -20 and 20*");
    }

    [Fact]
    public void LevelOutRight_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new PresetDetailViewModel();
        Action act = () => vm.LevelOutRight = 21;
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*LevelOutRight must be between -20 and 20*");
    }
}
