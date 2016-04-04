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
    public class CorrecaoVendaErpController : ApiController
    {
        // GET /BaixaAutomaticaERP/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Put(string token, [FromBody]CorrigeVendasErp param)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put", _db);

                    HttpResponseMessage retorno = new HttpResponseMessage();
                    if (Permissoes.Autenticado(token, _db))
                    {
                        GatewayCorrecaoVendaErp.CorrigeVenda(token, param, _db);
                        log.codResposta = (int)HttpStatusCode.OK;
                        Bibliotecas.LogAcaoUsuario.Save(log);
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
                    Bibliotecas.LogAcaoUsuario.Save(log);//, _db);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                    //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}