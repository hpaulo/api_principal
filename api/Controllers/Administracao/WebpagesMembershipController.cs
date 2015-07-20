﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Models;
using api.Negocios.Administracao;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Controllers.Administracao
{
    public class WebpagesMembershipController : ApiController
    {

        // GET /webpages_Membership/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            try
            {
                Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Retorno>(HttpStatusCode.OK, GatewayWebpagesMembership.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // POST /webpages_Membership/token/
        public HttpResponseMessage Post(string token, [FromBody]webpages_Membership param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                    return Request.CreateResponse<Int32>(HttpStatusCode.OK, GatewayWebpagesMembership.Add(token, param));
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }


        }

        // PUT /webpages_Membership/token/
        public HttpResponseMessage Put(string token, [FromBody]webpages_Membership param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayWebpagesMembership.Update(token, param);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        // PUT /webpages_Membership/token/
        public HttpResponseMessage Put(string token, [FromBody]Models.Object.AlterarSenha param)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayWebpagesMembership.Update(token, param);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }


        // DELETE /webpages_Membership/token/UserId
        public HttpResponseMessage Delete(string token, Int32 UserId)
        {
            try
            {
                HttpResponseMessage retorno = new HttpResponseMessage();
                if (Permissoes.Autenticado(token))
                {
                    GatewayWebpagesMembership.Delete(token, UserId);
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
