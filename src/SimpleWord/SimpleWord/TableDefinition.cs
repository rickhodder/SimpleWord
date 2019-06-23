using System.Collections.Generic;

namespace SimpleWord
{
    public class TableDefinition<TDataClass>
    {
        public TableDefinitionBorderType BorderType { get; set; } = TableDefinitionBorderType.Single;
        public int BorderWidth { get; set; } = 1;
        public ColorScheme ColorScheme { get; set; }
        public List<ColumnDefinition<TDataClass>> Columns { get; set; }

        public TableDefinition(ColorScheme scheme)
        {
            ColorScheme = scheme;
        }
    }
}