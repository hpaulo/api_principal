using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Data;
using Microsoft.Reporting.WebForms;

namespace api.Negocios.Util
{
    public class GatewayExportar
    {
        /// <summary>
        /// Converte uma array de objetos em CSV e retorna o array de BYTES
        /// </summary>
        /// <param name="Collection">Coleção de dados a ser Convertida</param>
        /// <returns></returns>
        public static byte[] CSV(dynamic[] Collection)
        {
            return Bibliotecas.Converter.ListToCSV(Collection);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSource">Nome do Data Source Definido no Template</param>
        /// <param name="Collection">Coleção de dados a ser renderizada</param>
        /// <param name="TemplateReportPath">Caminho para o Template do Relatório</param>
        /// <param name="TipoArquivo">Excel, Word, Pdf</param>
        /// <returns></returns>
        public static byte[] Relatorio(string DataSource, IEnumerable<dynamic> Collection, string TemplateReportPath, String TipoArquivo = "Excel")
        {
            ReportViewer ReportViewer1 = new ReportViewer();
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath = TemplateReportPath;
            DataTable dt = new DataTable();
            ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(DataSource, Collection));

            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            return ReportViewer1.LocalReport.Render(TipoArquivo, null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }

        public static Byte[] Excel(DataSet dataSet)//List<IList<object>> ListDados)
        {
            //DataSet dataSet = Bibliotecas.Converter.ConvertToDataSet(ListDados);

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

        }



        public static MemoryStream Create(Dictionary<String, List<OpenXmlElement>> sets)
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

        private static void ExportDSToExcel(DataSet ds, string destination)
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