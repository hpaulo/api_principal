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
using System.Data.Entity.Validation;
using System.IO;
using System.Net.Http.Headers;
using System.Data;

namespace api.Controllers.Dbo
{
    public class PessoaController : ApiController
    {

        // GET /pessoa/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();

                log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get");

                if (Permissoes.Autenticado(token))
                {
                    /*string outValue = null;
                    if (queryString.TryGetValue("" + 9999, out outValue))
                    {
                        Models.painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
                        IList<pessoa> itens = _db.pessoas.Select(e => e).ToList<pessoa>();
                        //Retorno dados = GatewayPessoa.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString);
                        DataSet dataSet = Bibliotecas.Converter.ToDataSet<pessoa>(itens);// dados.Registros);

                        byte[] arquivo = Negocios.Util.GatewayExportar.Excel(dataSet);//ListDados);
                        string nmArquivo = "Relátorio de Pessoa.xlsx";

                        HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                        result.Content = new StreamContent(new MemoryStream(arquivo));
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        result.Content.Headers.ContentDisposition.FileName = nmArquivo;
                        result.Content.Headers.Add("x-filename", nmArquivo);
                        return result;
                    }
                    else
                    {*/
                        Retorno dados = GatewayPessoa.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString);
                        log.codResposta = (int)HttpStatusCode.OK;
                        Bibliotecas.LogAcaoUsuario.Save(log);
                        return Request.CreateResponse<Retorno>(HttpStatusCode.OK, dados);
                    //}
                }
                else
                {
                    log.codResposta = (int)HttpStatusCode.Unauthorized;
                    log.msgErro = "Unauthorized";
                    Bibliotecas.LogAcaoUsuario.Save(log);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            catch( Exception e)
            {
                log.codResposta = (int)HttpStatusCode.InternalServerError;
                log.msgErro = e.Message;
                Bibliotecas.LogAcaoUsuario.Save(log);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // POST /pessoa/token/
        public HttpResponseMessage Post(string token, [FromBody]pessoa param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    Int32 dados = GatewayPessoa.Add(token, param);
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
            catch(Exception e)
            {
                log.codResposta = (int)HttpStatusCode.InternalServerError;
                log.msgErro = e.Message;
                Bibliotecas.LogAcaoUsuario.Save(log);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }


        }

        // PUT /pessoa/token/
        public HttpResponseMessage Put(string token, [FromBody]pessoa param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayPessoa.Update(token, param);
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

        // DELETE /pessoa/token/id_pesssoa
        public HttpResponseMessage Delete(string token, Int32 id_pesssoa)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject("id_pesssoa : " + id_pesssoa), "Delete");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayPessoa.Delete(token, id_pesssoa);
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
            catch(Exception e)
            {
                log.codResposta = (int)HttpStatusCode.InternalServerError;
                log.msgErro = e.Message;
                Bibliotecas.LogAcaoUsuario.Save(log);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}
