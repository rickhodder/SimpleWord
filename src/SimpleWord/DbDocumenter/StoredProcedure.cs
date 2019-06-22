using System.Collections.Generic;

namespace DbDocumenter
{
    public class StoredProcedure
    {
        public Schema Schema { get; set; }
        public string StoredProcedureName { get; set; }
        public int IsSqlClr { get; set; }
        public List<StoredProcedureParameter> Parameters { get; set; } = new List<StoredProcedureParameter>();
    }
}