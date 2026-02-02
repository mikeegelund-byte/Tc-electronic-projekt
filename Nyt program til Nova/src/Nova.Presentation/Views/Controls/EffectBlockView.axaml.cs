using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace Nova.Presentation.Views.Controls;

/// <summary>
/// Reusable UserControl for displaying effect block parameters.
/// Shows a collapsible Expander with on/off indicator, effect type, and parameter grid.
/// </summary>
public partial class EffectBlockView : UserControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<EffectBlockView, string>(nameof(Header), "Effect");

    public static readonly StyledProperty<object?> ParametersContentProperty =
        AvaloniaProperty.Register<EffectBlockView, object?>(nameof(ParametersContent));

    public static readonly StyledProperty<bool> IsEnabledIndicatorProperty =
        AvaloniaProperty.Register<EffectBlockView, bool>(nameof(IsEnabledIndicator));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? ParametersContent
    {
        get => GetValue(ParametersContentProperty);
        set => SetValue(ParametersContentProperty, value);
    }

    public bool IsEnabledIndicator
    {
        get => GetValue(IsEnabledIndicatorProperty);
        set => SetValue(IsEnabledIndicatorProperty, value);
    }

    public EffectBlockView()
    {
        InitializeComponent();
        
        // Update indicator color when IsEnabledIndicator changes
        PropertyChanged += (s, e) =>
        {
            if (e.Property == IsEnabledIndicatorProperty)
            {
                UpdateIndicatorColor();
            }
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateIndicatorColor();
    }

    private void UpdateIndicatorColor()
    {
        var indicator = this.FindControl<Ellipse>("StatusIndicator");
        if (indicator != null)
        {
            indicator.Fill = IsEnabledIndicator 
                ? new SolidColorBrush(Color.Parse("#00FF00"))  // Green
                : new SolidColorBrush(Color.Parse("#666666"));  // Gray
        }
    }
}
