using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using NFe_Util_2G;
using System.IO.Compression;


namespace api.Negocios.Util
{
    public class GatewayUtilNfe
    {
        public static byte[] ArquivoXml(string xmlNFe)
        {
            XmlDocument xm = new XmlDocument();
            xm.LoadXml(xmlNFe);
            byte[] bytes = Encoding.Default.GetBytes(xm.OuterXml);
            return bytes; //return File(bytes, "application/force-download", id + ".xml");
        }

        public static byte[] DownloadZipXmls(List<dynamic> lista)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                {
                    int i = 0;
                    byte[] bytes;
                    foreach (var item in lista)
                    {
                        //Create a zip entry for each attachment
                        var zipEntry = zipArchive.CreateEntry(item.nrChave + ".xml");
                        i++;

                        bytes = ArquivoXml(item.xmlNFe);

                        //Get the stream of the attachment
                        using (var originalFileStream = new MemoryStream(bytes))
                        {
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                //Copy the attachment stream to the zip entry stream
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
                return compressedFileStream.ToArray();
            }
        }


        public static byte[] DownloadZipCSVs(List<List<string>> lista, List<string> nomesArquivo = null)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                {
                    int i = 0;
                    byte[] bytes;
                    foreach (List<string> lines in lista)
                    {
                        //Create a zip entry for each attachment
                        var zipEntry = zipArchive.CreateEntry((nomesArquivo != null && i < nomesArquivo.Count ? nomesArquivo[i] + "_" : "") + i + ".csv");
                        i++;

                        bytes = Bibliotecas.Converter.ListToCSV(lines);

                        //Get the stream of the attachment
                        using (var originalFileStream = new MemoryStream(bytes))
                        {
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                //Copy the attachment stream to the zip entry stream
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
                return compressedFileStream.ToArray();
            }
        }

        public static byte[] PDFDanfe(string xmlNFe, string nrChave)
        {
            
            string path = HttpContext.Current.Server.MapPath("~/App_Data/PDF/");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = path + nrChave + ".pdf";

            NFe_Util_2G.Util nfeImp = new NFe_Util_2G.Util();
            string retorno;
            nfeImp.geraPdfDANFE(
               xmlNFe,
                "http://www.atoscapital.com.br/portal/img/AtosCapital.jpg", //"C:/Users/Elton%20Nunes/Downloads/Guia_NFe_Util_2Gv1.4/xhtml_single/media/LogFD.jpg", //Logo
                "S", //[S]Sim, [N]Não
                "S", //[S]Sim, [N]Não
                "S", //[S]Sim, [N]Não
                "",
                "b", //[T]racejado, [L]inha, [B]ranco, [Z]ebrado, [t]racejado, [l]inha, [b]ranco ou [z]ebrado
                path, out retorno);

            byte[] bytes = System.IO.File.ReadAllBytes(path);
            System.IO.File.Delete(path);
            return bytes;
        }

        public static byte[] DownloadZipPdfs(List<dynamic> lista)
        {
            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                {
                    int i = 0;
                    byte[] bytes;
                    foreach (var item in lista)
                    {
                        //Create a zip entry for each attachment
                        var zipEntry = zipArchive.CreateEntry(item.nrChave + ".pdf");
                        i++;

                        bytes = PDFDanfe(item.xmlNFe, item.nrChave);

                        //Get the stream of the attachment
                        using (var originalFileStream = new MemoryStream(bytes))
                        {
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                //Copy the attachment stream to the zip entry stream
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
                return compressedFileStream.ToArray();
            }
        }

    }
}