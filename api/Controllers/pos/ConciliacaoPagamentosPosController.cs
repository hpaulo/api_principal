using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Models;
using api.Negocios.Pos;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Controllers.Pos
{
    public class ConciliacaoPagamentosPosController : ApiController
    {

        // GET /ConciliacaoPagamentosPos/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            try
            {
                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Retorno>(HttpStatusCode.OK, GatewayConciliacaoPagamentosPos.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // POST /ConciliacaoPagamentosPos/token/
        public HttpResponseMessage Post(string token, [FromBody]ConciliacaoPagamentosPos param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Int32>(HttpStatusCode.OK, GatewayConciliacaoPagamentosPos.Add(token, param));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }


        }

        // PUT /ConciliacaoPagamentosPos/token/
        public HttpResponseMessage Put(string token, [FromBody]ConciliacaoPagamentosPos param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayConciliacaoPagamentosPos.Update(token, param);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // DELETE /ConciliacaoPagamentosPos/token/IdConciliacaoPagamento
        public HttpResponseMessage Delete(string token, Int32 IdConciliacaoPagamento)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayConciliacaoPagamentosPos.Delete(token, IdConciliacaoPagamento);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}
