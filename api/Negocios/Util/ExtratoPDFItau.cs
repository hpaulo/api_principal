using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using OFXSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace api.Negocios.Util
{
    public class ExtratoPDFItau
    {
        public static OFXDocument Import(string pathFilename)
        {

            OFXDocument document = new OFXDocument();
            document.Account = new Account();
            document.Account.BankID = "341";
            document.Transactions = new List<Transaction>();
            document.SignOn = new SignOn();

            string strText = string.Empty;

            string anoFixo = String.Empty;
            PdfReader reader = new PdfReader(pathFilename);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
                string cipherText = PdfTextExtractor.GetTextFromPage(reader, page, its);
                cipherText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(cipherText)));
                strText = strText + "\n" + cipherText;
            }
            int maxLength = Convert.ToString(reader.NumberOfPages).Length;
            reader.Close();

            string[] temp = strText.Split('\n');
            bool head = false;
            Transaction transaction = new Transaction();

            for (int i = 0; i < temp.Length; i++)
            {
                string value = string.Empty;

                try
                {
                    if (!head && temp[i].Contains("Nome") && temp[i].Contains("Agência/Conta"))
                    {
                        value = temp[i].ToString().Trim().Substring(temp[i].IndexOf("Agência/Conta:") + 14);
                        document.Account.BranchID = value.ToString().Trim().Substring(0, value.IndexOf("/") - 1);
                        document.Account.AccountID = value.ToString().Trim().Substring(value.IndexOf("/"));
                    }
                    else if (!head && temp[i].Contains("Data") && temp[i].Contains("Horário"))
                    {
                        value = temp[i].ToString().Trim();
                        //DateTime.ParseExact(data.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        string auxDt = value.ToString().Trim().Substring(temp[i].IndexOf("Data") + 5, 12).Trim();
                        string auxHora = value.ToString().Trim().Substring(temp[i].IndexOf("Horário:") + 9).Trim();
                        try
                        {
                            document.SignOn.DTServer = DateTime.ParseExact(auxDt + " " + auxHora + ".000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                        catch { }
                    }
                    else if (!head && temp[i].Contains("Extrato de") && temp[i].Contains("até"))
                    {
                        value = temp[i].ToString().Trim();
                        string auxInicio = value.ToString().Trim().Substring(temp[i].IndexOf("Extrato de") + 10).Trim().Substring(0, 10) + " 00:00:00.000";
                        try
                        {
                            document.StatementStart = DateTime.ParseExact(auxInicio, "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            throw new Exception("'" + auxInicio + "' não corresponde a uma data válida (1)");
                        }
                        anoFixo = document.StatementStart.ToString("yyyy");
                        string auxFinal = value.ToString().Trim().Substring(value.Length - 10).Trim() + " 00:00:00.000";
                        try
                        {
                            document.StatementEnd = DateTime.ParseExact(auxFinal, "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            throw new Exception("'" + auxFinal + "' não corresponde a uma data válida (2)");
                        }
                    }
                    else if (!head && temp[i].Contains("Data") && temp[i].Contains("Lançamento") && temp[i].Contains("Ag./Origem")
                        && temp[i].Contains("Valor") && temp[i].Contains("Saldo") && temp[i].Contains("(R$)"))
                    {
                        head = true;
                    }
                    else if (head && !temp[i].Contains("ItaúEmpresas") && !(temp[i] == "") && !(temp[i].Trim().Length <= maxLength) && !(temp[i].ToString().Trim().Contains("SALDO")))
                    {
                        value = temp[i].ToString().Trim();
 
                        //Guardando data da linha
                        //string dsds = temp[i].Trim();
                        string aux = value.ToString().Trim().Substring(0, 5).Trim() + "/" + document.StatementStart.ToString("yyyy") + " 00:00:00.000";

                        try
                        {
                            transaction.Date = DateTime.ParseExact(aux, "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            throw new Exception("'" + aux + "' não corresponde a uma data válida (3)");
                        }

                        value = value.ToString().Trim().Substring(5).Trim();
                        //List<string> colunas2 = value.Split(' ').ToList();
                        string[] colunas = value.Split(' ');


                        //Guardando valor da linha
                        if (temp[i + 1].Contains("SALDO FINAL"))
                        {
                            for (int j = colunas.Length - 2; j >= 0; j--)
                            {
                                if (colunas[j] != "")
                                {
                                    try
                                    {
                                        transaction.Amount = Convert.ToDecimal(colunas[j]);
                                    }
                                    catch
                                    {
                                        throw new Exception("'" + colunas[j] + "' não corresponde a um número válido (1)");
                                    }
                                    value = value.Trim().Substring(0, value.IndexOf(colunas[j])).Trim();
                                    break;
                                }
                            }
                        }
                        else if (temp[i + 2].Contains("(-) SALDO A LIBERAR") || (temp[i + 1].Contains("SALDO PARCIAL") && temp[i + 4].Contains("(-) SALDO A LIBERAR")) || (temp[i + 2].Contains("ItaúEmpresas") && temp[i + 5].Contains("(-) SALDO A LIBERAR")) ||
                            (temp[i + 3].Contains("ItaúEmpresas") && temp[i + 5].Contains("(-) SALDO A LIBERAR")) || (temp[i + 2].Contains("ItaúEmpresas") && temp[i + 4].Contains("SALDO PARCIAL") && temp[i + 7].Contains("(-) SALDO A LIBERAR")) ||
                            (temp[i + 1].Contains("SALDO PARCIAL") && temp[i + 3].Contains("ItaúEmpresas") && temp[i + 7].Contains("(-) SALDO A LIBERAR")) || (temp[i + 1].Contains("SALDO PARCIAL") && temp[i + 4].Contains("ItaúEmpresas") && temp[i + 7].Contains("(-) SALDO A LIBERAR")) ||
                            (temp[i + 1].Contains("SALDO PARCIAL") && temp[i + 5].Contains("ItaúEmpresas") && temp[i + 7].Contains("(-) SALDO A LIBERAR")))
                        {
                            for (int j = colunas.Length - 2; j >= 0; j--)
                            {
                                if (colunas[j] != "")
                                {
                                    try
                                    {
                                        transaction.Amount = Convert.ToDecimal(colunas[j]);
                                    }
                                    catch
                                    {
                                        throw new Exception("'" + colunas[j] + "' não corresponde a um número válido (2)");
                                    }
                                    value = value.Trim().Substring(0, value.IndexOf(colunas[j])).Trim();
                                    break;
                                }
                            }
                        }
                        else if (temp[i + 1] != "" && !(temp[i + 1].Trim().Length <= maxLength) && ((temp[i].Substring(0, 5).Trim() != temp[i + 1].Substring(0, 5).Trim()) || (temp[i + 2] == "ItaúEmpresas" && temp[i].Substring(0, 5).Trim() != temp[i + 4].Substring(0, 5).Trim()) ||
                            (temp[i + 1].Contains("SALDO PARCIAL") && temp[i].Substring(0, 5).Trim() != temp[i + 3].Substring(0, 5).Trim()) || (temp[i + 2] == "ItaúEmpresas" && temp[i + 4].Contains("SALDO PARCIAL") && temp[i].Substring(0, 5).Trim() != temp[i + 6].Substring(0, 5).Trim()) ||
                            (temp[i + 1].Contains("SALDO PARCIAL") && temp[i + 3] == "ItaúEmpresas" && temp[i].Substring(0, 5).Trim() != temp[i + 6].Substring(0, 5).Trim()) || (temp[i + 1].Contains("SALDO PARCIAL") && temp[i + 4] == "ItaúEmpresas" && temp[i].Substring(0, 5).Trim() != temp[i + 6].Substring(0, 5).Trim())))
                        {
                            for (int j = colunas.Length - 2; j >= 0; j--)
                            {
                                if (colunas[j] != "")
                                {
                                    try
                                    {
                                        transaction.Amount = Convert.ToDecimal(colunas[j]);
                                    }
                                    catch
                                    {
                                        throw new Exception("'" + colunas[j] + "' não corresponde a um número válido (3)");
                                    }
                                    value = value.Trim().Substring(0, value.IndexOf(colunas[j])).Trim();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                transaction.Amount = Convert.ToDecimal(colunas[colunas.Length - 1]);
                            }
                            catch
                            {
                                throw new Exception("'" + colunas[colunas.Length - 1] + "' não corresponde a um número válido (4)");
                            }
                            value = value.Replace(colunas[colunas.Length - 1], "").Trim();
                        }

                        // Sem número do documento
                        transaction.CheckNum = String.Empty;

                        //Guardando descrição
                        transaction.Memo = value;

                        //Guardando tipo
                        transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                        // Adiciona a tranction
                        document.Transactions.Add(transaction);
                        transaction = new Transaction();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(i.ToString() + " -> " + e.Message);
                }
            }

            return document;
        }
    }
}