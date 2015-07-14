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

namespace api.Controllers.Dbo
{
    public class PessoaController : ApiController
    {

        // GET /pessoa/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            try
            {
                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Retorno>(HttpStatusCode.OK, GatewayPessoa.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // POST /pessoa/token/
        public HttpResponseMessage Post(string token, [FromBody]pessoa param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Int32>(HttpStatusCode.OK, GatewayPessoa.Add(token, param));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }


        }

        // PUT /pessoa/token/
        public HttpResponseMessage Put(string token, [FromBody]pessoa param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayPessoa.Update(token, param);
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

        // DELETE /pessoa/token/id_pesssoa
        public HttpResponseMessage Delete(string token, Int32 id_pesssoa)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayPessoa.Delete(token, id_pesssoa);
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
