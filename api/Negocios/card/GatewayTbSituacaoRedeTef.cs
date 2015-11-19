using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Card
{
    public class GatewayTbSituacaoRedeTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbSituacaoRedeTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDSITUACAOREDETEF = 100,
            CDREDETEF = 101,
            DSSITUACAO = 102,
            CDTIPOSITUACAO = 103,

        };

        /// <summary>
        /// Get TbSituacaoRedeTef/TbSituacaoRedeTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbSituacaoRedeTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbSituacaoRedeTefs.AsQueryable<tbSituacaoRedeTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDSITUACAOREDETEF:
                        short cdSituacaoRedeTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdSituacaoRedeTef.Equals(cdSituacaoRedeTef)).AsQueryable<tbSituacaoRedeTef>();
                        break;
                    case CAMPOS.CDREDETEF:
                        short cdRedeTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdRedeTef.Equals(cdRedeTef)).AsQueryable<tbSituacaoRedeTef>();
                        break;
                    case CAMPOS.DSSITUACAO:
                        string dsSituacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsSituacao.Equals(dsSituacao)).AsQueryable<tbSituacaoRedeTef>();
                        break;
                    case CAMPOS.CDTIPOSITUACAO:
                        short cdTipoSituacao = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdTipoSituacao.Equals(cdTipoSituacao)).AsQueryable<tbSituacaoRedeTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDSITUACAOREDETEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSituacaoRedeTef).AsQueryable<tbSituacaoRedeTef>();
                    else entity = entity.OrderByDescending(e => e.cdSituacaoRedeTef).AsQueryable<tbSituacaoRedeTef>();
                    break;
                case CAMPOS.CDREDETEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdRedeTef).AsQueryable<tbSituacaoRedeTef>();
                    else entity = entity.OrderByDescending(e => e.cdRedeTef).AsQueryable<tbSituacaoRedeTef>();
                    break;
                case CAMPOS.DSSITUACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsSituacao).AsQueryable<tbSituacaoRedeTef>();
                    else entity = entity.OrderByDescending(e => e.dsSituacao).AsQueryable<tbSituacaoRedeTef>();
                    break;
                case CAMPOS.CDTIPOSITUACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTipoSituacao).AsQueryable<tbSituacaoRedeTef>();
                    else entity = entity.OrderByDescending(e => e.cdTipoSituacao).AsQueryable<tbSituacaoRedeTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbSituacaoRedeTef/TbSituacaoRedeTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbSituacaoRedeTef = new List<dynamic>();
                Retorno retorno = new Retorno();

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
                var queryTotal = query;

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = queryTotal.Count();


                // PAGINAÇÃO
                int skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    query = query.Skip(skipRows).Take(pageSize);
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;

                // COLEÇÃO DE RETORNO
                if (colecao == 1)
                {
                    CollectionTbSituacaoRedeTef = query.Select(e => new
                    {

                        cdSituacaoRedeTef = e.cdSituacaoRedeTef,
                        cdRedeTef = e.cdRedeTef,
                        dsSituacao = e.dsSituacao,
                        cdTipoSituacao = e.cdTipoSituacao,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbSituacaoRedeTef = query.Select(e => new
                    {

                        cdSituacaoRedeTef = e.cdSituacaoRedeTef,
                        cdRedeTef = e.cdRedeTef,
                        dsSituacao = e.dsSituacao,
                        cdTipoSituacao = e.cdTipoSituacao,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbSituacaoRedeTef;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar situação rede tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbSituacaoRedeTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static short Add(string token, tbSituacaoRedeTef param)
        {
            try
            {
                _db.tbSituacaoRedeTefs.Add(param);
                _db.SaveChanges();
                return param.cdSituacaoRedeTef;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar situação rede tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbSituacaoRedeTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdSituacaoRedeTef)
        {
            try
            {
                _db.tbSituacaoRedeTefs.Remove(_db.tbSituacaoRedeTefs.Where(e => e.cdSituacaoRedeTef.Equals(cdSituacaoRedeTef)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar situação rede tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera tbSituacaoRedeTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbSituacaoRedeTef param)
        {
            try
            {
                tbSituacaoRedeTef value = _db.tbSituacaoRedeTefs
                        .Where(e => e.cdSituacaoRedeTef.Equals(param.cdSituacaoRedeTef))
                        .First<tbSituacaoRedeTef>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.cdSituacaoRedeTef != null && param.cdSituacaoRedeTef != value.cdSituacaoRedeTef)
                    value.cdSituacaoRedeTef = param.cdSituacaoRedeTef;
                if (param.cdRedeTef != null && param.cdRedeTef != value.cdRedeTef)
                    value.cdRedeTef = param.cdRedeTef;
                if (param.dsSituacao != null && param.dsSituacao != value.dsSituacao)
                    value.dsSituacao = param.dsSituacao;
                if (param.cdTipoSituacao != null && param.cdTipoSituacao != value.cdTipoSituacao)
                    value.cdTipoSituacao = param.cdTipoSituacao;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar situação rede tef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
