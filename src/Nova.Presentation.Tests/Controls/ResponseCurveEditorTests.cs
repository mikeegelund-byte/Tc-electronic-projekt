using Avalonia;
using Nova.Presentation.Controls;
using Xunit;

namespace Nova.Presentation.Tests.Controls;

public class ResponseCurveEditorTests
{
    [Fact]
    public void EvaluateCurve_AtStart_Returns0()
    {
        // Arrange
        var editor = new ResponseCurveEditor();
        
        // Act
        var result = editor.EvaluateCurve(0);
        
        // Assert
        Assert.Equal(0, result, precision: 5);
    }

    [Fact]
    public void EvaluateCurve_AtEnd_Returns1()
    {
        // Arrange
        var editor = new ResponseCurveEditor();
        
        // Act
        var result = editor.EvaluateCurve(1);
        
        // Assert
        Assert.Equal(1, result, precision: 5);
    }

    [Fact]
    public void EvaluateCurve_AtMidpoint_ReturnsExpectedValue()
    {
        // Arrange
        var editor = new ResponseCurveEditor();
        
        // Act
        var result = editor.EvaluateCurve(0.5);
        
        // Assert
        // With default control points, the midpoint should be close to 0.5
        // (linear interpolation would be exactly 0.5)
        Assert.InRange(result, 0.4, 0.6);
    }

    [Fact]
    public void EvaluateCurve_MonotonicallyIncreasing()
    {
        // Arrange
        var editor = new ResponseCurveEditor();
        
        // Act & Assert
        // Curve should be monotonically increasing from 0 to 1
        double prev = 0;
        for (double t = 0; t <= 1; t += 0.1)
        {
            double current = editor.EvaluateCurve(t);
            Assert.True(current >= prev, $"Curve not monotonic at t={t}");
            prev = current;
        }
    }

    [Fact]
    public void GetControlPoints_ReturnsNormalizedCoordinates()
    {
        // Arrange
        var editor = new ResponseCurveEditor();
        
        // Act
        var (p1x, p1y, p2x, p2y) = editor.GetControlPoints();
        
        // Assert
        Assert.InRange(p1x, 0, 1);
        Assert.InRange(p1y, 0, 1);
        Assert.InRange(p2x, 0, 1);
        Assert.InRange(p2y, 0, 1);
    }

    [Fact]
    public void SetControlPoints_UpdatesCurveShape()
    {
        // Arrange
        var editor = new ResponseCurveEditor();
        
        // Act
        // Set control points for exponential curve (more weight at start)
        editor.SetControlPoints(0.2, 0.1, 0.8, 0.5);
        var earlyValue = editor.EvaluateCurve(0.25);
        
        // Set control points for logarithmic curve (more weight at end)
        editor.SetControlPoints(0.2, 0.5, 0.8, 0.9);
        var lateValue = editor.EvaluateCurve(0.25);
        
        // Assert
        // Different control points should produce different curve shapes
        // At t=0.25, exponential curve should give lower value than logarithmic
        Assert.True(earlyValue < lateValue, 
            $"Exponential curve ({earlyValue}) should be lower than logarithmic ({lateValue}) at t=0.25");
    }

    [Fact]
    public void SetControlPoints_ValidRange_DoesNotThrow()
    {
        // Arrange
        var editor = new ResponseCurveEditor();
        
        // Act & Assert
        // Should not throw for valid normalized coordinates
        editor.SetControlPoints(0, 0, 0, 0);
        editor.SetControlPoints(1, 1, 1, 1);
        editor.SetControlPoints(0.5, 0.5, 0.5, 0.5);
    }
}
