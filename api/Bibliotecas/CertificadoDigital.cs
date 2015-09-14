using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using api.Models.Object;

namespace api.Bibliotecas
{
    public static class CertificadoDigital
    {
        #region GetDataValidade
        public static DateTime GetDataValidade(byte[] certificado, string senha)
        {
            X509Certificate2 certific = new X509Certificate2(certificado, senha,
    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            DateTime dtValidade = DateTime.Parse(certific.GetExpirationDateString());
            return dtValidade;
        }
        #endregion

        #region ValidarCertificado
        public static Mensagem ValidarCertificado(byte[] certificado, string senha)
        {
            Mensagem mensagem = new Mensagem();
            try
            {
                X509Certificate2 certific = new X509Certificate2(certificado, senha,
    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

                DateTime dtValidade = DateTime.Parse(certific.GetExpirationDateString());
                if (dtValidade >= DateTime.Now)
                {
                    mensagem.cdMensagem = 200;
                    mensagem.dsMensagem = "Certificado válido";
                }
                else
                {
                    mensagem.cdMensagem = 203;
                    mensagem.dsMensagem = "Certificado vencido";
                }

                return mensagem;

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("A senha de rede especificada não está correta"))
                {
                    mensagem.cdMensagem = 201;
                    mensagem.dsMensagem = "Senha incorreta";
                }
                else
                {
                    mensagem.cdMensagem = 202;
                    mensagem.dsMensagem = "Certificado inválido (" + ex.Message + ")";
                }
                return mensagem;
            }
        }
        #endregion

        #region DecodeFrom64
        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
        #endregion
    }
}