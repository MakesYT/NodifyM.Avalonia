using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data.Converters;
using Avalonia.VisualTree;
using Nodify.Avalonia.Controls;
using Nodify.Avalonia.ViewModelBase;

namespace Nodify.Avalonia.Converters;

public class NodeOffsetCtr : IMultiValueConverter
{
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
       // Debug.WriteLine(values.Count);
        double sum = 0;
        for (var i = 0; i < values.Count; i++)
        {
            if (values[i] is double d)
            {
                sum+=d;
                continue;
            }

            if (values[i] is NodifyEditor nodifyEditor)
            {
                if (nodifyEditor.ItemsPanelRoot is ZoomCanvas zoomCanvas)
                {
                    if ((string)parameter =="Y")
                    {
                        sum+=zoomCanvas.OffsetY;
                    }
                    else
                    {
                        sum+=zoomCanvas.OffsetX;
                    }
                }
            }

        }
        return sum;
    }
}