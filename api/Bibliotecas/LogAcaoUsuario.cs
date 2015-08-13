using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace api.Bibliotecas
{
    public class LogAcaoUsuario
    {
        public static tbLogAcessoUsuario New(string token, string dsJson)
        {
            try
            {

                    tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                    log.idUser = Bibliotecas.Permissoes.GetIdUser(token);
                    log.dsUrl = HttpContext.Current.Request.Url.Segments[1] + HttpContext.Current.Request.Url.Segments[2];
                    log.dsParametros = HttpContext.Current.Request.RawUrl.Replace( (HttpContext.Current.Request.Url.Segments[0] + HttpContext.Current.Request.Url.Segments[1] + HttpContext.Current.Request.Url.Segments[2] ) ,"");
                    log.dsFiltros = HttpContext.Current.Request.RawUrl.LastIndexOf('?') > 0 ? HttpContext.Current.Request.RawUrl.Remove(0, HttpContext.Current.Request.RawUrl.LastIndexOf('?')) : String.Empty;
                    log.dtAcesso = DateTime.Now;
                    log.dsAplicacao = Bibliotecas.Device.IsMobile() ? "M" : "P";
                    log.dsMethod = HttpContext.Current.Request.HttpMethod;
                    /* Campos alimentados no controller*/
                        //log.codResposta = data.codResposta; 
                        //log.msgErro = data.msgErro;
                    log.dsJson = dsJson != null ? dsJson : String.Empty;
                    log.dsUserAgent = HttpContext.Current.Request.UserAgent;
                    return log;
            }
            catch (Exception e)
            {
                throw new Exception("Mensagem: " + e.Message);
            }
        }

        public static void Save(tbLogAcessoUsuario data)
        {
            try
            {
                using (var _db = new painel_taxservices_dbContext())
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    _db.tbLogAcessoUsuarios.Add(data);
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Mensagem: " + e.Message);
            }
        }
    }
}