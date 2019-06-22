namespace DbDocumenter
{
    public class StoredProcedureParameter
    {
        public int ParameterOrder { get; set; }
        public string ParameterName { get; set; }
        public string ParameterDirection { get; set; }
        public string DataType { get; set; }
        public int? Length { get; set; }
    }
}