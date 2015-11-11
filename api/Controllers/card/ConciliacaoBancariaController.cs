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
    public class ConciliacaoBancariaController : ApiController
    {
        // GET /ConciliacaoBancaria/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get");

                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    Retorno dados = GatewayConciliacaoBancaria.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString);
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

       
        // PUT /ConciliacaoBancaria/token/
        public HttpResponseMessage Put(string token, [FromBody]List<ConciliaRecebimentoParcela> param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayConciliacaoBancaria.Update(token, param);
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


        // PATCH: /ConciliacaoBancaria/token/ => upload de um arquivo ofx
        public HttpResponseMessage Patch(string token, [FromBody]List<BaixaTitulos> param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Patch");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    List<List<string>> arquivos = GatewayConciliacaoBancaria.Patch(token, param);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log);

                    HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);

                    List<string> nomesArquivo = new List<string>();
                    foreach (BaixaTitulos p in param)
                        nomesArquivo.Add(p.dataRecebimento + "_" + p.idsRecebimento.Count);

                    if (arquivos.Count == 1)
                    {
                        string nmArquivo = nomesArquivo[0] + ".csv";
                        result.Content = new StreamContent(new MemoryStream(Bibliotecas.Converter.ListToCSV(arquivos[0])));
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        result.Content.Headers.ContentDisposition.FileName = nmArquivo;
                        result.Content.Headers.Add("x-filename", nmArquivo);
                    }
                    else if(arquivos.Count > 1)
                    {
                        string nmArquivo = "file" + DateTime.Now.ToString().Replace("/", "-") + ".zip";
                        result.Content = new StreamContent(new MemoryStream(GatewayUtilNfe.DownloadZipCSVs(arquivos, nomesArquivo)));
                        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        result.Content.Headers.ContentDisposition.FileName = nmArquivo;
                        result.Content.Headers.Add("x-filename", nmArquivo);
                    }
                    return result;
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
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        
    }

}
