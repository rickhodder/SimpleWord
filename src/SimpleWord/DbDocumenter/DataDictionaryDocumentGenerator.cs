﻿using System.Collections.Generic;
using System.Linq;
using SimpleWord;

namespace DbDocumenter
{
    public class DataDictionaryDocumentGenerator : AbstractDocumentGenerator
    {
        TableDefinition<TableField> _fieldListTableDefinition;
        TableDefinition<StoredProcedureParameter> _storedProcedureParameterTableDefinition;

        public override void PerformGeneration()
        {
            CreateTableDefinitions();

            var dataDictionary = new DataDictionaryData();

            var databaseSchema = dataDictionary.GetDataSchema();

            var data = dataDictionary.Get();

            AddDocumentHeader(data.First().Table_Catalog);

            AddPageBreak();
            AddText("Create table of contents here - headings have been added to the following, so that this will generate a table of contents");
            AddPageBreak();

            AddOverviewHeader(data.First().Table_Catalog);

            foreach (var schema in databaseSchema.Schemas)
            {
                AddSchemaHeader(schema.SchemaName);

                foreach (var table in schema.Tables)
                {
                    if (table.Fields.Any())
                    {
                        AddFieldListTableHeader(table);

                        Document.AddTable(CreateFieldListTable(table));
                    }
                    else
                    {
                        AddText("This table has no fields");
                    }

                    AddPageBreak();
                }

                foreach (var sproc in schema.StoredProcedures)
                {
                    AddStoredProcTableHeader(sproc);
                    if (sproc.Parameters.Any())
                    {
                        AddTable(CreateStoredProcParameterTable(sproc.Parameters));
                    }
                    else
                    {
                        AddText("This stored procedure has no parameters");
                    }

                    AddBlankLine();
                }
            }
        }

        private void AddOverviewHeader(string databaseName)
        {
            AddText("Overview", "Heading2");
            AddText("Overview of this database", "Heading4");
            AddText($"What follows is a breakdown of the {databaseName} database, by schema. Each schema will include a list of the tables and stored procedures that it contains. The tables will show the fields in the tables. The stored procedures will show the parameters of the procedures, if any exist.");
        }


        private void AddStoredProcTableHeader(StoredProcedure proc)
        {
            AddText($"Stored Procedure: {proc.Schema.SchemaName}.{proc.StoredProcedureName}", "Heading3");
            AddText($"Description of {proc.Schema.SchemaName}.{proc.StoredProcedureName}", "Heading4");
            AddBlankLine();
        }

        private SimpleWordTable CreateStoredProcParameterTable(List<StoredProcedureParameter> group)
        {
            var tb = CreateTableBuilder(_storedProcedureParameterTableDefinition, Request.ColorScheme);

            var table = tb.Build(group);

            return table;
        }

        private void CreateTableDefinitions()
        {
            _fieldListTableDefinition = new TableDefinition<TableField>(Request.ColorScheme)
            {
                Columns = new List<ColumnDefinition<TableField>> {
                    new ColumnDefinition<TableField> (Request.ColorScheme){
                        HeaderText="PK",
                        Contents= (d) => d.IsPrimaryKey ? "Yes" : "",
                    },
                    new ColumnDefinition<TableField> (Request.ColorScheme) {
                        HeaderText="FK",
                        Contents= (d) => d.IsForeignKey ? "Yes" : ""
                    },
                    new ColumnDefinition<TableField> (Request.ColorScheme) {
                        HeaderText="UK",
                        Contents= (d) => d.IsUniqueKey? "Yes" : ""
                    },
                    new ColumnDefinition<TableField>  (Request.ColorScheme){
                        HeaderText="Column",
                        Contents= (d) => d.ColumnName,
                        Width=200
                    },
                    new ColumnDefinition<TableField>  (Request.ColorScheme){
                        HeaderText="Type",
                        Contents= (d) => d.DataType
                    },
                    new ColumnDefinition<TableField>  (Request.ColorScheme){
                        HeaderText="Size",
                        Contents= (d) => {
                            if(d.CharacterMaximumLength==null && d.NumericPrecision==null) return "";
                            if (d.DataType=="int" ||d.DataType=="decimal" || d.DataType=="bigint") return "";

                            var size=d.CharacterMaximumLength!=null ? d.CharacterMaximumLength.ToString() : d.NumericPrecision.ToString();
                            return size=="-1"? "MAX":size;
                        }
                    },
                    new ColumnDefinition<TableField>  (Request.ColorScheme){
                        HeaderText="Note",
                        Contents= (d) => "",
                        Width=300
                    },
                }
            };

            _storedProcedureParameterTableDefinition = new TableDefinition<StoredProcedureParameter>(Request.ColorScheme)
            {
                Columns = new List<ColumnDefinition<StoredProcedureParameter>> {
                    new ColumnDefinition<StoredProcedureParameter>  (Request.ColorScheme){
                        HeaderText="In/Out",
                        Contents= (d) => d.ParameterDirection,
                        Width=60
                    },
                    new ColumnDefinition<StoredProcedureParameter> (Request.ColorScheme){
                        HeaderText="Parameter Name",
                        Contents= (d) => d.ParameterName,
                        Width=200
                    },
                    new ColumnDefinition<StoredProcedureParameter> (Request.ColorScheme) {
                        HeaderText="Type",
                        Contents= (d) => d.DataType
                    },
                    new ColumnDefinition<StoredProcedureParameter> (Request.ColorScheme) {
                        HeaderText="Size",
                        Contents= (d) => {
                            if(d.Length==null ) return "";
                            if (d.DataType=="int" ||d.DataType=="decimal" || d.DataType=="bigint") return "";

                            var size=d.Length!=null ? d.Length.ToString() : "";
                            return size=="-1"? "MAX":size;

                        }
                    },
                    new ColumnDefinition<StoredProcedureParameter>  (Request.ColorScheme){
                        HeaderText="Note",
                        Contents= (d) => "",
                        Width=300
                    },
                }
            };
        }

        private void AddSchemaHeader(string schemaName)
        {
            AddPageBreak();

            AddText($"Schema: {schemaName}", "Heading2");
            AddText($"Description of schema {schemaName}", "Heading4");
            AddBlankLine();
        }

        private void AddDocumentHeader(string database)
        {
            AddText($"Data Dictionary for Database: {database}", "Heading1");
        }

        private void AddFieldListTableHeader(SchemaTable table)
        {
            AddText($"Table: {table.Schema.SchemaName}.{table.TableName}", "Heading3");
            AddText($"Description of {table.Schema.SchemaName}.{table.TableName}", "Heading4");
            AddBlankLine();
        }

        private SimpleWordTable CreateFieldListTable(SchemaTable schemaTable)
        {
            var tb = CreateTableBuilder(_fieldListTableDefinition, Request.ColorScheme);

            var table = tb.Build(schemaTable.Fields);

            return table;
        }
    }
}