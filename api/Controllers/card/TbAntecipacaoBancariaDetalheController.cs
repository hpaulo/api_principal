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

namespace api.Controllers.Card
{
    public class TbAntecipacaoBancariaDetalheController : ApiController
    {

        // GET /tbAntecipacaoBancariaDetalhe/token/colecao/campo/orderBy/pageSize/pageNumber?CAMPO1=VALOR&CAMPO2=VALOR
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get", _db);

                    Dictionary<string, string> queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                    HttpResponseMessage retorno = new HttpResponseMessage();
                    if (Permissoes.Autenticado(token))
                    {
                        Retorno dados = GatewayTbAntecipacaoBancariaDetalhe.Get(token, colecao, campo, orderBy, pageSize, pageNumber, queryString, _db);
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
            }
        }


        // POST /tbAntecipacaoBancariaDetalhe/token/
        //public HttpResponseMessage Post(string token, [FromBody]tbAntecipacaoBancariaDetalhe param)
        //{
        //    // Abre nova conexão
        //    using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
        //    {
        //        tbLogAcessoUsuario log = new tbLogAcessoUsuario();
        //        try
        //        {
        //            log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Post", _db);

        //            HttpResponseMessage retorno = new HttpResponseMessage();
        //            if (Permissoes.Autenticado(token, _db))
        //            {
        //                Int32 dados = GatewayTbAntecipacaoBancariaDetalhe.Add(token, param, _db);
        //                log.codResposta = (int)HttpStatusCode.OK;
        //                Bibliotecas.LogAcaoUsuario.Save(log, _db);
        //                return Request.CreateResponse<Int32>(HttpStatusCode.OK, dados);
        //            }
        //            else
        //            {
        //                log.codResposta = (int)HttpStatusCode.Unauthorized;
        //                Bibliotecas.LogAcaoUsuario.Save(log, _db);
        //                return Request.CreateResponse(HttpStatusCode.Unauthorized);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.codResposta = (int)HttpStatusCode.InternalServerError;
        //            log.msgErro = e.Message;
        //            Bibliotecas.LogAcaoUsuario.Save(log);//, _db);
        //            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        //        }
        //    }
        //}


        // PUT /tbAntecipacaoBancariaDetalhe/token/
        public HttpResponseMessage Put(string token, [FromBody]AntecipacaoBancariaAnteciparParcelas param)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {
                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject(param), "Put", _db);

                    HttpResponseMessage retorno = new HttpResponseMessage();
                    if (Permissoes.Autenticado(token, _db))// && Permissoes.isAtosRole(token, _db))
                    {
                        GatewayTbAntecipacaoBancariaDetalhe.AntecipaParcelas(token, param, _db);
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
                    //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                }
            }
        }


        // DELETE /tbAntecipacaoBancariaDetalhe/token/idAntecipacaoBancariaDetalhe
        //public HttpResponseMessage Delete(string token, Int32 idAntecipacaoBancariaDetalhe)
        //{
        //    // Abre nova conexão
        //    using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
        //    {
        //        tbLogAcessoUsuario log = new tbLogAcessoUsuario();
        //        try
        //        {
        //            log = Bibliotecas.LogAcaoUsuario.New(token, JsonConvert.SerializeObject("idAntecipacaoBancariaDetalhe : " + idAntecipacaoBancariaDetalhe), "Delete", _db);

        //            HttpResponseMessage retorno = new HttpResponseMessage();
        //            if (Permissoes.Autenticado(token, _db))
        //            {
        //                GatewayTbAntecipacaoBancariaDetalhe.Delete(token, idAntecipacaoBancariaDetalhe, _db);
        //                log.codResposta = (int)HttpStatusCode.OK;
        //                Bibliotecas.LogAcaoUsuario.Save(log, _db);
        //                return Request.CreateResponse(HttpStatusCode.OK);
        //            }
        //            else
        //            {
        //                log.codResposta = (int)HttpStatusCode.Unauthorized;
        //                Bibliotecas.LogAcaoUsuario.Save(log, _db);
        //                return Request.CreateResponse(HttpStatusCode.Unauthorized);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            log.codResposta = (int)HttpStatusCode.InternalServerError;
        //            log.msgErro = e.Message;
        //            Bibliotecas.LogAcaoUsuario.Save(log);//, _db);
        //            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        //        }
        //    }
        //}
    }
}