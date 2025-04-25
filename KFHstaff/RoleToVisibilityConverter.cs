using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KFHstaff
{
    public class RoleToVisibilityConverter : IValueConverter
    {
        // Преобразование роли в видимость
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string role = value as string;
            return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Обратное преобразование (не используется)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}