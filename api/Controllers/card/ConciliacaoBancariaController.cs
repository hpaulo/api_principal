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
            ////declare the transaction options
            //System.Transactions.TransactionOptions transactionOptions = new System.Transactions.TransactionOptions();
            ////set it to read uncommited
            //transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            ////create the transaction scope, passing our options in
            //using (System.Transactions.TransactionScope transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            //{
            //    //declare our context
            //    using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            //    {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get", _db);

                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db))
                {
                    Retorno dados = GatewayConciliacaoBancaria.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString, _db);
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
                log.codResposta = (int)HttpStatusCode.InternalServerError;
                log.msgErro = e.Message;
                Bibliotecas.LogAcaoUsuario.Save(log);//, _db);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }
            //    }
            //}
        }


        // POST /ConciliacaoBancaria/token/
        public HttpResponseMessage Post(string token, [FromBody]List<ConciliaRecebimentoParcela> param)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post", _db);

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db))
                {
                    GatewayConciliacaoBancaria.Post(token, param, _db);
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
                Bibliotecas.LogAcaoUsuario.Save(log);//, _db);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }
        }
       
        // PUT /ConciliacaoBancaria/token/
        public HttpResponseMessage Put(string token, [FromBody]RecebimentosParcela param)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put", _db);

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db))
                {
                    GatewayConciliacaoBancaria.Update(token, param, _db);
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
                Bibliotecas.LogAcaoUsuario.Save(log);//, _db);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }
        }


        // PATCH: /ConciliacaoBancaria/token/ => upload de um arquivo ofx
        public HttpResponseMessage Patch(string token, [FromBody]List<BaixaTitulos> param)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Patch", _db);

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db))
                {
                    List<List<string>> arquivos = GatewayConciliacaoBancaria.Patch(token, param, _db);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log, _db);

                    HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);

                    List<string> nomesArquivo = new List<string>();
                    foreach (BaixaTitulos p in param)
                        //nomesArquivo.Add(p.dataRecebimento + "_" + p.idsRecebimento.Count);
                        nomesArquivo.Add(p.idExtrato.ToString());

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
                    Bibliotecas.LogAcaoUsuario.Save(log, _db);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            catch (Exception e)
            {
                log.codResposta = (int)HttpStatusCode.InternalServerError;
                log.msgErro = e.Message;
                Bibliotecas.LogAcaoUsuario.Save(log);//, _db);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
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
