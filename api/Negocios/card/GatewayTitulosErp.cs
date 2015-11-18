using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;
using api.Negocios.Pos;
using Microsoft.Ajax.Utilities;
using api.Negocios.Util;
using System.Globalization;
using System.Net.Http;

namespace api.Negocios.Card
{
    public class GatewayTitulosErp
    {
        public static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTitulosErp()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            DATA = 100,
            ID_GRUPO = 101,
        };

        private readonly static string DOMINIO = System.Configuration.ConfigurationManager.AppSettings["DOMINIO"];



        private static Retorno carregaTitulos(string token, string dsAPI, string data, int pageSize = 0, int pageNumber = 0)
        {
            // Coloca a data no padrão de sql
            data = data.Substring(0, 4) + "-" + data.Substring(4, 2) + "-" + data.Substring(6, 2);

            string url = "http://" + dsAPI + DOMINIO;
            string complemento = "titulos/consultatitulos/" + token + "?" + ("" + (int)CAMPOS.DATA) + "=" + data;


            HttpClient client = new System.Net.Http.HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
            System.Net.Http.HttpResponseMessage response = client.GetAsync(complemento).Result;

            //se retornar com sucesso busca os dados
            if (response.IsSuccessStatusCode)
            {
                //Pegando os dados do Rest e armazenando na variável retorno
                Retorno retorno = response.Content.ReadAsAsync<Retorno>().Result;

                retorno.TotalDeRegistros = retorno.Registros.Count;

                // Obtém os registros
                List<dynamic> titulos = new List<dynamic>();
                foreach (dynamic registro in retorno.Registros)
                {
                    string nrCNPJ = registro.nrCNPJ;
                    Int32 cdAdquirente = Convert.ToInt32(registro.cdAdquirente);

                    titulos.Add(new
                    {
                        empresa = _db.empresas.Where(f => f.nu_cnpj.Equals(nrCNPJ)).Select(f => new
                        {
                            f.nu_cnpj,
                            f.ds_fantasia,
                            f.filial
                        }).FirstOrDefault(),
                        nrNSU = registro.nrNSU,
                        dtVenda = registro.dtVenda,
                        tbAdquirente = _db.tbAdquirentes.Where(a => a.cdAdquirente == cdAdquirente).Select(a => new
                        {
                            a.cdAdquirente,
                            a.nmAdquirente
                        }).FirstOrDefault(),
                        dsBandeira = registro.dsBandeira,
                        vlVenda = registro.vlVenda,
                        qtParcelas = registro.qtParcelas,
                        dtTitulo = registro.dtTitulo,
                        vlParcela = registro.vlParcela,
                        nrParcela = registro.nrParcela,
                        cdERP = registro.cdERP,
                        dtBaixaERP = registro.dtBaixaERP,
                    });
                }


                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (titulos.Count > pageSize && pageNumber > 0 && pageSize > 0)
                {
                    titulos = titulos.OrderBy(r => r.empresa.ds_fantasia)
                                     .ThenBy(r => r.dtVenda)
                                     .ThenBy(r => r.tbAdquirente.nmAdquirente)
                                     .ThenBy(r => r.dsBandeira)
                                     .ThenBy(r => r.dtTitulo)
                                     .Skip(skipRows).Take(pageSize)
                                     .ToList<dynamic>();
                }
                else
                {
                    pageNumber = 1;
                    titulos = titulos.OrderBy(r => r.empresa.ds_fantasia)
                                     .ThenBy(r => r.dtVenda)
                                     .ThenBy(r => r.tbAdquirente.nmAdquirente)
                                     .ThenBy(r => r.dsBandeira)
                                     .ThenBy(r => r.dtTitulo)
                                     .ToList<dynamic>();
                }

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                retorno.Registros = titulos;

                return retorno;

            }
            else
            {
                string resp = response.Content.ReadAsAsync<string>().Result;
                if (resp != null && !resp.Trim().Equals(""))
                    throw new Exception(((int)response.StatusCode) + " - " + resp);
                throw new Exception(((int)response.StatusCode) + "");
            }

        }



