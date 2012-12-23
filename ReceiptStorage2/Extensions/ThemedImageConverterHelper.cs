using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ReceiptStorage.Extensions
{
    public class ThemedImageConverterHelper : IValueConverter
    {
        private static Dictionary<String, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();
        private static Visibility currentTheme;
        private static string assetPath = "/Content/Images/dark/";


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage result = null;
            // Detect current theme
            this.DetectTheme((Color)parameter);
            // Path to the icon image
            string path = assetPath + (string)value;
            // Check if we already cached the image
            if (!imageCache.TryGetValue(path, out result))
            {
                Uri source = new Uri(path, UriKind.Relative);
                result = new BitmapImage(source);
                imageCache.Add(path, result);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void DetectTheme(Color color)
        {
            Visibility lightThemeVisibility = Visibility.Collapsed;

            if (color == Colors.White)
            {
                lightThemeVisibility = Visibility.Visible;
            }

            // Check if the theme changed
            if (currentTheme != lightThemeVisibility)
            {
                currentTheme = lightThemeVisibility;

                if (lightThemeVisibility == Visibility.Visible)
                {
                    assetPath = "/Content/Images/dark/";
                }
                else
                {
                    assetPath = "/Content/Images/light/";
                }
            }
        }
    }
}
