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
    public class ExtratoPDFBNB
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
            document.Account.BankID = "004";
            document.Transactions = new List<Transaction>();
            document.SignOn = new SignOn();

            string strText = string.Empty;
            bool tratamento = false;
            bool dataGeracaoExtrato = false;
            string stringTratamento1 = String.Empty;
            string stringTratamento2 = String.Empty;
            string stringTratamento3 = String.Empty;
            string textTemp = String.Empty;


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

            bool head = false;
            Transaction transaction = new Transaction();

            for (int i = 0; i < temp.Count; i++)
            {
                string value = String.Empty;
                try 
                {
                    if (i == 1)
                    {
                        // [Data de geração do extrato]
                        if (iniciaComData(temp[i].ToString()))
                        {
                            value = temp[i].ToString().TrimStart().Substring(0, 10);
                            try
                            {
                                document.SignOn.DTServer = Convert.ToDateTime(value);
                                dataGeracaoExtrato = true;
                            }
                            catch
                            {
                                // falha ao obter data de geração do extrato
                            }
                        }
                    }
                    else if (!head && temp[i].ToString().Trim().StartsWith("Agência:"))
                    {
                        value = temp[i].ToString().Substring(temp[i].ToString().IndexOf("Agência:") + 8);
                        document.Account.BranchID = value.Trim().Substring(0, value.Trim().IndexOf(" "));
                        if (value.Contains("Conta Corrente:"))
                        {
                            value = temp[i].ToString().Substring((temp[i].ToString().IndexOf("Corrente:") + 9), 9);
                            document.Account.AccountID = value.Trim();
                        }
                    }
                    else if (!head && temp[i].ToString().Trim().StartsWith("Conta Corrente:"))
                    {
                        value = temp[i].ToString().Substring((temp[i].ToString().IndexOf("Corrente:") + 9), 9);
                        document.Account.AccountID = value.Trim();
                    }
                    else if (!head && temp[i].ToString().Trim().Contains("Período:"))
                    {
                        value = temp[i].ToString().TrimEnd();
                        value = value.Substring(value.IndexOf("Período:") + 8);
                        int indiceAte = value.IndexOf("até");
                        try
                        {
                            document.StatementStart = Convert.ToDateTime(value.Substring(0, indiceAte).Trim());
                        }
                        catch
                        {
                            // falha ao obter data início das movimentações
                        }
                        try
                        {
                            document.StatementEnd = Convert.ToDateTime(value.Substring(indiceAte + 3).Trim());
                        }
                        catch
                        {
                            // falha ao obter data final das movimentações
                        }
                    }
                    else if (!head && temp[i].ToString().Trim().Contains("Data Histórico Documento Valor R$ Saldo R$"))
                        head = true;
                    else if (head && temp[i].ToString().IndexOf("https") < 0 &&
                             !temp[i].ToString().EndsWith("Nordeste Eletrônico") &&
                             !temp[i].ToString().StartsWith("Nordeste Eletrônico Page") &&
                             !temp[i].ToString().StartsWith("Importante:") &&
                             !temp[i].ToString().StartsWith("Não constam valores de aplicações e resgates efetuados no dia.") &&
                             !temp[i].ToString().StartsWith("Banco do Nordeste - Cliente Consulta | Ouvidoria:"))
                    {

                        String cipherText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(temp[i].ToString())));

                        if (iniciaComData(cipherText))
                        {

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
                            if (index > 1 && auxiliar[index - 1] == '-')
                            {
                                auxiliar = auxiliar.Substring(0, index) + auxiliar.Substring(index + 1);
                                index -= 2; // valor negativo vem o '-' separado por espaço
                            }
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
                            if (index > 1 && auxiliar[index - 1] == '-')
                            {
                                auxiliar = auxiliar.Substring(0, index) + auxiliar.Substring(index + 1);
                                index -= 2; // valor negativo vem o '-' separado por espaço
                            }
                            try
                            {
                                Convert.ToInt32(auxiliar.Substring(index + 1));
                            }
                            catch
                            {
                                amount = Convert.ToDecimal(auxiliar.Substring(index + 1));
                                auxiliar = auxiliar.Substring(0, index).TrimEnd();
                                index = auxiliar.LastIndexOf(" ");
                                if (index > 1 && auxiliar[index - 1] == '-')
                                {
                                    auxiliar = auxiliar.Substring(0, index) + auxiliar.Substring(index + 1);
                                    index -= 2; // valor negativo vem o '-' separado por espaço
                                }
                                Convert.ToInt32(auxiliar.Substring(index + 1));
                            }
                            transaction.CheckNum = auxiliar.Substring(index + 1);
                            transaction.Amount = amount;
                            transaction.Memo = auxiliar.Substring(0, index).TrimEnd();
                            transaction.TransType = transaction.Amount > 0 ? OFXTransactionType.CREDIT : OFXTransactionType.DEBIT;

                            if (!tratamento)
                            {
                                document.Transactions.Add(transaction);
                                transaction = new Transaction();
                            }
                        }
                        else
                        {
                            textTemp += " " + cipherText;

                            if (!tratamento)
                                tratamento = true;
                            else
                            {
                                tratamento = false;
                                transaction.Memo = textTemp.Trim();
                                document.Transactions.Add(transaction);
                                textTemp = String.Empty;
                                transaction = new Transaction();
                            }
                        }
                    }
                    else if (!dataGeracaoExtrato && temp[i].ToString().IndexOf("https") >= 0)
                    {
                        value = temp[i].ToString().Trim();
                        value = value.Substring(value.LastIndexOf(" ") + 1);
                        try
                        {
                            document.SignOn.DTServer = Convert.ToDateTime(value);
                            dataGeracaoExtrato = true;
                        }
                        catch
                        {
                            // falha ao obter data de geração do extrato
                        }
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