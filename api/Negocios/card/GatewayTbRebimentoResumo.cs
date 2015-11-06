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
    public class GatewayTbRebimentoResumo
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRebimentoResumo()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDREBIMENTORESUMO = 100,
            CDADQUIRENTE = 101,
            CDBANDEIRA = 102,
            CDTIPOPRODUTOTEF = 103,
            CDTERMINAL = 104,
            DTVENDA = 105,
            VLVENDABRUTO = 106,

        };

        /// <summary>
        /// Get TbRebimentoResumo/TbRebimentoResumo
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbRebimentoResumo> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRebimentoResumos.AsQueryable<tbRebimentoResumo>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDREBIMENTORESUMO:
                        Int32 idRebimentoResumo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRebimentoResumo.Equals(idRebimentoResumo)).AsQueryable<tbRebimentoResumo>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbRebimentoResumo>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira.Equals(cdBandeira)).AsQueryable<tbRebimentoResumo>();
                        break;
                    case CAMPOS.CDTIPOPRODUTOTEF:
                        short cdTipoProdutoTef = short.Parse(item.Value);
                        entity = entity.Where(e => e.cdTipoProdutoTef.Equals(cdTipoProdutoTef)).AsQueryable<tbRebimentoResumo>();
                        break;
                    case CAMPOS.CDTERMINAL:
                        Int32 cdTerminal = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdTerminal.Equals(cdTerminal)).AsQueryable<tbRebimentoResumo>();
                        break;
                    case CAMPOS.DTVENDA:
                        DateTime dtVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtVenda.Equals(dtVenda)).AsQueryable<tbRebimentoResumo>();
                        break;
                    case CAMPOS.VLVENDABRUTO:
                        decimal vlVendaBruto = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVendaBruto.Equals(vlVendaBruto)).AsQueryable<tbRebimentoResumo>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDREBIMENTORESUMO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRebimentoResumo).AsQueryable<tbRebimentoResumo>();
                    else entity = entity.OrderByDescending(e => e.idRebimentoResumo).AsQueryable<tbRebimentoResumo>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbRebimentoResumo>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbRebimentoResumo>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbRebimentoResumo>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbRebimentoResumo>();
                    break;
                case CAMPOS.CDTIPOPRODUTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTipoProdutoTef).AsQueryable<tbRebimentoResumo>();
                    else entity = entity.OrderByDescending(e => e.cdTipoProdutoTef).AsQueryable<tbRebimentoResumo>();
                    break;
                case CAMPOS.CDTERMINAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTerminal).AsQueryable<tbRebimentoResumo>();
                    else entity = entity.OrderByDescending(e => e.cdTerminal).AsQueryable<tbRebimentoResumo>();
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVenda).AsQueryable<tbRebimentoResumo>();
                    else entity = entity.OrderByDescending(e => e.dtVenda).AsQueryable<tbRebimentoResumo>();
                    break;
                case CAMPOS.VLVENDABRUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVendaBruto).AsQueryable<tbRebimentoResumo>();
                    else entity = entity.OrderByDescending(e => e.vlVendaBruto).AsQueryable<tbRebimentoResumo>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbRebimentoResumo/TbRebimentoResumo
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbRebimentoResumo = new List<dynamic>();
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
                    CollectionTbRebimentoResumo = query.Select(e => new
                    {

                        idRebimentoResumo = e.idRebimentoResumo,
                        cdAdquirente = e.cdAdquirente,
                        cdBandeira = e.cdBandeira,
                        cdTipoProdutoTef = e.cdTipoProdutoTef,
                        cdTerminal = e.cdTerminal,
                        dtVenda = e.dtVenda,
                        vlVendaBruto = e.vlVendaBruto,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbRebimentoResumo = query.Select(e => new
                    {

                        idRebimentoResumo = e.idRebimentoResumo,
                        cdAdquirente = e.cdAdquirente,
                        cdBandeira = e.cdBandeira,
                        cdTipoProdutoTef = e.cdTipoProdutoTef,
                        cdTerminal = e.cdTerminal,
                        dtVenda = e.dtVenda,
                        vlVendaBruto = e.vlVendaBruto,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbRebimentoResumo;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar recebimento resumo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbRebimentoResumo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbRebimentoResumo param)
        {
            try
            {
                _db.tbRebimentoResumos.Add(param);
                _db.SaveChanges();
                return param.idRebimentoResumo;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar recebimento resumo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbRebimentoResumo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRebimentoResumo)
        {
            try
            {
                _db.tbRebimentoResumos.Remove(_db.tbRebimentoResumos.Where(e => e.idRebimentoResumo.Equals(idRebimentoResumo)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar recebimento resumo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera tbRebimentoResumo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRebimentoResumo param)
        {
            try
            {
                tbRebimentoResumo value = _db.tbRebimentoResumos
                        .Where(e => e.idRebimentoResumo.Equals(param.idRebimentoResumo))
                        .First<tbRebimentoResumo>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.idRebimentoResumo != null && param.idRebimentoResumo != value.idRebimentoResumo)
                    value.idRebimentoResumo = param.idRebimentoResumo;
                if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
                    value.cdAdquirente = param.cdAdquirente;
                if (param.cdBandeira != null && param.cdBandeira != value.cdBandeira)
                    value.cdBandeira = param.cdBandeira;
                if (param.cdTipoProdutoTef != null && param.cdTipoProdutoTef != value.cdTipoProdutoTef)
                    value.cdTipoProdutoTef = param.cdTipoProdutoTef;
                if (param.cdTerminal != null && param.cdTerminal != value.cdTerminal)
                    value.cdTerminal = param.cdTerminal;
                if (param.dtVenda != null && param.dtVenda != value.dtVenda)
                    value.dtVenda = param.dtVenda;
                if (param.vlVendaBruto != null && param.vlVendaBruto != value.vlVendaBruto)
                    value.vlVendaBruto = param.vlVendaBruto;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar recebimento resumo" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
