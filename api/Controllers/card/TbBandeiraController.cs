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
    public class TbBandeiraController : ApiController
    {

        // GET /tbBandeira/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
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
                    Retorno dados = GatewayTbBandeira.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString);
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


        // POST /tbBandeira/token/
        public HttpResponseMessage Post(string token, [FromBody]tbBandeira param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    Int32 dados = GatewayTbBandeira.Add(token, param);
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
            catch (Exception e)
            {
                log.codResposta = (int)HttpStatusCode.InternalServerError;
                log.msgErro = e.Message;
                Bibliotecas.LogAcaoUsuario.Save(log);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }


        // PUT /tbBandeira/token/
        public HttpResponseMessage Put(string token, [FromBody]tbBandeira param)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayTbBandeira.Update(token, param);
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


        // DELETE /tbBandeira/token/cdBandeira
        public HttpResponseMessage Delete(string token, Int32 cdBandeira)
        {
            tbLogAcessoUsuario log = new tbLogAcessoUsuario();
            try
            {
                log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject("cdBandeira : " + cdBandeira), "Delete");

                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayTbBandeira.Delete(token, cdBandeira);
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