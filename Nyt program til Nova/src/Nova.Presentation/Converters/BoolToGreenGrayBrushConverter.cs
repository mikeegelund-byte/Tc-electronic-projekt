using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Nova.Presentation.Converters;

/// <summary>
/// Converts a boolean value to a SolidColorBrush.
/// True = Green (#4CAF50), False = Gray (#757575)
/// </summary>
public class BoolToGreenGrayBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue 
                ? new SolidColorBrush(Color.Parse("#4CAF50")) 
                : new SolidColorBrush(Color.Parse("#757575"));
        }
        
        return new SolidColorBrush(Color.Parse("#757575"));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
