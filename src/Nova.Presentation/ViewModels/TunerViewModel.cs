using CommunityToolkit.Mvvm.ComponentModel;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for the tuner UI stub. Real-time audio integration is deferred.
/// </summary>
public partial class TunerViewModel : ObservableObject
{
    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private string _note = "--";
    [ObservableProperty] private double _frequency;
    [ObservableProperty] private int _cents;
    [ObservableProperty] private string _statusMessage = "Tuner idle";

    public int CentsOffset => Cents + 50;

    partial void OnCentsChanged(int value)
    {
        OnPropertyChanged(nameof(CentsOffset));
    }
}
