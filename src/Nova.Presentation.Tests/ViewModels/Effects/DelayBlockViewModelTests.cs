using System;
using FluentAssertions;
using Nova.Presentation.ViewModels.Effects;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels.Effects;

public class DelayBlockViewModelTests
{
    [Fact]
    public void Time_WithValidValue_Sets()
    {
        var vm = new DelayBlockViewModel();
        vm.Time = 500;
        vm.Time.Should().Be(500);
    }

    [Fact]
    public void Time_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new DelayBlockViewModel();
        var action = () => vm.Time = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Time must be between 0 and 1800 ms*");
    }

    [Fact]
    public void Time_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new DelayBlockViewModel();
        var action = () => vm.Time = 2001;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Time must be between 0 and 1800 ms*");
    }

    [Fact]
    public void Feedback_WithValidValue_Sets()
    {
        var vm = new DelayBlockViewModel();
        vm.Feedback = 50;
        vm.Feedback.Should().Be(50);
    }

    [Fact]
    public void Feedback_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new DelayBlockViewModel();
        var action = () => vm.Feedback = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Feedback must be between 0 and 120%*");
    }

    [Fact]
    public void Feedback_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new DelayBlockViewModel();
        var action = () => vm.Feedback = 121;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Feedback must be between 0 and 120%*");
    }

    [Fact]
    public void Mix_WithValidValue_Sets()
    {
        var vm = new DelayBlockViewModel();
        vm.Mix = 75;
        vm.Mix.Should().Be(75);
    }

    [Fact]
    public void Mix_WithValueBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var vm = new DelayBlockViewModel();
        var action = () => vm.Mix = -1;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Mix must be between 0 and 100%*");
    }

    [Fact]
    public void Mix_WithValueAboveMax_ThrowsArgumentOutOfRangeException()
    {
        var vm = new DelayBlockViewModel();
        var action = () => vm.Mix = 101;
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Mix must be between 0 and 100%*");
    }
}
