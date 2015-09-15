using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Data;

namespace api.Negocios.Util
{
    public class GatewayExportar
    {

        public static Byte[] Excel(List<IList<object>> ListDados)
        {
            DataSet dataSet = Bibliotecas.Converter.ConvertToDataSet(ListDados);

            Dictionary<String, List<OpenXmlElement>> sets = (from dt in dataSet.Tables.OfType<DataTable>()
                     select new
                     {
                         // Sheet Name
                         Key = dt.TableName,
                         Value = (
                         // Sheet Columns
                         new List<OpenXmlElement>(
                            new OpenXmlElement[]
                            {
                    new Row(
                        from d in dt.Columns.OfType<DataColumn>()
                        select (OpenXmlElement)new Cell()
                        {
                            CellValue = new CellValue(d.ColumnName),
                            DataType = CellValues.String
                        })
                            })).Union
                         // Sheet Rows
                         ((from dr in dt.Rows.OfType<DataRow>()
                           select ((OpenXmlElement)new Row(from dc in dr.ItemArray
                                                           select (OpenXmlElement)new Cell()
                                                           {
                                                               CellValue = new CellValue(dc.ToString()),
                                                               DataType = CellValues.String
                                                           })))).ToList()
                     }).ToDictionary(p => p.Key, p => p.Value);

            MemoryStream memoryStream = Create(sets);

            return memoryStream.GetBuffer();

            #region testes
            /*List<OpenXmlElement> elements = new List<OpenXmlElement>();

            for (int i = 0; i < 10; i++)
            {
                Row row = new Row
                (
                    new Cell[]
                    {
                        new Cell()
                        {
                            CellValue = new CellValue(i.ToString()), DataType = CellValues.Number
                        },
                        new Cell()
                        {
                            CellValue = new CellValue("test " + i), DataType = CellValues.String
                        }
                    }
                );
                    elements.Add(row);
            }

            Dictionary<String, List<OpenXmlElement>> sets = new Dictionary<string, List<OpenXmlElement>>();
            sets.Add("Relatório1", elements);
            sets.Add("Relatório2", elements);
            MemoryStream memoryStream = Create(sets);
            return memoryStream.GetBuffer();*/

            /*MemoryStream memoryStream = new MemoryStream();
            using (var workbook = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                uint sheetId = 1;

                foreach (List<dynamic> table in Dados)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table. };
                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                }
                */

            /* MemoryStream mem = new MemoryStream();
             // Open the document for editing.
             using (SpreadsheetDocument document = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
             {
                 // Add a WorkbookPart to the document.
                 WorkbookPart workbookpart = document.AddWorkbookPart();
                 workbookpart.Workbook = new Workbook();

                 // Add a WorksheetPart to the WorkbookPart.
                 WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                 worksheetPart.Worksheet = new Worksheet(new SheetData());

                 // Add Sheets to the Workbook.
                 Sheets sheets = document.WorkbookPart.Workbook.
                     AppendChild<Sheets>(new Sheets());

                 SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                 Row contentRow = new Row( CreateContentRow(id, true, 2 + id);

                 //row start
                 for (int id = 0; id <= 10; id++)
                 {
                     if (id == 0)
                     {
                         Row contentRow = CreateContentRow(id, true, 2 + id);
                         sheetData.AppendChild(contentRow);
                     }
                     else
                     {
                         Row contentRow = CreateContentRow(id, false, 2 + id);
                         sheetData.AppendChild(contentRow);
                     }

                 }

                 // Append a new worksheet and associate it with the workbook.
                 Sheet sheet = new Sheet()
                 {
                     Id = document.WorkbookPart.
                     GetIdOfPart(worksheetPart),
                     SheetId = 1,
                     Name = "mySheet"
                 };
                 sheets.Append(sheet);



                 // Close the document.
                 document.Close();

                 return mem.GetBuffer();
             }*/

            #endregion
        }



        static MemoryStream Create(Dictionary<String, List<OpenXmlElement>> sets)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (SpreadsheetDocument package = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = package.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                Sheets sheets = workbookpart.Workbook.AppendChild(new Sheets());

                foreach (KeyValuePair<String, List<OpenXmlElement>> set in sets)
                {
                    WorksheetPart worksheetpart = workbookpart.AddNewPart<WorksheetPart>();
                    worksheetpart.Worksheet = new Worksheet(new SheetData(set.Value));
                    worksheetpart.Worksheet.Save();

                    Sheet sheet = new Sheet()
                    {
                        Id = workbookpart.GetIdOfPart(worksheetpart),
                        SheetId = (uint)(sheets.Count() + 1),
                        Name = set.Key
                    };
                    sheets.AppendChild(sheet);
                }
                workbookpart.Workbook.Save();

                return memoryStream;
            }
        }

        private void ExportDSToExcel(DataSet ds, string destination)
        {
            using (var workbook = SpreadsheetDocument.Create(destination, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                uint sheetId = 1;

                foreach (DataTable table in ds.Tables)
                {
                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<String> columns = new List<string>();
                    foreach (DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString()); 
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                }
            }
        }
    }
}