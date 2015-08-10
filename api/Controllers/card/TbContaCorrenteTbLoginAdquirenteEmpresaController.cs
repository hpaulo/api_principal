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

namespace api.Controllers.Card
{
    public class TbContaCorrenteTbLoginAdquirenteEmpresaController : ApiController
    {

        // GET /tbContaCorrente_tbLoginAdquirenteEmpresa/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            try
            {
                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Retorno>(HttpStatusCode.OK, GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // Adiciona via PUT
        // POST /tbContaCorrente_tbLoginAdquirenteEmpresa/token/    
        public HttpResponseMessage Post(string token, [FromBody]tbContaCorrente_tbLoginAdquirenteEmpresa param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Int32>(HttpStatusCode.OK, GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.Add(token, param));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch(Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }


        }

        // PUT /tbContaCorrente_tbLoginAdquirenteEmpresa/token/
        public HttpResponseMessage Put(string token, [FromBody]tbContaCorrente_tbLoginAdquirenteEmpresa param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.Update(token, param);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch(Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // DELETE /tbContaCorrente_tbLoginAdquirenteEmpresa/token/cdContaCorrente
        public HttpResponseMessage Delete(string token, Int32 cdContaCorrente, Int32 cdLoginAdquirenteEmpresa, DateTime dtInicio)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayTbContaCorrenteTbLoginAdquirenteEmpresa.Delete(token, cdContaCorrente, cdLoginAdquirenteEmpresa, dtInicio);
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