        /// <summary>
        /// Retorna Títulos ERP
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                string outValue = null;

                // DATA
                string data = String.Empty;
                if (!queryString.TryGetValue("" + (int)CAMPOS.DATA, out outValue))
                    throw new Exception("O identificador da movimentação bancária deve ser informada para a baixa automática!");

                data = queryString["" + (int)CAMPOS.DATA];

                // GRUPO EMPRESA => OBRIGATÓRIO!
                Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                if (IdGrupo == 0 && queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    IdGrupo = Convert.ToInt32(queryString["" + (int)CAMPOS.ID_GRUPO]);
                if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a baixa automática!");

                grupo_empresa grupo_empresa = _db.grupo_empresa.Where(e => e.id_grupo == IdGrupo).FirstOrDefault();

                if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                    throw new Exception("Permissão negada! Empresa não possui o serviço ativo");

                return carregaTitulos(token, grupo_empresa.dsAPI, data, pageSize, pageNumber);

            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar títulos ERP" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

        /// <summary>
        /// Importa títulos
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void ImportaTitulos(string token, ImportaTitulos param)
        {
            try
            {
                if (param != null) 
                { 
                    // GRUPO EMPRESA => OBRIGATÓRIO!
                    Int32 IdGrupo = Permissoes.GetIdGrupo(token);
                    //if (IdGrupo == 0 && param.id_grupo != 0) IdGrupo = param.id_grupo;
                    if (IdGrupo == 0) throw new Exception("Um grupo deve ser selecionado como para a baixa automática!");

                    grupo_empresa grupo_empresa = _db.grupo_empresa.Where(e => e.id_grupo == IdGrupo).FirstOrDefault();

                    if (grupo_empresa.dsAPI == null || grupo_empresa.dsAPI.Equals(""))
                        throw new Exception("Permissão negada! Empresa não possui o serviço ativo");

                    Retorno retorno = carregaTitulos(token, grupo_empresa.dsAPI, param.data);

                    foreach (dynamic tit in retorno.Registros)
                    {
                        tbRecebimentoTitulo tbRecebimentoTitulo = new tbRecebimentoTitulo
                        {
                            dsBandeira = tit.dsBandeira,
                            cdAdquirente = tit.tbAdquirente.cdAdquirente,
                            cdERP = tit.cdERP,
                            dtBaixaERP = tit.dtBaixaERP,
                            dtTitulo = tit.dtTitulo,
                            dtVenda = tit.dtVenda,
                            nrCNPJ = tit.empresa.nu_cnpj,
                            nrNSU = tit.nrNSU,
                            nrParcela = Convert.ToByte(tit.nrParcela),
                            qtParcelas = Convert.ToByte(tit.qtParcelas),
                            vlParcela = Convert.ToDecimal(tit.vlParcela),
                            vlVenda = Convert.ToDecimal(tit.vlVenda),
                        };

                        tbRecebimentoTitulo titulo = _db.tbRecebimentoTitulos
                                                                // Unique
                                                                .Where(e => e.nrCNPJ.Equals(tbRecebimentoTitulo.nrCNPJ))
                                                                .Where(e => e.nrNSU.Equals(tbRecebimentoTitulo.nrNSU))
                                                                .Where(e => e.dtTitulo.Equals(tbRecebimentoTitulo.dtTitulo))
                                                                .Where(e => e.nrParcela == tbRecebimentoTitulo.nrParcela)
                                                                .FirstOrDefault();

                        if (titulo == null)
                            GatewayTbRecebimentoTitulo.Add(token, tbRecebimentoTitulo);
                        else
                        {
                            tbRecebimentoTitulo.idRecebimentoTitulo = titulo.idRecebimentoTitulo;
                            GatewayTbRecebimentoTitulo.Update(token, tbRecebimentoTitulo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao importar títulos ERP" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
