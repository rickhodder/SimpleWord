using System.Collections.Generic;

namespace DbDocumenter
{
    public class SchemaTable
    {
        public Schema Schema { get; set; }
        public string TableName { get; set; }
        public List<TableField> Fields { get; set; } = new List<TableField>();
    }
}