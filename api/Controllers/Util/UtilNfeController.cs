using api.Bibliotecas;
using api.Models;
using api.Models.Object;
using api.Negocios.Tax;
using api.Negocios.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace api.Controllers.Util
{
    public class UtilNfeController : ApiController
    {
        public HttpResponseMessage Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0)
        {
            // Abre nova conexão
            using (painel_taxservices_dbContext _db = new painel_taxservices_dbContext())
            {

                tbLogAcessoUsuario log = new tbLogAcessoUsuario();
                try
                {
                    log = Bibliotecas.LogAcaoUsuario.New(token, null, "Get", _db);

                    Dictionary<string, string> queryString = new Dictionary<string, string>();
                    queryString = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                    HttpResponseMessage retorno = new HttpResponseMessage();
                    if (Permissoes.Autenticado(token, _db))
                    {

                        HttpResponseMessage result = null;
                        byte[] arquivo = null;
                        string nmArquivo = null;

                        if (colecao == 0) // [PORTAL] Download de um XML NFe
                        {
                            Retorno dados = GatewayTbManifesto.Get(token, 0, campo, orderBy, pageSize, pageNumber, queryString, _db);
                            if (dados.TotalDeRegistros > 0)
                            {
                                dynamic item = dados.Registros.Cast<dynamic>().Select(e => new { xmlNFe = e.xmlNFe, nrChave = e.nrChave }).First();
                                arquivo = GatewayUtilNfe.ArquivoXml(Convert.ToString(item.xmlNFe));
                                nmArquivo = item.nrChave + ".xml";

                                if (arquivo.Length > 0)
                                {
                                    result = Request.CreateResponse(HttpStatusCode.OK);
                                    result.Content = new StreamContent(new MemoryStream(arquivo));
                                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                    result.Content.Headers.ContentDisposition.FileName = nmArquivo;
                                    result.Content.Headers.Add("x-filename", nmArquivo);
                                    log.codResposta = (int)HttpStatusCode.OK;
                                    Bibliotecas.LogAcaoUsuario.Save(log, _db);
                                    return result;
                                }
                                else
                                {
                                    log.codResposta = (int)HttpStatusCode.InternalServerError;
                                    Bibliotecas.LogAcaoUsuario.Save(log);
                                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                                }

                                //return result;
                            }
                            else
                            {
                                log.codResposta = (int)HttpStatusCode.NotFound;
                                Bibliotecas.LogAcaoUsuario.Save(log, _db);
                                throw new HttpResponseException(HttpStatusCode.NotFound);
                            }
                        }
                        else if (colecao == 1) // [PORTAL] Download de um Arquivo ZIP Contendo vários XMLs NFe
                        {
                            Retorno dados = GatewayTbManifesto.Get(token, 0, campo, orderBy, pageSize, pageNumber, queryString, _db);
                            dynamic itens = dados.Registros.Cast<dynamic>().Select(e => new { xmlNFe = e.xmlNFe, nrChave = e.nrChave }).ToList();

                            arquivo = GatewayUtilNfe.DownloadZipXmls(dados.Registros);
                            dynamic item = dados.Registros.Cast<dynamic>().Select(e => new { xmlNFe = e.xmlNFe, nrChave = e.nrChave, dtEmissao = e.dtEmissao, nmEmitente = e.nmEmitente, nrEmitenteCNPJCPF = e.nrEmitenteCNPJCPF }).First();
                            string dtEmissao = item.dtEmissao.ToString().Replace('/', '-'); //DateTime.ParseExact(item.dtEmissao + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture).ToString("yyyyMMdd HH:mm:ss.fff");
                            nmArquivo = "XMLs - " + dtEmissao + " - " + item.nmEmitente + " - " + item.nrEmitenteCNPJCPF + ".zip";

                            if (arquivo.Length > 0)
                            {
                                result = Request.CreateResponse(HttpStatusCode.OK);
                                result.Content = new StreamContent(new MemoryStream(arquivo));
                                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                result.Content.Headers.ContentDisposition.FileName = nmArquivo;
                                result.Content.Headers.ContentDisposition.Name = nmArquivo;
                                result.Content.Headers.ContentDisposition.FileNameStar = nmArquivo;
                                result.Content.Headers.Add("x-filename", nmArquivo);
                            }
                            log.codResposta = (int)HttpStatusCode.OK;
                            Bibliotecas.LogAcaoUsuario.Save(log, _db);
                            return result;
                        }
                        else if (colecao == 2) // [PORTAL] Download de um PDF DANFE
                        {
                            Retorno dados = GatewayTbManifesto.Get(token, 0, campo, orderBy, pageSize, pageNumber, queryString, _db);
                            if (dados.TotalDeRegistros > 0)
                            {
                                dynamic item = dados.Registros.Cast<dynamic>().Select(e => new { xmlNFe = e.xmlNFe, nrChave = e.nrChave }).First();
                                arquivo = GatewayUtilNfe.PDFDanfe(Convert.ToString(item.xmlNFe), Convert.ToString(item.nrChave));
                                nmArquivo = item.nrChave + ".pdf";

                                if (arquivo.Length > 0)
                                {
                                    result = Request.CreateResponse(HttpStatusCode.OK);
                                    result.Content = new StreamContent(new MemoryStream(arquivo));
                                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                    result.Content.Headers.ContentDisposition.FileName = nmArquivo;
                                    result.Content.Headers.Add("x-filename", nmArquivo);
                                }
                                log.codResposta = (int)HttpStatusCode.OK;
                                Bibliotecas.LogAcaoUsuario.Save(log, _db);
                                return result;
                            }
                            else
                            {
                                log.codResposta = (int)HttpStatusCode.NotFound;
                                Bibliotecas.LogAcaoUsuario.Save(log, _db);
                                throw new HttpResponseException(HttpStatusCode.NotFound);
                            }
                        }
                        else if (colecao == 3) // [PORTAL] Download de um Arquivo ZIP Contendo vários PDFs DANFE
                        {
                            Retorno dados = GatewayTbManifesto.Get(token, 0, campo, orderBy, pageSize, pageNumber, queryString, _db);
                            dynamic itens = dados.Registros.Cast<dynamic>().Select(e => new { xmlNFe = e.xmlNFe, nrChave = e.nrChave }).ToList();

                            arquivo = GatewayUtilNfe.DownloadZipPdfs(dados.Registros);
                            dynamic item = dados.Registros.Cast<dynamic>().Select(e => new { xmlNFe = e.xmlNFe, nrChave = e.nrChave, dtEmissao = e.dtEmissao, nmEmitente = e.nmEmitente, nrEmitenteCNPJCPF = e.nrEmitenteCNPJCPF }).First();
                            string dtEmissao = item.dtEmissao.ToString().Replace('/', '-'); //DateTime.ParseExact(item.dtEmissao + " 00:00:00.000", "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture).ToString("yyyyMMdd HH:mm:ss.fff");
                            nmArquivo = "PDFs - " + dtEmissao + " - " + item.nmEmitente + " - " + item.nrEmitenteCNPJCPF + ".zip";

                            if (arquivo.Length > 0)
                            {
                                result = Request.CreateResponse(HttpStatusCode.OK);
                                result.Content = new StreamContent(new MemoryStream(arquivo));
                                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                result.Content.Headers.ContentDisposition.FileName = nmArquivo;
                                result.Content.Headers.Add("x-filename", nmArquivo);
                            }
                            log.codResposta = (int)HttpStatusCode.OK;
                            Bibliotecas.LogAcaoUsuario.Save(log, _db);
                            return result;
                        }
                        else
                        {
                            log.codResposta = (int)HttpStatusCode.NotFound;
                            Bibliotecas.LogAcaoUsuario.Save(log, _db);
                            throw new HttpResponseException(HttpStatusCode.NotFound);
                        }
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
            }
            
        }
    }
}
