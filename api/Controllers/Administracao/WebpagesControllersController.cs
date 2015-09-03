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
    public class WebpagesControllersController : ApiController
    {

        // GET /webpages_Controllers/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                var URLAPI = Request.RequestUri.Segments[1].ToUpper() + Request.RequestUri.Segments[2].ToUpper().Replace("/", "");

                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();

                log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get");

                if (Permissoes.Autenticado(token))
                {
                    Retorno dados = GatewayWebpagesControllers.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    return Request.CreateResponse<Retorno>(HttpStatusCode.OK, dados);
                }
                else
                {
                    log.codResposta = (int)HttpStatusCode.Unauthorized;
                    Bibliotecas.LogAcaoUsuario.Save(log);
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

        // POST /webpages_Controllers/token/
        public HttpResponseMessage Post(string token, [FromBody]Models.Object.CadastroController param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();

                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post");

                if (Permissoes.Autenticado(token))
                {
                    Int32 dados = GatewayWebpagesControllers.Add(token, param);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    return Request.CreateResponse<Int32>(HttpStatusCode.OK, dados);
                }
                else
                {
                    log.codResposta = (int)HttpStatusCode.Unauthorized;
                    Bibliotecas.LogAcaoUsuario.Save(log);
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

        // PUT /webpages_Controllers/token/
        public HttpResponseMessage Put(string token, [FromBody]webpages_Controllers param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();

                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put");

                if (Permissoes.Autenticado(token))
                {
                    GatewayWebpagesControllers.Update(token, param);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    log.codResposta = (int)HttpStatusCode.Unauthorized;
                    Bibliotecas.LogAcaoUsuario.Save(log);
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

        // DELETE /webpages_Controllers/token/id_controller
        public HttpResponseMessage Delete(string token, Int32 id_controller)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();

                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject("id_controller : " + id_controller), "Delete");

                if (Permissoes.Autenticado(token))
                {
                    GatewayWebpagesControllers.Delete(token, id_controller);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    log.codResposta = (int)HttpStatusCode.Unauthorized;
                    Bibliotecas.LogAcaoUsuario.Save(log);
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
