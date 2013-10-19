using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GameLibrary.ViewModel
{
    public class MultiStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format((string)parameter, values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringVisibilityConverter : MatchConverter<string, Visibility> { }

    public class StringBoolConverter : MatchConverter<string, bool> { }

    public class DirectionBoolConverter : MatchConverter<ListSortDirection, bool> { }

    public class MatchConverter<TIn, TOut> : IValueConverter
    {
        // marking these as virtual gets them seen by XAML for derived classes... crazy!
        public virtual TOut DefaultValue { get; set; }
        public virtual TOut MatchedValue { get; set; }

        public virtual TIn ConverterParameter { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object safeParameter = parameter ?? this.ConverterParameter;

            bool isEqual = object.Equals(safeParameter, value);

            return isEqual ? this.MatchedValue : this.DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
