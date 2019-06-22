using System;
using DocumentFormat.OpenXml.Drawing;

namespace SimpleWord
{
    public class ColumnDefinition<TDataClass>
    {
        public string HeaderText { get; set; }
        public System.Drawing.Color HeaderBackgroundColor { get; set; }
        public System.Drawing.Color HeaderForegroundColor { get; set; }
        public ColumnWidthUnitType WidthUnit { get; set; } = ColumnWidthUnitType.Points;
        public int Width { get; set; } = 50;

        public Func<TDataClass, System.Drawing.Color> BackgroundColor = (d) => new DefaultColorScheme().DefaultBackgroundColor;
        public Func<TDataClass, System.Drawing.Color> ForegroundColor = (d) => new DefaultColorScheme().DefaultForegroundColor;

        public Func<TDataClass, string> Contents { get; set; }
        public Action<TDataClass, TableCell> Formatting { get; set; } = (d, tc) => { };

        public ColorScheme ColorScheme { get; set; }

        public ColumnDefinition(ColorScheme scheme)
        {
            if (scheme != null)
            {
                ColorScheme = scheme;
                HeaderBackgroundColor = ColorScheme.DefaultTableHeaderBackgroundColor;
                HeaderForegroundColor = ColorScheme.DefaultTableHeaderForegroundColor;
            }
        }
    }
}