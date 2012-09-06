﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ClearStyle.Converters
{
  public class ColorToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new SolidColorBrush((Color)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new System.NotImplementedException();
    }
  }
}
