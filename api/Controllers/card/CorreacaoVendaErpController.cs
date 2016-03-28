using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Models;
using api.Negocios.Card;
using api.Bibliotecas;
using api.Models.Object;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;
using api.Negocios.Util;

namespace api.Controllers.Card
{
    public class CorreacaoVendaErpController : ApiController
    {
        // GET /BaixaAutomaticaERP/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get", _db);

                    Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                    HttpResponseMessage retorno = new HttpResponseMessage();
                    if (Permissoes.Autenticado(token, _db))
                    {
                        Retorno dados = GatewayCorrecaoVendaErp.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString, _db);
                        log.codResposta = (int)HttpStatusCode.OK;
                        Bibliotecas.LogAcaoUsuario.Save(log, _db);
                        return Request.CreateResponse<Retorno>(HttpStatusCode.OK, dados);
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
                    HttpStatusCode httpStatus = e.Message.StartsWith("Permissão negada") || e.Message.StartsWith("401") ? HttpStatusCode.Unauthorized : HttpStatusCode.InternalServerError;
                    log.codResposta = (int)httpStatus;
                    log.msgErro = e.Message;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    return Request.CreateResponse(httpStatus, e.Message);
                    //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}