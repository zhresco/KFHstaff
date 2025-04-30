using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KFHstaff
{
    public class UserRoleToVisibilityConverter : IValueConverter
    {
        // Преобразование роли в видимость (видно только для User)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string role = value as string;
            return string.Equals(role, "User", StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Обратное преобразование (не используется)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}