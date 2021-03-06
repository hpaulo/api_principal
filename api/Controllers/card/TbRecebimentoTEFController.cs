﻿using System;
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

namespace api.Controllers.Card
{
    public class TbRecebimentoTEFController : ApiController
    {

        // GET /tbRecebimentoTEF/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
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
                    Retorno dados = GatewayTbRecebimentoTEF.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString, _db);
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
            catch(Exception e)
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

        // POST /tbRecebimentoTEF/token/
        public HttpResponseMessage Post(string token, [FromBody]tbRecebimentoTEF param)
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
                    Int32 dados = GatewayTbRecebimentoTEF.Add(token, param, _db);
                    log.codResposta = (int)HttpStatusCode.OK;
                    Bibliotecas.LogAcaoUsuario.Save(log, _db);
                    return Request.CreateResponse<Int32>(HttpStatusCode.OK, dados);
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
                Bibliotecas.LogAcaoUsuario.Save(log);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Fecha conexão
                _db.Database.Connection.Close();
                _db.Dispose();
            }
        }

        // PUT /tbRecebimentoTEF/token/
        public HttpResponseMessage Put(string token, [FromBody]tbRecebimentoTEF param)
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
                    GatewayTbRecebimentoTEF.Update(token, param, _db);
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

        // DELETE /tbRecebimentoTEF/token/idRecebimentoTEF
        public HttpResponseMessage Delete(string token, Int32 idRecebimentoTEF)
        {
            // Abre nova conexão
            painel_taxservices_dbContext _db = new painel_taxservices_dbContext();
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject("idRecebimentoTEF : " + idRecebimentoTEF), "Delete", _db);

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token, _db))
                {
                    GatewayTbRecebimentoTEF.Delete(token, idRecebimentoTEF, _db);
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
    }
}
