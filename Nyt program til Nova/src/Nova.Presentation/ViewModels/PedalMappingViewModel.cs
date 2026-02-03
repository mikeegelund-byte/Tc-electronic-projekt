using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

public partial class PedalMappingViewModel : ObservableObject
{
    [ObservableProperty]
    private int _parameter;

    [ObservableProperty]
    private int _min;

    [ObservableProperty]
    private int _mid;

    [ObservableProperty]
    private int _max;

    [ObservableProperty]
    private double _curveP1X = 0.33;

    [ObservableProperty]
    private double _curveP1Y = 0.67;

    [ObservableProperty]
    private double _curveP2X = 0.67;

    [ObservableProperty]
    private double _curveP2Y = 0.33;

    public void LoadFromDump(SystemDump dump)
    {
        Parameter = dump.GetPedalParameter();
        Min = dump.GetPedalMin();
        Mid = dump.GetPedalMid();
        Max = dump.GetPedalMax();
        
        // Load curve control points if stored (future enhancement)
        // For now, use default curve
    }

    /// <summary>
    /// Sets the response curve control points from the editor.
    /// </summary>
    public void SetCurvePoints(double p1x, double p1y, double p2x, double p2y)
    {
        CurveP1X = p1x;
        CurveP1Y = p1y;
        CurveP2X = p2x;
        CurveP2Y = p2y;
    }
}
