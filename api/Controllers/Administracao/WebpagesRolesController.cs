using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Models;
using api.Negocios.Administracao;
using api.Bibliotecas;
using api.Models.Object;
using Newtonsoft.Json;

namespace api.Controllers.Administracao
{
    public class WebpagesRolesController : ApiController
    {

        // GET /webpages_Roles/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                    HttpResponseMessage retorno = new HttpResponseMessage();

                    log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get", _db);

                    if (Permissoes.Autenticado(token, _db))
                    {
                        Retorno dados = GatewayWebpagesRoles.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString, _db);
                        log.codResposta = (int)HttpStatusCode.OK;
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse<Retorno>(HttpStatusCode.OK, dados);
                    }
                    else
                    {
                        log.codResposta = (int)HttpStatusCode.Unauthorized;
                        log.msgErro = "Unauthorized";
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    log.codResposta = (int)HttpStatusCode.InternalServerError;
                    log.msgErro = e.Message;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
        }

        // POST /webpages_Roles/token/
        public HttpResponseMessage Post(string token, [FromBody]webpages_Roles param)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    HttpResponseMessage retorno = new HttpResponseMessage();

                    log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post", _db);

                    if (Permissoes.Autenticado(token, _db))
                    {
                        Int32 dados = GatewayWebpagesRoles.Add(token, param, _db);
                        log.codResposta = (int)HttpStatusCode.OK;
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse<Int32>(HttpStatusCode.OK, dados);
                    }
                    else
                    {
                        log.codResposta = (int)HttpStatusCode.Unauthorized;
                        log.msgErro = "Unauthorized";
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Equals("401"))
                    {
                        log.codResposta = (int)HttpStatusCode.Unauthorized;
                        log.msgErro = "Unauthorized";
                        Bibliotecas.LogAcaoUsuario.Save(log);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        log.codResposta = (int)HttpStatusCode.InternalServerError;
                        log.msgErro = e.Message;
                        Bibliotecas.LogAcaoUsuario.Save(log);
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }
                }
            }

        }

        // PUT /webpages_Roles/token/
        public HttpResponseMessage Put(string token, [FromBody]webpages_Roles param)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    HttpResponseMessage retorno = new HttpResponseMessage();

                    log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put", _db);

                    if (Permissoes.Autenticado(token, _db))
                    {
                        GatewayWebpagesRoles.Update(token, param, _db);
                        log.codResposta = (int)HttpStatusCode.OK;
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        log.codResposta = (int)HttpStatusCode.Unauthorized;
                        log.msgErro = "Unauthorized";
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Equals("401"))
                    {
                        log.codResposta = (int)HttpStatusCode.Unauthorized;
                        log.msgErro = "Unauthorized";
                        Bibliotecas.LogAcaoUsuario.Save(log);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        log.codResposta = (int)HttpStatusCode.InternalServerError;
                        log.msgErro = e.Message;
                        Bibliotecas.LogAcaoUsuario.Save(log);
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }
                }
            }
        }

        // DELETE /webpages_Roles/token/RoleId
        public HttpResponseMessage Delete(string token, Int32 RoleId)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    HttpResponseMessage retorno = new HttpResponseMessage();

                    log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject("RoleId : " + RoleId), "Delete", _db);

                    if (Permissoes.Autenticado(token, _db))
                    {
                        GatewayWebpagesRoles.Delete(token, RoleId, _db);
                        log.codResposta = (int)HttpStatusCode.OK;
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        log.codResposta = (int)HttpStatusCode.Unauthorized;
                        log.msgErro = "Unauthorized";
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                catch (Exception e)
                {
                    log.codResposta = (int)HttpStatusCode.InternalServerError;
                    log.msgErro = e.Message;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}
