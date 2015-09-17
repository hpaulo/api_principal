using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.Globalization;
using OFXSharp;
using System.Xml;

namespace api.Controllers.Util
{
    public class ExtratoPDFSantanderController : ApiController
    {

        /*public class DadosSantander
        {
            public string cdBanco { get; set; }
            public string nrAgencia { get; set; }
            public string nrConta { get; set; }
            public List<LinhaExtrato> linhasExtrato { get; set; }

        }

        public class LinhaExtrato
        {
            public string nrDocumento { get; set; }
            public System.DateTime dtExtrato { get; set; }
            public string dsDocumento { get; set; }
            public decimal vlMovimento { get; set; }
            public string dsTipo { get; set; }
            public string row { get; set; }
        }*/


        public static OFXDocument Import(string pathFilename)
        {
            //DadosSantander dados = new DadosSantander();
            //dados.cdBanco = "033";

            //List<LinhaExtrato> list = new List<LinhaExtrato>();
            OFXDocument document = new OFXDocument();
            document.Account = new Account();
            document.Account.BankID = "033";
            document.Transactions = new List<Transaction>();

            string strText = string.Empty;
            string[] PdfData = null;
            bool tratamento = false;
            int rowsTratamento = 0;
            string stringTratamento1 = String.Empty;
            string stringTratamento2 = String.Empty;
            string stringTratamento3 = String.Empty;
            int iof = 0;

            /*try
            {*/
            PdfReader reader = new PdfReader((string)pathFilename);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
                String cipherText = PdfTextExtractor.GetTextFromPage(reader, page, its);
                cipherText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(cipherText)));
                strText = strText + "\n" + cipherText;
                PdfData = strText.Split('\n');
            }
            reader.Close();
            /*}
            catch (Exception ex)
            {
            }*/


            List<string> temp = PdfData.ToList();

            bool head = false;
            for (int i = 0; i < temp.Count; i++)
            {
                if(i == 201)
                    Console.WriteLine("Line: " + i + " Total: " + temp.Count + " | " + temp[i].ToString());
                string value = String.Empty;

                switch (i)
                {
                    case 1: // [Agência e Conta Corrente]
                        value = temp[i].ToString().Substring((temp[i].ToString().IndexOf("Agência:") + 8), 6);
                        //dados.nrAgencia = value.Trim().TrimStart().TrimEnd();
                        document.Account.BranchID = value.Trim().TrimStart().TrimEnd();

                        value = temp[i].ToString().Substring((temp[i].ToString().IndexOf("Corrente:") + 9), 12);
                        //dados.nrConta = value.Trim().TrimStart().TrimEnd();
                        document.Account.AccountID = value.Trim().TrimStart().TrimEnd();
                        // Avalia se tem o código da operação antes
                        if (document.Account.AccountID.IndexOf("-") != document.Account.AccountID.LastIndexOf("-"))
                            document.Account.AccountID = document.Account.AccountID.Substring(document.Account.AccountID.IndexOf("-") + 1);


                        break;
                    case 3: // Período do extrato
                        if (temp[i].ToString().Contains("Período:"))
                        {
                            int index = temp[i].ToString().IndexOf("Período:") + "Periodo:".Length;
                            string periodo = temp[i].ToString().Substring(index);
                            if (periodo.Contains("Data/Hora"))
                                periodo = periodo.Substring(0, periodo.IndexOf("Data/Hora"));

                            index = periodo.IndexOf(" a ");
                            string dtInicio = periodo.Substring(0, index).Trim();
                            string dtFim = periodo.Substring(index + 3).Trim();
                            document.StatementStart = DateTime.ParseExact(dtInicio + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            document.StatementEnd = DateTime.ParseExact(dtFim + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                        break;
                    case 4: // [ Início do Extrato]
                        if (temp[i].ToString().Contains("Data Histórico Docto. Valor R$ Saldo R$"))
                            head = true;
                        break;
                    default: // [ Linhas do Extrato]
                        if (head && temp[i].ToString().IndexOf("SALDO") < 0) //[ NÃO LEVA EM CONSIDERAÇÂO SALDOS ]
                        {

                            //LinhaExtrato row = new LinhaExtrato();

                            String cipherText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(temp[i].ToString())));

                            if (
                                cipherText.IndexOf("Posição em:") < 0 &&
                                cipherText.IndexOf("Provisão Encargos") < 0 &&
                                cipherText.IndexOf("Limite Santander Master") < 0 &&
                                cipherText.IndexOf("a = Bloqueio Dia") < 0 &&
                                cipherText.IndexOf("b = Bloqueado") < 0 &&
                                cipherText.IndexOf("p = Lançamento Provisionado") < 0 &&
                                cipherText.IndexOf("Superlinha") < 0 &&
                                cipherText.IndexOf("Saldo Bloqueado") < 0 &&
                                cipherText.IndexOf("Saldo Bloqueio Dia") < 0 &&
                                cipherText.IndexOf("Saldo Total de Conta Corrente") < 0 &&
                                cipherText.IndexOf("Saldo de Conta Corrente") < 0 &&
                                cipherText.IndexOf("Saldo em Investimentos com Resgate") < 0 &&
                                cipherText.IndexOf("Saldo Disponível (") < 0 &&
                                cipherText.IndexOf("Saldo Disponível Conta Corrente") < 0 &&
                                cipherText.IndexOf("Saldo Disponível Total") < 0 &&
                                cipherText.IndexOf("Ouvidoria 0800") < 0
                                )
                            {

                                if (cipherText.IndexOf("IOF") < 0 && iof == 0)
                                {
                                    if (cipherText.Contains("/") && !tratamento)
                                    {
                                        //row.row = cipherText;

                                        string[] verify = cipherText.Split(',');
                                        if (verify.Length > 2)
                                            cipherText = cipherText.Replace(cipherText.Substring((cipherText.IndexOf(',') + 3), cipherText.Length - (cipherText.IndexOf(',') + 3)), "");

                                        string[] verify2 = cipherText.Split('/');
                                        if (verify2.Length > 4)
                                        {
                                            cipherText = cipherText.Replace(cipherText.Substring((cipherText.IndexOf(',') + 3), cipherText.Length - (cipherText.IndexOf(',') + 3)), "");
                                            string StringTemp = verify2[2].Substring(5, verify2[2].Length - 8);
                                            string StringReplace = cipherText.Substring(cipherText.IndexOf(StringTemp) + StringTemp.Length, 11);
                                            cipherText = cipherText.Replace(StringReplace, "");
                                        }

                                        Transaction transaction = new Transaction();

                                        //row.dtExtrato = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        transaction.Date = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);

                                        string nrDocumento = Regex.Match(cipherText.Substring(10, cipherText.Length - 10), @"\d+").Value;
                                        if (nrDocumento.Length > 6)
                                            nrDocumento = Regex.Match(cipherText.Replace(nrDocumento, "").Substring(10, cipherText.Replace(nrDocumento, "").Length - 10), @"\d+").Value;
                                        //row.nrDocumento = nrDocumento;
                                        transaction.CheckNum = nrDocumento;

                                        string val = (cipherText.Substring(cipherText.IndexOf(nrDocumento) + nrDocumento.Length, (cipherText.Length - (cipherText.IndexOf(nrDocumento) + nrDocumento.Length)))).Trim().TrimStart().TrimEnd().Replace(" ", "");
                                        //row.vlMovimento = Convert.ToDecimal(val);
                                        transaction.Amount = Convert.ToDecimal(val);
                                        //row.dsDocumento = cipherText.Substring(11, cipherText.IndexOf(row.nrDocumento) - 11);
                                        transaction.Memo = cipherText.Substring(11, cipherText.IndexOf(nrDocumento) - 11);
                                        //row.dsTipo = row.vlMovimento > 0 ? "CREDIT" : "DEBIT";
                                        transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                                        // Adiciona a transaction
                                        document.Transactions.Add(transaction);
                                        //list.Add(row);
                                        //row = new LinhaExtrato();
                                    }
                                    else
                                    {
                                        tratamento = true;
                                        rowsTratamento++;

                                        if (rowsTratamento == 1)// && tratamento)
                                            stringTratamento1 = cipherText.Replace('/', '|');
                                        else if (rowsTratamento == 2)// && tratamento)
                                            stringTratamento2 = cipherText;
                                        else if (rowsTratamento == 3)// && tratamento)
                                        {
                                            if (stringTratamento1.Length > 10)
                                            {
                                                stringTratamento3 = cipherText.Replace('/', '|');
                                                cipherText = stringTratamento2.Substring(0, 11) + stringTratamento1 + " " + stringTratamento3 + " " + stringTratamento2.Substring(11, stringTratamento2.Length - 11);
                                                //row.row = cipherText;
                                                string filter = String.Empty;
                                                filter = cipherText.Replace(stringTratamento3, " ");

                                                string[] verify = filter.Split(',');
                                                if (verify.Length > 2)
                                                    filter = cipherText.Replace(filter.Substring((filter.IndexOf(',') + 3), filter.Length - (filter.IndexOf(',') + 3)), "");

                                                string[] verify2 = filter.Split('/');
                                                if (verify2.Length > 4)
                                                {
                                                    filter = filter.Replace(filter.Substring((filter.IndexOf(',') + 3), filter.Length - (filter.IndexOf(',') + 3)), "");
                                                    string StringTemp = verify2[2].Substring(5, verify2[2].Length - 8);
                                                    string StringReplace = filter.Substring(filter.IndexOf(StringTemp) + StringTemp.Length, 11);
                                                    filter = filter.Replace(StringReplace, "");
                                                }

                                                Transaction transaction = new Transaction();

                                                //row.dtExtrato = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                                transaction.Date = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                                //row.nrDocumento = Regex.Match(filter.Substring(10, filter.Length - 10), @"\d+").Value;
                                                transaction.CheckNum = Regex.Match(filter.Substring(10, filter.Length - 10), @"\d+").Value;
                                                //row.vlMovimento = Convert.ToDecimal(filter.Substring(filter.IndexOf(row.nrDocumento) + row.nrDocumento.Length, (filter.Length - (filter.IndexOf(row.nrDocumento) + row.nrDocumento.Length))).Trim().TrimStart().TrimEnd());
                                                transaction.Amount = Convert.ToDecimal(filter.Substring(filter.IndexOf(transaction.CheckNum) + transaction.CheckNum.Length, (filter.Length - (filter.IndexOf(transaction.CheckNum) + transaction.CheckNum.Length))).Trim().TrimStart().TrimEnd());
                                                //row.dsDocumento = cipherText.Substring(11, cipherText.IndexOf(row.nrDocumento) - 11);
                                                transaction.Memo = cipherText.Substring(11, cipherText.IndexOf(transaction.CheckNum) - 11);
                                                //row.dsTipo = row.vlMovimento > 0 ? "CREDIT" : "DEBIT";
                                                transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                                                // Adiciona a transaction
                                                document.Transactions.Add(transaction);
                                                //list.Add(row);
                                                //row = new LinhaExtrato();

                                                rowsTratamento = 0;
                                                tratamento = false;
                                            }
                                            else
                                            {
                                                rowsTratamento = 0;
                                                tratamento = false;
                                            }
                                        }
                                    }
                                    //}
                                }
                                else
                                {
                                    if (iof == 0)
                                    {
                                        iof++;
                                        if (cipherText.IndexOf('/') > 0)
                                        {
                                            string[] tempFilter = cipherText.Split(':');
                                            stringTratamento1 = tempFilter[0];
                                        }
                                        else
                                            stringTratamento1 = cipherText;
                                    }
                                    else if (iof == 1)
                                    {
                                        iof++;
                                        stringTratamento2 = cipherText;
                                    }
                                    else if (iof == 2)
                                    {
                                        iof = 0;
                                        stringTratamento3 = cipherText.Replace('/', '-');
                                        cipherText = stringTratamento2.Substring(0, 11) + stringTratamento1 + " " + stringTratamento3 + " " + stringTratamento2.Substring(11, stringTratamento2.Length - 11);
                                        //row.row = cipherText;
                                        string filter = String.Empty;
                                        filter = cipherText.Replace(stringTratamento3, " ");

                                        Transaction transaction = new Transaction();

                                        //row.dtExtrato = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        transaction.Date = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        //row.nrDocumento = Regex.Match(filter.Substring(10, filter.Length - 10), @"\d+").Value;
                                        transaction.CheckNum = Regex.Match(filter.Substring(10, filter.Length - 10), @"\d+").Value;
                                        //row.vlMovimento = Convert.ToDecimal(filter.Substring(filter.IndexOf(row.nrDocumento) + row.nrDocumento.Length, (filter.Length - (filter.IndexOf(row.nrDocumento) + row.nrDocumento.Length))).Trim().TrimStart().TrimEnd());
                                        transaction.Amount = Convert.ToDecimal(filter.Substring(filter.IndexOf(transaction.CheckNum) + transaction.CheckNum.Length, (filter.Length - (filter.IndexOf(transaction.CheckNum) + transaction.CheckNum.Length))).Trim().TrimStart().TrimEnd());
                                        //row.dsDocumento = cipherText.Substring(11, cipherText.IndexOf(row.nrDocumento) - 11);
                                        transaction.Memo = cipherText.Substring(11, cipherText.IndexOf(transaction.CheckNum) - 11);
                                        //row.dsTipo = row.vlMovimento > 0 ? "CREDIT" : "DEBIT";
                                        transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                                        // Adiciona a transaction
                                        document.Transactions.Add(transaction);
                                        //list.Add(row);
                                        //row = new LinhaExtrato();
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            //return list;
            return document;
        }

    }
}
