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


    }
}