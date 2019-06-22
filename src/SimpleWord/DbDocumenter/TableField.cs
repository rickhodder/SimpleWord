namespace DbDocumenter
{
    public class TableField
    {
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsCheck { get; set; }
        public bool IsUniqueKey { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int? CharacterMaximumLength { get; set; }
        public int? NumericPrecision { get; set; }
        public int? DateTimePrecision { get; set; }
        public int OrdinalPosition { get; set; }
        public string ForeignKeyConstraintName { get; set; }
        public string CheckConstraintName { get; set; }
        public string UniqueKeyconstraintName { get; set; }
    }
}