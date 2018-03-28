using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Vishnu.Messenger.Common;

namespace Vishnu.Messenger.Test.UiClient.Converters
{
    class StatusEnumToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserStatus status = (UserStatus)value;
            SolidColorBrush result = new SolidColorBrush();
            switch(status)
            {
                case UserStatus.Active:
                    result.Color = Colors.YellowGreen;
                    break;
                case UserStatus.Busy:
                    result.Color = Colors.Orange;
                    break;
                case UserStatus.DoNotDisturb:
                    result.Color = Colors.Red;
                    break;
                case UserStatus.InActive:
                    result.Color = Colors.Yellow;
                    break;
                case UserStatus.LogOut:
                    result.Color = Colors.LightGray;
                    break;
            }
            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
