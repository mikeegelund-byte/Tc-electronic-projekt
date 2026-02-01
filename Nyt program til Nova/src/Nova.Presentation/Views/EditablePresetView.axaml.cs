using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Nova.Presentation.Views;

public partial class EditablePresetView : UserControl
{
    public EditablePresetView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
