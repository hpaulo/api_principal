using api.Bibliotecas;
using api.Models;
using api.Models.Object;
using api.Negocios.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace api.Controllers.Util
{
    public class BancosController : ApiController
    {
        // GET: /Bancos/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                try
                {
                    Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                    HttpResponseMessage retorno = new HttpResponseMessage();
                    if (Permissoes.Autenticado(token, _db))
                        return Request.CreateResponse<Retorno>(HttpStatusCode.OK, GatewayBancos.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString));
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
}
