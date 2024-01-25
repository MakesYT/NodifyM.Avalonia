using System.Globalization;
using Avalonia.Data.Converters;
using Nodify.Avalonia.Controls;
using Nodify.Avalonia.ViewModelBase;

namespace Nodify.Avalonia.Converters
{
    public class FlowToDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConnectorViewModelBase.ConnectorFlow flow)
            {
                return flow == ConnectorViewModelBase.ConnectorFlow.Output ? ConnectionDirection.Forward : ConnectionDirection.Backward;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConnectionDirection dir)
            {
                return dir == ConnectionDirection.Forward ? ConnectorViewModelBase.ConnectorFlow.Output : ConnectorViewModelBase.ConnectorFlow.Input;
            }

            return value;
        }
    }
}
