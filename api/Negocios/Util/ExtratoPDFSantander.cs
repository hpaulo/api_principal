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
    public class ExtratoPDFSantander : ApiController
    {

        private static bool iniciaComData(string text)
        {
            if (!text.Contains("/") || text.TrimStart().Length < 10) return false;

            text = text.TrimStart();

            string data = text.TrimStart().Substring(0, 10);

            try
            {
                DateTime.ParseExact(data.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static OFXDocument Import(string pathFilename)
        {

            OFXDocument document = new OFXDocument();
            document.Account = new Account();
            document.Account.BankID = "033";
            document.Transactions = new List<Transaction>();
            document.SignOn = new SignOn();

            string strText = string.Empty;
            bool tratamento = false;
            int rowsTratamento = 0;
            string stringTratamento1 = String.Empty;
            string stringTratamento2 = String.Empty;
            string stringTratamento3 = String.Empty;
            int iof = 0;

            PdfReader reader = new PdfReader((string)pathFilename);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
                String cipherText = PdfTextExtractor.GetTextFromPage(reader, page, its);
                cipherText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(cipherText)));
                strText = strText + "\n" + cipherText;
            }
            reader.Close();


            List<string> temp = strText.Split('\n').ToList();

            int contAjuste = 0;

            bool head = false;
            for (int i = 0; i < temp.Count; i++)
            {
                try
                {
                    //if (i == 21)
                    //    Console.WriteLine("Line: " + i + " Total: " + temp.Count + " | " + temp[i].ToString());
                    string value = String.Empty;

                    if (i == 1)
                    {
                        // [Agência e Conta Corrente]
                        if (!temp[i].ToString().Contains("Agência:")) { i++; contAjuste++; }
                        value = temp[i].ToString().Substring((temp[i].ToString().IndexOf("Agência:") + 8), 6);
                        // Agência
                        document.Account.BranchID = value.Trim().TrimStart().TrimEnd();
                        // Conta
                        value = temp[i].ToString().Substring((temp[i].ToString().IndexOf("Corrente:") + 9), 12);
                        document.Account.AccountID = value.Trim().TrimStart().TrimEnd();
                        // Avalia se tem o código da operação antes
                        if (document.Account.AccountID.IndexOf("-") != document.Account.AccountID.LastIndexOf("-"))
                            document.Account.AccountID = document.Account.AccountID.Substring(document.Account.AccountID.IndexOf("-") + 1);

                    }
                    else if ((i == 3 && contAjuste == 0) || (contAjuste == 1 && i == 4))
                    {
                        // Período do extrato
                        if (temp[i].ToString().Contains("Período:"))
                        {
                            int index = temp[i].ToString().IndexOf("Período:") + "Periodo:".Length;
                            string periodo = temp[i].ToString().Substring(index);
                            if (periodo.Contains("Data/Hora"))
                            {
                                // DTSERVER
                                string dtServer = periodo.Substring(periodo.IndexOf("Data/Hora") + 10);
                                if (dtServer.Contains("às"))
                                    dtServer = dtServer.Substring(0, dtServer.IndexOf("às"));

                                document.SignOn.DTServer = Convert.ToDateTime(dtServer.Trim());

                                // Período
                                periodo = periodo.Substring(0, periodo.IndexOf("Data/Hora"));
                            }

                            index = periodo.IndexOf(" a ");
                            string dtInicio = periodo.Substring(0, index).Trim();
                            string dtFim = periodo.Substring(index + 3).Trim();
                            document.StatementStart = DateTime.ParseExact(dtInicio + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                            document.StatementEnd = DateTime.ParseExact(dtFim + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        }
                    }
                    else if ((i == 4 && contAjuste == 0) || (contAjuste == 1 && i == 5))
                    {
                        // [ Início do Extrato]
                        if (temp[i].ToString().Contains("Data Histórico Docto. Valor R$"))// Saldo R$"))
                            head = true;
                    }
                    else
                    {
                        // [ Linhas do Extrato]
                        if (head && temp[i].ToString().IndexOf("SALDO ANTERIOR") < 0) //[ NÃO LEVA EM CONSIDERAÇÂO SALDOS ]
                        {

                            //LinhaExtrato row = new LinhaExtrato();
                            String cipherText = String.Empty;
                            try
                            {
                                cipherText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(temp[i].ToString())));
                            }
                            catch
                            {
                                cipherText = String.Empty;
                            }

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
                                    if (iniciaComData(cipherText) && !tratamento)
                                    {
                                        Transaction transaction = new Transaction();

                                        try
                                        {
                                            transaction.Date = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        }
                                        catch
                                        {
                                            throw new Exception("'" + cipherText.Substring(0, 10) + "' não corresponde a uma data válida (1)");
                                        }
                                        // Procura valor e nrDocumento
                                        string auxiliar = cipherText.Substring(10).TrimEnd();
                                        int index = auxiliar.LastIndexOf(" ");
                                        decimal amount = new decimal(0.0);
                                        try
                                        {
                                            amount = Convert.ToDecimal(auxiliar.Substring(index + 1));
                                        }
                                        catch
                                        {
                                            throw new Exception("'" + auxiliar.Substring(index + 1) + "' não corresponde a um valor monetário (1)");
                                        }
                                        auxiliar = auxiliar.Substring(0, index).TrimEnd();
                                        index = auxiliar.LastIndexOf(" ");
                                        try
                                        {
                                            Convert.ToInt32(auxiliar.Substring(index + 1));
                                        }
                                        catch
                                        {
                                            amount = Convert.ToDecimal(auxiliar.Substring(index + 1));
                                            auxiliar = auxiliar.Substring(0, index).TrimEnd();
                                            index = auxiliar.LastIndexOf(" ");
                                            Convert.ToInt32(auxiliar.Substring(index + 1));
                                        }
                                        transaction.CheckNum = auxiliar.Substring(index + 1);
                                        transaction.Amount = amount;
                                        transaction.Memo = auxiliar.Substring(0, index).TrimEnd();
                                        // Legenda?
                                        if (transaction.Memo.EndsWith(" a")) // Bloqueio Dia / ADM
                                            transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" a"));
                                        else if (transaction.Memo.EndsWith(" b")) // Bloqueado
                                            transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" b"));
                                        else if (transaction.Memo.EndsWith(" p")) // Lançamento Provisionado
                                            transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" p"));
                                        //row.dsTipo = row.vlMovimento > 0 ? "CREDIT" : "DEBIT";
                                        transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                                        // Adiciona a transaction
                                        document.Transactions.Add(transaction);
                                    }
                                    else if (cipherText.StartsWith("https://www.") || cipherText.StartsWith("Internet Banking Página"))
                                        continue;
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

                                                Transaction transaction = new Transaction();

                                                try
                                                {
                                                    transaction.Date = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                                }
                                                catch
                                                {
                                                    throw new Exception("'" + cipherText.Substring(0, 10) + "' não corresponde a uma data válida (2)");
                                                }

                                                // Procura valor e nrDocumento
                                                string auxiliar = cipherText.Substring(10).TrimEnd();
                                                int index = auxiliar.LastIndexOf(" ");
                                                decimal amount = new decimal(0.0);
                                                try
                                                {
                                                    amount = Convert.ToDecimal(auxiliar.Substring(index + 1));
                                                }
                                                catch
                                                {
                                                    throw new Exception("'" + auxiliar.Substring(index + 1) + "' não corresponde a um valor monetário (2)");
                                                }
                                                auxiliar = auxiliar.Substring(0, index).TrimEnd();
                                                index = auxiliar.LastIndexOf(" ");
                                                try
                                                {
                                                    Convert.ToInt32(auxiliar.Substring(index + 1));
                                                }
                                                catch
                                                {
                                                    amount = Convert.ToDecimal(auxiliar.Substring(index + 1));
                                                    auxiliar = auxiliar.Substring(0, index).TrimEnd();
                                                    index = auxiliar.LastIndexOf(" ");
                                                    Convert.ToInt32(auxiliar.Substring(index + 1));
                                                }
                                                transaction.CheckNum = auxiliar.Substring(index + 1);
                                                transaction.Amount = amount;
                                                transaction.Memo = auxiliar.Substring(0, index).TrimEnd();

                                                // Legenda?
                                                if (transaction.Memo.EndsWith(" a")) // Bloqueio Dia / ADM
                                                    transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" a"));
                                                else if (transaction.Memo.EndsWith(" b")) // Bloqueado
                                                    transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" b"));
                                                else if (transaction.Memo.EndsWith(" p")) // Lançamento Provisionado
                                                    transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" p"));
                                                transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                                                // Adiciona a transaction
                                                document.Transactions.Add(transaction);

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
   
                                        Transaction transaction = new Transaction();

                                        try
                                        {
                                            transaction.Date = DateTime.ParseExact(cipherText.Substring(0, 10) + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                        }
                                        catch
                                        {
                                            throw new Exception("'" + cipherText.Substring(0, 10) + "' não corresponde a uma data válida (3)");
                                        }

                                        // Procura valor e nrDocumento
                                        string auxiliar = cipherText.Substring(10).TrimEnd();
                                        int index = auxiliar.LastIndexOf(" ");
                                        decimal amount = new decimal(0.0);
                                        try
                                        {
                                            amount = Convert.ToDecimal(auxiliar.Substring(index + 1));
                                        }
                                        catch
                                        {
                                            //amount = new decimal(0.0);
                                            throw new Exception("'" + auxiliar.Substring(index + 1) + "' não corresponde a um valor monetário (3)");
                                        }
                                        auxiliar = auxiliar.Substring(0, index).TrimEnd();
                                        index = auxiliar.LastIndexOf(" ");
                                        try
                                        {
                                            Convert.ToInt32(auxiliar.Substring(index + 1));
                                        }
                                        catch
                                        {
                                            amount = Convert.ToDecimal(auxiliar.Substring(index + 1));
                                            auxiliar = auxiliar.Substring(0, index).TrimEnd();
                                            index = auxiliar.LastIndexOf(" ");
                                            Convert.ToInt32(auxiliar.Substring(index + 1));
                                        }
                                        transaction.CheckNum = auxiliar.Substring(index + 1);
                                        transaction.Amount = amount;
                                        transaction.Memo = auxiliar.Substring(0, index).TrimEnd();
                                        // Legenda?
                                        if (transaction.Memo.EndsWith(" a")) // Bloqueio Dia / ADM
                                            transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" a"));
                                        else if (transaction.Memo.EndsWith(" b")) // Bloqueado
                                            transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" b"));
                                        else if (transaction.Memo.EndsWith(" p")) // Lançamento Provisionado
                                            transaction.Memo = transaction.Memo.Substring(0, transaction.Memo.IndexOf(" p"));

                                        transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                                        // Adiciona a transaction
                                        document.Transactions.Add(transaction);
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    throw new Exception(i.ToString() + " -> " + e.Message);
                }
            }

            return document;
        }

    }
}
