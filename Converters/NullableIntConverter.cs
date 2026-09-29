using System;
using System.Globalization;
using System.Windows.Data;

namespace diNo.Converters
{
  // Bindet TextBox.Text an eine nullable Ganzzahl (int?). Ersatz für NumericUpDownNullable:
  // leeres Feld bleibt "kein Wert" (null), eine tatsächlich eingegebene, aber ungültige Zahl wird
  // beim Verlassen des Feldes (Standard-UpdateSourceTrigger=LostFocus) auf 0 zurückgesetzt. Optional
  // per ConverterParameter="min,max" (z.B. "0,15") wird der Wert zusätzlich in diesen Bereich geklemmt.
  public class NullableIntConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      => value == null ? "" : value.ToString();

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var text = (value as string)?.Trim();
      if (string.IsNullOrEmpty(text)) return null;
      if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var result)) return 0;

      if (parameter is string range)
      {
        var parts = range.Split(',');
        if (parts.Length == 2 && int.TryParse(parts[0], out var min) && int.TryParse(parts[1], out var max))
        {
          if (result < min) result = min;
          if (result > max) result = max;
        }
      }

      return (int?)result;
    }
  }
}
