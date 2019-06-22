namespace DbDocumenter
{
    public class StoredProcedureData
    {
        public string SchemaName { get; set; }
        public string StoredProcedureName { get; set; }
        public int IsSqlClr { get; set; }
        public int ParameterOrder { get; set; }
        public string ParameterName { get; set; }
        public string ParameterDirection { get; set; }
        public string DataType { get; set; }
        public int? Length { get; set; }
    }
}