using System.Collections.Generic;

namespace DbDocumenter
{
    public class Schema
    {
        public string SchemaName { get; set; }
        public List<SchemaTable> Tables { get; set; } = new List<SchemaTable>();
        public List<StoredProcedure> StoredProcedures { get; set; } = new List<StoredProcedure>();

    }
}