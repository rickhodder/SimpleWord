using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DbDocumenter
{
    public class DataDictionaryData
    {
        // put the connection string for the database you want to document here
        string connectionString = @"server=aa;database=bb;integrated security=true";
        public List<Data> Get()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sql =
                    @"select 
      CASE WHEN pks.Table_Catalog IS NULL THEN 0 ELSE 1 END AS isPrimaryKey, 
      CASE WHEN Fks.Table_Catalog IS NULL THEN 0 ELSE 1 END AS isForeignKey, 
      CASE WHEN Cks.Table_Catalog IS NULL THEN 0 ELSE 1 END AS isCheck, 
      CASE WHEN Uks.Table_Catalog IS NULL THEN 0 ELSE 1 END AS IsUniqueKey, 
      c.TABLE_CATALOG, c.TABLE_SCHEMA, c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE , c.CHARACTER_MAXIMUM_LENGTH, c.NUMERIC_PRECISION, c.DATETIME_PRECISION, c.ORDINAL_POSITION , fks.CONSTRAINT_NAME fk_constraint_name, cks.CONSTRAINT_NAME ck_constraint_name, uks.CONSTRAINT_NAME AS uk_constraint_name
   from INFORMATION_SCHEMA.COLUMNS c
   JOIN INFORMATION_SCHEMA.TABLES t on c.TABLE_CATALOG = t.TABLE_CATALOG AND c.TABLE_SCHEMA = t.TABLE_SCHEMA AND c.TABLE_NAME = t.TABLE_NAME AND t.TABLE_TYPE = 'BASE TABLE'
   LEFT JOIN(
     SELECT distinct ccu.TABLE_CATALOG, ccu.TABLE_SCHEMA, ccu.TABLE_NAME, ccu.COLUMN_NAME, ccu.CONSTRAINT_NAME
      FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
       JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
      WHERE tc.CONSTRAINT_TYPE = 'Primary Key'
   ) PKS ON c.TABLE_CATALOG = PKS.TABLE_CATALOG AND c.TABLE_SCHEMA = PKS.TABLE_SCHEMA AND c.TABLE_NAME = PKS.TABLE_NAME AND c.COLUMN_NAME = PKS.COLUMN_NAME
   LEFT JOIN(
     SELECT distinct ccu.TABLE_CATALOG, ccu.TABLE_SCHEMA, ccu.TABLE_NAME, ccu.COLUMN_NAME , ccu.CONSTRAINT_NAME
      FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
       JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
      WHERE tc.CONSTRAINT_TYPE = 'Foreign Key'
   ) FKS ON c.TABLE_CATALOG = FKS.TABLE_CATALOG AND c.TABLE_SCHEMA = FKS.TABLE_SCHEMA AND c.TABLE_NAME = FKS.TABLE_NAME AND c.COLUMN_NAME = FKS.COLUMN_NAME
   LEFT JOIN(
     SELECT distinct ccu.TABLE_CATALOG, ccu.TABLE_SCHEMA, ccu.TABLE_NAME, ccu.COLUMN_NAME , ccu.CONSTRAINT_NAME
      FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
       JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
      WHERE tc.CONSTRAINT_TYPE = 'Check'
   ) CKS ON c.TABLE_CATALOG = CKS.TABLE_CATALOG AND c.TABLE_SCHEMA = cKS.TABLE_SCHEMA AND c.TABLE_NAME = cKS.TABLE_NAME AND c.COLUMN_NAME = cKS.COLUMN_NAME
   LEFT JOIN(
     SELECT distinct ccu.TABLE_CATALOG, ccu.TABLE_SCHEMA, ccu.TABLE_NAME, ccu.COLUMN_NAME , ccu.CONSTRAINT_NAME
      FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
       JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
      WHERE tc.CONSTRAINT_TYPE = 'Unique'
   ) UKS ON c.TABLE_CATALOG = UKS.TABLE_CATALOG AND c.TABLE_SCHEMA = UKS.TABLE_SCHEMA AND c.TABLE_NAME = UKS.TABLE_NAME AND c.COLUMN_NAME = UKS.COLUMN_NAME
    ";

                return connection.Query<Data>(sql).ToList();
            }
        }
        public List<StoredProcedureData> GetStoredProcedures()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sql =
                    @"select 
   pr.SPECIFIC_SCHEMA as SchemaName, 
   pr.SPECIFIC_NAME as StoredProcedureName, 
   CASE 
    WHEN pr.ROUTINE_BODY='EXTERNAL' THEN 1 
    ELSE 0 
   END AS IsSqlClr, 
   p.ORDINAL_POSITION AS ParameterOrder, 
   p.PARAMETER_NAME AS ParameterName, 
   p.PARAMETER_MODE AS ParameterDirection, 
   p.DATA_TYPE AS DataType,
   p.CHARACTER_MAXIMUM_LENGTH as Length
  from information_schema.routines pr
   LEFT Join information_schema.PARAMETERS p on p.SPECIFIC_SCHEMA = pr.SPECIFIC_SCHEMA AND p.SPECIFIC_NAME =pr.SPECIFIC_NAME 
  where routine_type = 'PROCEDURE'
  ORDER BY SchemaName, StoredProcedureName, ParameterOrder
  ";


                return connection.Query<StoredProcedureData>(sql).ToList();
            }
        }

        public DatabaseSchema GetDataSchema()
        {
            var result = new DatabaseSchema();

            var tables = Get();
            var storedProcedures = GetStoredProcedures();

            var usedSchemas = tables.Select(tb => tb.Table_Schema)
                .Union(storedProcedures.Select(sp => sp.SchemaName))
                .OrderBy(t => t == "dbo" ? "aaa" : t) // sort it so that dbo comes out at the top of the list
                .ToList();


            //result.Tables = tables;
            result.StoredProcedures = storedProcedures;

            foreach (var schemaName in usedSchemas)
            {
                var schema = new Schema
                {
                    SchemaName = schemaName,
                };

                MapTableDataToField(schema, tables);
                MapStoredProcedureDataToStoredProcedures(schema, storedProcedures);

                result.Schemas.Add(schema);
            }

            return result;
        }

        public void MapTableDataToField(Schema schema, List<Data> tables)
        {
            // create tables

            var tableNames = tables
                .Where(t => t.Table_Schema == schema.SchemaName)
                .Select(t => t.Table_Name)
                .Distinct()
                .ToList();

            foreach (var tableName in tableNames)
            {
                var table = new SchemaTable
                {
                    Schema = schema,
                    TableName = tableName
                };

                schema.Tables.Add(table);

                foreach (var fieldInfo in tables.Where(t => t.Table_Schema == schema.SchemaName && t.Table_Name == tableName))
                {
                    table.Fields.Add(
                        new TableField
                        {
                            CharacterMaximumLength = fieldInfo.Character_Maximum_Length,
                            ColumnName = fieldInfo.Column_Name,
                            CheckConstraintName = fieldInfo.ck_constraint_name,
                            DataType = fieldInfo.Data_Type,
                            DateTimePrecision = fieldInfo.DateTime_Precision,
                            ForeignKeyConstraintName = fieldInfo.fk_constraint_name,
                            IsCheck = fieldInfo.IsCheck,
                            IsForeignKey = fieldInfo.IsForeignKey,
                            IsPrimaryKey = fieldInfo.IsPrimaryKey,
                            IsUniqueKey = fieldInfo.IsUniqueKey,
                            NumericPrecision = fieldInfo.Numeric_Precision,
                            OrdinalPosition = fieldInfo.Ordinal_Position,
                            TableName = fieldInfo.Table_Name,
                            Schema = fieldInfo.Table_Schema,
                            UniqueKeyconstraintName = fieldInfo.uk_constraint_name,
                            Database = ""
                        });
                }
            }
        }

        public void MapStoredProcedureDataToStoredProcedures(Schema schema, List<StoredProcedureData> storedProcedures)
        {

            foreach (var spName in storedProcedures.Where(sd => schema == null || sd.SchemaName == schema.SchemaName).Select(sd => sd.StoredProcedureName).Distinct().OrderBy(sp => sp))
            {
                var sprocParameters = storedProcedures
                    .Where(spp => spp.ParameterName != null && spp.StoredProcedureName == spName)
                    .OrderBy(spp => spp.ParameterOrder)
                    .ToList();

                var sp = new StoredProcedure
                {
                    Schema = schema,
                    StoredProcedureName = spName,
                    IsSqlClr = sprocParameters.Any() ? sprocParameters[0].IsSqlClr : 0,

                };

                schema.StoredProcedures.Add(sp);

                foreach (var spParameter in sprocParameters)
                {
                    sp.Parameters.Add(
                        new StoredProcedureParameter
                        {
                            ParameterName = spParameter.ParameterName,
                            DataType = spParameter.DataType,
                            Length = spParameter.Length,
                            ParameterDirection = spParameter.ParameterDirection,
                            ParameterOrder = spParameter.ParameterOrder
                        }
                    );
                }

            }
        }
    }
}