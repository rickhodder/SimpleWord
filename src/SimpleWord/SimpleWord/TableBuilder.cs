using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using BottomBorder = DocumentFormat.OpenXml.Wordprocessing.BottomBorder;
using InsideHorizontalBorder = DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder;
using InsideVerticalBorder = DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder;
using LeftBorder = DocumentFormat.OpenXml.Wordprocessing.LeftBorder;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using RightBorder = DocumentFormat.OpenXml.Wordprocessing.RightBorder;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableCellProperties = DocumentFormat.OpenXml.Wordprocessing.TableCellProperties;
using TableProperties = DocumentFormat.OpenXml.Wordprocessing.TableProperties;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TopBorder = DocumentFormat.OpenXml.Wordprocessing.TopBorder;

namespace SimpleWord
{
    public class TableBuilder<TDataClass>
    {
        readonly TableDefinition<TDataClass> _td;
        ColorScheme _colorScheme;
        SimpleWordDocument _document;

        public TableBuilder(SimpleWordDocument document, TableDefinition<TDataClass> tableDefinition, ColorScheme scheme)
        {
            _document = document;
            _colorScheme = scheme;
            _td = tableDefinition;
        }

        public SimpleWordTable Build(IGrouping<string, TDataClass> group)
        {
            var table = CreateTable();

            //Headers
            table.Append(CreateHeaderRow());

            foreach (var row in group)
            {
                table.Append(CreateRow(row));
            }

            return new SimpleWordTable(table);
        }

        public SimpleWordTable Build(List<TDataClass> group)
        {

            var table = CreateTable();

            //Headers
            table.Append(CreateHeaderRow());

            foreach (var row in group)
            {
                table.Append(CreateRow(row));
            }

            return new SimpleWordTable(table);
        }
        private TableCellWidth GetCellWidth(ColumnDefinition<TDataClass> column)
        {
            TableWidthUnitValues units = TableWidthUnitValues.Auto;
            var size = 0;

            var result = new TableCellWidth();
            if (column.WidthUnit == ColumnWidthUnitType.Points)
            {
                units = TableWidthUnitValues.Dxa;
                size = column.Width * 20;
            }
            else
            {
                units = TableWidthUnitValues.Pct;
                size = column.Width;
            }

            return new TableCellWidth { Type = units, Width = size.ToString() };
        }

        private TableRow CreateHeaderRow()
        {
            var tr = new TableRow();

            foreach (var column in _td.Columns)
            {
                // Create a cell.
                TableCell tc1 = new TableCell();

                var tcp = CreateTableCellProperties(column);

                tc1.Append(tcp);

                // Specify the table cell content.
                tc1.Append(CreateParagraph(column.HeaderText, column.HeaderForegroundColor));

                tr.Append(tc1);
            }

            return tr;
        }

        public Paragraph CreateParagraph(string text, System.Drawing.Color foreColor)
        {
            return new Paragraph(CreateRun(text, foreColor));
        }

        public Run CreateRun(string text, System.Drawing.Color foreColor)
        {
            var run = new Run(CreateText(text));
            var rp = new RunProperties(new Color { Val = HexConverter(foreColor) });
            run.RunProperties = rp;
            return run;
        }

        public Text CreateText(string text)
        {
            return new Text(text);
        }

        public TableCellProperties CreateTableCellProperties(ColumnDefinition<TDataClass> column)
        {
            return new TableCellProperties(
                GetCellWidth(column),
                new TableCellMargin(new TableCellLeftMargin { Type = TableWidthValues.Dxa, Width = 100 },
                    new TableCellRightMargin { Type = TableWidthValues.Dxa, Width = 100 }
                ),
                new Shading()
                {
                    Color = "auto",
                    Fill = HexConverter(column.HeaderBackgroundColor),
                    Val = ShadingPatternValues.Clear
                }
            );
        }


        public TableRow CreateRow(TDataClass data)
        {
            var tr = new TableRow();

            foreach (var column in _td.Columns)
            {
                // Create a cell.
                TableCell tc1 = new TableCell();

                // Format the cell
                tc1.Append(CreateTableCellProperties(data, column));

                // Specify the table cell content.
                tc1.Append(CreateParagraph(column.Contents(data), column.ForegroundColor(data)));

                tr.Append(tc1);
            }

            return tr;
        }

        public TableCellProperties CreateTableCellProperties(TDataClass data, ColumnDefinition<TDataClass> column)
        {
            return new TableCellProperties(
                GetCellWidth(column),
                new TableCellMargin(
                    new TableCellLeftMargin { Type = TableWidthValues.Dxa, Width = 100 },
                    new TableCellRightMargin { Type = TableWidthValues.Dxa, Width = 100 }
                ),
                new Shading()
                {
                    Color = "auto",
                    Fill = HexConverter(column.BackgroundColor(data)),
                    Val = ShadingPatternValues.Clear
                }
            );
        }

        private Table CreateTable()
        {
            Table table = new Table();
            StyleTable(table);
            return table;
        }

        private BorderValues MapBorderType(TableDefinitionBorderType borderType)
        {
            switch (borderType)
            {
                case TableDefinitionBorderType.Dashed:
                    return BorderValues.Dashed;

                case TableDefinitionBorderType.Dotted:
                    return BorderValues.Dotted;

                case TableDefinitionBorderType.Double:
                    return BorderValues.Single;

                default:
                    return BorderValues.Single;
            }
        }

        public void StyleTable(Table table)
        {
            UInt32Value borderWidth = Convert.ToUInt32(_td.BorderWidth);
            var border = MapBorderType(_td.BorderType);

            // Create a TableProperties object and specify its border information.
            TableProperties tblProp = new TableProperties(
                new TableBorders(
                    new TopBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(border),
                        Size = borderWidth,
                    },
                    new BottomBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(border),
                        Size = borderWidth,

                    },
                    new LeftBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(border),
                        Size = borderWidth,

                    },
                    new RightBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(border),
                        Size = borderWidth
                    },
                    new InsideHorizontalBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(border),
                        Size = borderWidth,
                    },
                    new InsideVerticalBorder()
                    {
                        Val =
                            new EnumValue<BorderValues>(border),
                        Size = borderWidth,
                    }
                )
            );

            // Append the TableProperties object to the empty table.
            table.AppendChild<TableProperties>(tblProp);
        }

        public static string HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}