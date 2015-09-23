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
using Newtonsoft.Json;
using api.Models;
using System.IO;
using System.Net.Http.Headers;
using System.Data;

namespace api.Controllers.Pos
{
    public class RecebimentoParcelaController : ApiController
    {

        // GET /RecebimentoParcela/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get");

                HttpResponseMessage result = null;
                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                string outValue = null;
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    Retorno dados = GatewayRecebimentoParcela.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    if (queryString.TryGetValue("" + (int)GatewayRecebimentoParcela.CAMPOS.EXPORTAR, out outValue))
                    {
                        result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new StreamContent(new MemoryStream(Negocios.Util.GatewayExportar.CSV(dados.Registros.ToArray())));
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        result.Content.Headers.ContentDisposition.FileName = "file" + DateTime.Now.ToString();
                        return result;
                    }
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

        // POST /RecebimentoParcela/token/
        /*public HttpResponseMessage Post(string token, [FromBody]RecebimentoParcela param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Int32>(HttpStatusCode.OK, GatewayRecebimentoParcela.Add(token, param));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }


        }

        // PUT /RecebimentoParcela/token/
        public HttpResponseMessage Put(string token, [FromBody]RecebimentoParcela param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayRecebimentoParcela.Update(token, param);
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

        // DELETE /RecebimentoParcela/token/idRecebimento
        public HttpResponseMessage Delete(string token, Int32 idRecebimento)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayRecebimentoParcela.Delete(token, idRecebimento);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }*/

        // PUT /RecebimentoParcela/token/
        public HttpResponseMessage Put(string token, RecebimentosParcela param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayRecebimentoParcela.Update(token, param);
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