using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MomenMedmSys.Core.Enums;

namespace MomenMedmSys.WPF.Converters
{
    /// <summary>
    /// Converts NotificationPriority to a Material Design icon path
    /// </summary>
    public class NotificationPriorityToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotificationPriority priority)
            {
                return priority switch
                {
                    NotificationPriority.Low => "M12 2A10 10 0 1 0 22 12A10 10 0 0 0 12 2M12 20A8 8 0 1 1 20 12A8 8 0 0 1 12 20M12 6A6 6 0 1 0 18 12A6 6 0 0 0 12 6M12 16A4 4 0 1 1 16 12A4 4 0 0 1 12 16Z",
                    NotificationPriority.Medium => "M12 2L1 21H23M12 6L19.53 19H4.47M11 10V14H13V10M11 16V18H13V16Z",
                    NotificationPriority.High => "M12 2L2 22H22M12 8L18 18H6M11 12V16H13V12Z",
                    NotificationPriority.Critical => "M13 14H11V9H13M13 18H11V16H13M1 21H23L12 2L1 21Z",
                    _ => "M12 2A10 10 0 1 0 22 12A10 10 0 0 0 12 2Z"
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts NotificationPriority to a color brush
    /// </summary>
    public class NotificationPriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotificationPriority priority)
            {
                return priority switch
                {
                    NotificationPriority.Low => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2")),     // Blue - Info
                    NotificationPriority.Medium => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F57C00")),  // Orange - Warning
                    NotificationPriority.High => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F")),    // Red - High
                    NotificationPriority.Critical => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828")), // Dark Red - Critical
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts NotificationType to a Material Design icon path
    /// </summary>
    public class NotificationTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NotificationType type)
            {
                return type switch
                {
                    NotificationType.Maintenance => "M22.7 19L13.6 9.9C14.5 7.6 14 4.9 12.1 3C10.1 1 7.1 0.6 4.7 1.9L9 6.2C7.9 6.9 7 8.1 7 9.5C7 12.3 9.2 14.5 12 14.5C12.5 14.5 13 14.4 13.4 14.3L18 18.9L22.7 19Z",
                    NotificationType.Calibration => "M19.8 18.4L14 10.6L16.1 8.5L18.2 10.6L21.5 7.3L19.4 5.2L17.3 7.3L15.2 5.2L13.1 7.3L5.2 15.2L3.1 13.1L1 15.2L3.1 17.3L5.2 19.4L13.1 11.5L15.2 13.6L17.3 11.5L19.4 13.6L21.5 15.7L19.8 18.4Z",
                    NotificationType.Warranty => "M12 1L3 5V11C3 16.6 7 21.4 12 22C17 21.4 21 16.6 21 11V5L12 1ZM12 11.9H11V7H13V11.9H12Z",
                    NotificationType.Stock => "M20 2H4C3 2 2 3 2 4V20C2 21 3 22 4 22H20C21 22 22 21 22 20V4C22 3 21 2 20 2M20 20H4V4H20V20M18 6H6V10H18V6M6 12H18V16H6V12Z",
                    NotificationType.Risk => "M1 21H23L12 2L1 21M13 18H11V16H13V18M13 14H11V10H13V14Z",
                    NotificationType.System => "M12 2A10 10 0 1 0 22 12A10 10 0 0 0 12 2M12 20A8 8 0 1 1 20 12A8 8 0 0 1 12 20M12 6A6 6 0 1 0 18 12A6 6 0 0 0 12 6M12 16A4 4 0 1 1 16 12A4 4 0 0 1 12 16Z",
                    _ => "M12 2A10 10 0 1 0 22 12A10 10 0 0 0 12 2Z"
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
