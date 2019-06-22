namespace DbDocumenter
{
    public class Data
    {
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsCheck { get; set; }
        public bool IsUniqueKey { get; set; }
        public string Table_Catalog { get; set; }
        public string Table_Schema { get; set; }
        public string Table_Name { get; set; }
        public string Column_Name { get; set; }
        public string Data_Type { get; set; }
        public int? Character_Maximum_Length { get; set; }
        public int? Numeric_Precision { get; set; }
        public int? DateTime_Precision { get; set; }
        public int Ordinal_Position { get; set; }
        public string fk_constraint_name { get; set; }
        public string ck_constraint_name { get; set; }
        public string uk_constraint_name { get; set; }
    }
}
