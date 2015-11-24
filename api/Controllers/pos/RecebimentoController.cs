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
using System.IO;
using System.Net.Http.Headers;
using System.Data;
using Newtonsoft.Json;

namespace api.Controllers.Pos
{
    public class RecebimentoController : ApiController
    {

        // GET /Recebimento/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get", _db);
                HttpResponseMessage result = null;
                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                string outValue = null;
                

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db))
                {
                    Retorno dados = GatewayRecebimento.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString, _db);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    if (queryString.TryGetValue("" + (int)GatewayRecebimento.CAMPOS.EXPORTAR, out outValue))
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
                    Bibliotecas.LogAcaoUsuario.Save(log, _db);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            catch (Exception e)
            {
                log.codResposta = (int)HttpStatusCode.InternalServerError;
                log.msgErro = e.Message;
                Bibliotecas.LogAcaoUsuario.Save(log, _db);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }
        }

        
        // POST /Recebimento/token/
        public HttpResponseMessage Post(string token, [FromBody]Recebimento param)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post", _db);

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db) && Permissoes.GetRoleLevel(token, _db) <= 1)
                {
                    Int32 id = GatewayRecebimento.Add(token, param, _db);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log, _db);
                    return Request.CreateResponse(HttpStatusCode.OK, id);
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
                Bibliotecas.LogAcaoUsuario.Save(log, _db);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }


        }

        // PUT /Recebimento/token/
        public HttpResponseMessage Put(string token, [FromBody]Recebimento param)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put", _db);

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db) && Permissoes.GetRoleLevel(token, _db) <= 1)
                {
                    GatewayRecebimento.Update(token, param, _db);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log, _db);
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
                Bibliotecas.LogAcaoUsuario.Save(log, _db);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }
        }

        // DELETE /Recebimento/token/id
        public HttpResponseMessage Delete(string token, Int32 id)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject("id : " + id), "Delete", _db);

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db) && Permissoes.GetRoleLevel(token, _db) <= 1)
                {
                    GatewayRecebimento.Delete(token, id, _db);
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
                Bibliotecas.LogAcaoUsuario.Save(log, _db);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }
        }
    }
}
