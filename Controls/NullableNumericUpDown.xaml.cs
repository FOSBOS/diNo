using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace diNo.Controls
{
  // WPF-Pendant zu NumericUpDownNullable.cs (WinForms): leerer Text == null,
  // sonst gerundeter decimal-Wert. Ersetzt das new-Value-Member-Hiding-Idiom
  // durch eine normale DependencyProperty.
  public partial class NullableNumericUpDown : UserControl
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      nameof(Value), typeof(decimal?), typeof(NullableNumericUpDown),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      nameof(Minimum), typeof(decimal), typeof(NullableNumericUpDown), new PropertyMetadata(0m));

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      nameof(Maximum), typeof(decimal), typeof(NullableNumericUpDown), new PropertyMetadata(100m));

    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(
      nameof(Increment), typeof(decimal), typeof(NullableNumericUpDown), new PropertyMetadata(1m));

    public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register(
      nameof(DecimalPlaces), typeof(int), typeof(NullableNumericUpDown), new PropertyMetadata(0));

    public decimal? Value
    {
      get => (decimal?)GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    public decimal Minimum
    {
      get => (decimal)GetValue(MinimumProperty);
      set => SetValue(MinimumProperty, value);
    }

    public decimal Maximum
    {
      get => (decimal)GetValue(MaximumProperty);
      set => SetValue(MaximumProperty, value);
    }

    public decimal Increment
    {
      get => (decimal)GetValue(IncrementProperty);
      set => SetValue(IncrementProperty, value);
    }

    public int DecimalPlaces
    {
      get => (int)GetValue(DecimalPlacesProperty);
      set => SetValue(DecimalPlacesProperty, value);
    }

    public NullableNumericUpDown()
    {
      InitializeComponent();
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (NullableNumericUpDown)d;
      var text = e.NewValue is decimal v ? v.ToString(CultureInfo.CurrentCulture) : "";
      if (control.PART_TextBox.Text != text)
        control.PART_TextBox.Text = text;
    }

    private void CommitTextBox()
    {
      var text = PART_TextBox.Text.Trim();
      if (text.Length == 0)
      {
        Value = null;
        return;
      }

      if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
      {
        parsed = Math.Round(parsed, DecimalPlaces, MidpointRounding.AwayFromZero);
        if (parsed < Minimum) parsed = Minimum;
        if (parsed > Maximum) parsed = Maximum;
        Value = parsed;
        PART_TextBox.Text = parsed.ToString(CultureInfo.CurrentCulture);
      }
      else
      {
        PART_TextBox.Text = Value?.ToString(CultureInfo.CurrentCulture) ?? "";
      }
    }

    private void Step(decimal delta)
    {
      var next = (Value ?? 0m) + delta;
      if (next < Minimum) next = Minimum;
      if (next > Maximum) next = Maximum;
      Value = Math.Round(next, DecimalPlaces, MidpointRounding.AwayFromZero);
    }

    private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e) => CommitTextBox();

    private void PART_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      string allowed = "0123456789" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "-";
      e.Handled = e.Text.Any(c => !allowed.Contains(c));
    }

    private void PART_UpButton_Click(object sender, RoutedEventArgs e) => Step(Increment);
    private void PART_DownButton_Click(object sender, RoutedEventArgs e) => Step(-Increment);
  }
}
