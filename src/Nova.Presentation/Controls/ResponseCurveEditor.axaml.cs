using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace Nova.Presentation.Controls;

/// <summary>
/// Interactive Bézier curve editor for expression pedal response mapping.
/// Allows user to define how pedal position (0-100%) maps to parameter value via a curve.
/// </summary>
public partial class ResponseCurveEditor : UserControl
{
    // Control points for cubic Bézier curve (P0, P1, P2, P3)
    private Point _p0 = new(0, 300);      // Start point (bottom-left)
    private Point _p1 = new(133, 200);    // First control point
    private Point _p2 = new(267, 100);    // Second control point
    private Point _p3 = new(400, 0);      // End point (top-right)

    private int _draggedPointIndex = -1;
    private const double ControlPointRadius = 8;

    public ResponseCurveEditor()
    {
        InitializeComponent();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var canvas = this.FindControl<Canvas>("CurveCanvas");
        if (canvas == null) return;

        // Draw grid
        DrawGrid(context);

        // Draw Bézier curve
        DrawCurve(context);

        // Draw control points
        DrawControlPoints(context);
    }

    private void DrawGrid(DrawingContext context)
    {
        var gridPen = new Pen(new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)), 1);

        // Vertical lines (every 10%)
        for (int i = 0; i <= 10; i++)
        {
            double x = i * 40;
            context.DrawLine(gridPen, new Point(x, 0), new Point(x, 300));
        }

        // Horizontal lines (every 10%)
        for (int i = 0; i <= 10; i++)
        {
            double y = i * 30;
            context.DrawLine(gridPen, new Point(0, y), new Point(400, y));
        }
    }

    private void DrawCurve(DrawingContext context)
    {
        var curvePen = new Pen(new SolidColorBrush(Colors.Cyan), 2);
        var controlLinePen = new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 255, 0)), 1, new DashStyle(new[] { 4.0, 4.0 }, 0));

        // Draw control lines (dashed yellow)
        context.DrawLine(controlLinePen, _p0, _p1);
        context.DrawLine(controlLinePen, _p2, _p3);

        // Draw Bézier curve (cyan)
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = _p0, IsClosed = false };
        
        var bezierSegment = new BezierSegment
        {
            Point1 = _p1,
            Point2 = _p2,
            Point3 = _p3
        };
        figure.Segments?.Add(bezierSegment);
        geometry.Figures?.Add(figure);

        context.DrawGeometry(null, curvePen, geometry);
    }

    private void DrawControlPoints(DrawingContext context)
    {
        var pointBrush = new SolidColorBrush(Colors.Yellow);
        var pointPen = new Pen(new SolidColorBrush(Colors.Black), 2);

        // Draw P0 and P3 (endpoints - not draggable)
        DrawPoint(context, _p0, Brushes.Gray, pointPen);
        DrawPoint(context, _p3, Brushes.Gray, pointPen);

        // Draw P1 and P2 (control points - draggable)
        DrawPoint(context, _p1, pointBrush, pointPen);
        DrawPoint(context, _p2, pointBrush, pointPen);
    }

    private void DrawPoint(DrawingContext context, Point point, IBrush brush, Pen pen)
    {
        context.DrawEllipse(brush, pen, point, ControlPointRadius, ControlPointRadius);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var pos = e.GetPosition(this);
        _draggedPointIndex = GetControlPointAt(pos);
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedPointIndex == -1) return;

        var pos = e.GetPosition(this);

        // Clamp position to canvas bounds
        pos = new Point(
            Math.Clamp(pos.X, 0, 400),
            Math.Clamp(pos.Y, 0, 300)
        );

        // Update control point position (only P1 and P2 are movable)
        if (_draggedPointIndex == 1)
            _p1 = pos;
        else if (_draggedPointIndex == 2)
            _p2 = pos;

        InvalidateVisual();
        e.Handled = true;
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _draggedPointIndex = -1;
        e.Handled = true;
    }

    private int GetControlPointAt(Point pos)
    {
        // Check P1 and P2 only (endpoints are fixed)
        if (IsPointNear(pos, _p1)) return 1;
        if (IsPointNear(pos, _p2)) return 2;
        return -1;
    }

    private bool IsPointNear(Point a, Point b)
    {
        double distance = Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        return distance <= ControlPointRadius * 2;
    }

    /// <summary>
    /// Evaluates the Bézier curve at position t (0-1) and returns normalized value (0-1).
    /// </summary>
    public double EvaluateCurve(double t)
    {
        t = Math.Clamp(t, 0, 1);

        // Cubic Bézier formula: B(t) = (1-t)³P0 + 3(1-t)²tP1 + 3(1-t)t²P2 + t³P3
        double u = 1 - t;
        double tt = t * t;
        double uu = u * u;
        double uuu = uu * u;
        double ttt = tt * t;

        // Y coordinate only (normalized to 0-1, inverted because Y axis goes down)
        double y = uuu * _p0.Y + 3 * uu * t * _p1.Y + 3 * u * tt * _p2.Y + ttt * _p3.Y;
        
        // Normalize and invert (300 = 0%, 0 = 100%)
        return 1.0 - (y / 300.0);
    }

    /// <summary>
    /// Gets the current curve control points for serialization.
    /// Returns normalized coordinates (0-1).
    /// </summary>
    public (double p1x, double p1y, double p2x, double p2y) GetControlPoints()
    {
        return (
            _p1.X / 400.0,
            1.0 - (_p1.Y / 300.0),
            _p2.X / 400.0,
            1.0 - (_p2.Y / 300.0)
        );
    }

    /// <summary>
    /// Sets the curve control points from normalized coordinates (0-1).
    /// </summary>
    public void SetControlPoints(double p1x, double p1y, double p2x, double p2y)
    {
        _p1 = new Point(p1x * 400, (1.0 - p1y) * 300);
        _p2 = new Point(p2x * 400, (1.0 - p2y) * 300);
        InvalidateVisual();
    }
}
