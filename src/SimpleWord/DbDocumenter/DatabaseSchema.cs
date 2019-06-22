using System.Collections.Generic;

namespace DbDocumenter
{
    public class DatabaseSchema
    {
        public string Name { get; set; }

        public List<Schema> Schemas { get; set; } = new List<Schema>();
        public List<StoredProcedureData> StoredProcedures { get; set; } = new List<StoredProcedureData>();
        public List<SchemaTable> Tables { get; set; } = new List<SchemaTable>();
    }
}