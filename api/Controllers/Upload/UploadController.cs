using api.Bibliotecas;
using api.Models;
using api.Models.Object;
using api.Negocios.Admin;
using api.Negocios.Card;
using api.Negocios.Tax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace api.Controllers.Upload
{
    public class UploadController : ApiController
    {
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public enum TIPO
        {
            TITULO = 100, // CSV
            EXTRATO = 101, // PDF or OFX
            NFE = 102, // XML
            CERTIFICADODIGITAL = 103,
        }

        // POST /Upload/token/tipo
        public HttpResponseMessage Post(string token, int tipo)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(new { tipo = tipo }), "Post", _db);

                    HttpResponseMessage retorno = new HttpResponseMessage();
                    if (Permissoes.Autenticado(token, _db))
                    {
                        Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                        if (queryString != null)
                        {
                            // Remove os dois
                            queryString.Remove("token");
                            queryString.Remove("tipo");
                        }
                        object resp = null;
                        TIPO tipoUpload = (TIPO)tipo;
                        switch (tipoUpload) 
                        { 
                            case TIPO.TITULO : 
                                GatewayTitulosErp.Patch(token, log, _db);
                                log.codResposta = (int)HttpStatusCode.OK;
                                break;
                            case TIPO.EXTRATO:
                                resp = GatewayTbExtrato.Patch(token, queryString, log, _db);
                                log.codResposta = (int)HttpStatusCode.OK;
                                break;
                            case TIPO.NFE:
                                Mensagem mensagemN = GatewayTbManifesto.Patch(token, queryString, _db);
                                log.codResposta = mensagemN.cdMensagem;
                                if (mensagemN.cdMensagem != 200) log.msgErro = mensagemN.dsMensagem;
                                resp = mensagemN;
                                break;
                            case TIPO.CERTIFICADODIGITAL:
                                Mensagem mensagemC = GatewayTbEmpresa.Patch(token, queryString, _db);
                                log.codResposta = mensagemC.cdMensagem;
                                if (mensagemC.cdMensagem != 200) log.msgErro = mensagemC.dsMensagem;
                                resp = mensagemC;
                                break;
                            default:
                                return Request.CreateResponse(HttpStatusCode.BadRequest);
                        }
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        if(resp != null)
                            return Request.CreateResponse(HttpStatusCode.OK, resp);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        log.codResposta = (int)HttpStatusCode.Unauthorized;
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    log.codResposta = (int)HttpStatusCode.InternalServerError;
                    log.msgErro = e.Message;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                }
            }
        }
    }
}