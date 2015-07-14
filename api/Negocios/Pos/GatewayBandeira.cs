using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Pos
{
    public class GatewayBandeira
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayBandeira()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDBANDEIRA = 100,
            DESCRICAOBANDEIRA = 101,
            IDGRUPO = 102,
            CODBANDEIRAERP = 103,
            CODBANDEIRAHOSTPAGAMENTO = 104,
            TAXAADMINISTRACAO = 105,
            IDTIPOPAGAMENTO = 106,
            SACADO = 107,

        };

        /// <summary>
        /// Get Bandeiras/Bandeiras
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Bandeira> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Bandeiras.AsQueryable<Bandeira>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDBANDEIRA:
                        Int32 IdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.IdBandeira.Equals(IdBandeira)).AsQueryable<Bandeira>();
                        break;
                    case CAMPOS.DESCRICAOBANDEIRA:
                        string DescricaoBandeira = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.DescricaoBandeira.Equals(DescricaoBandeira)).AsQueryable<Bandeira>();
                        break;
                    case CAMPOS.IDGRUPO:
                        Int32 IdGrupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.IdGrupo.Equals(IdGrupo)).AsQueryable<Bandeira>();
                        break;
                    case CAMPOS.CODBANDEIRAERP:
                        string CodBandeiraERP = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.CodBandeiraERP.Equals(CodBandeiraERP)).AsQueryable<Bandeira>();
                        break;
                    case CAMPOS.CODBANDEIRAHOSTPAGAMENTO:
                        decimal CodBandeiraHostPagamento = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.CodBandeiraHostPagamento.Equals(CodBandeiraHostPagamento)).AsQueryable<Bandeira>();
                        break;
                    case CAMPOS.TAXAADMINISTRACAO:
                        decimal TaxaAdministracao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.TaxaAdministracao.Equals(TaxaAdministracao)).AsQueryable<Bandeira>();
                        break;
                    case CAMPOS.IDTIPOPAGAMENTO:
                        Int32 IdTipoPagamento = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.IdTipoPagamento.Equals(IdTipoPagamento)).AsQueryable<Bandeira>();
                        break;
                    case CAMPOS.SACADO:
                        string Sacado = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.Sacado.Equals(Sacado)).AsQueryable<Bandeira>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.IdBandeira).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.IdBandeira).AsQueryable<Bandeira>();
                    break;
                case CAMPOS.DESCRICAOBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.DescricaoBandeira).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.DescricaoBandeira).AsQueryable<Bandeira>();
                    break;
                case CAMPOS.IDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.IdGrupo).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.IdGrupo).AsQueryable<Bandeira>();
                    break;
                case CAMPOS.CODBANDEIRAERP:
                    if (orderby == 0) entity = entity.OrderBy(e => e.CodBandeiraERP).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.CodBandeiraERP).AsQueryable<Bandeira>();
                    break;
                case CAMPOS.CODBANDEIRAHOSTPAGAMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.CodBandeiraHostPagamento).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.CodBandeiraHostPagamento).AsQueryable<Bandeira>();
                    break;
                case CAMPOS.TAXAADMINISTRACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.TaxaAdministracao).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.TaxaAdministracao).AsQueryable<Bandeira>();
                    break;
                case CAMPOS.IDTIPOPAGAMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.IdTipoPagamento).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.IdTipoPagamento).AsQueryable<Bandeira>();
                    break;
                case CAMPOS.SACADO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Sacado).AsQueryable<Bandeira>();
                    else entity = entity.OrderByDescending(e => e.Sacado).AsQueryable<Bandeira>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Bandeiras/Bandeiras
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionBandeiras = new List<dynamic>();
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
                CollectionBandeiras = query.Select(e => new
                {

                    IdBandeira = e.IdBandeira,
                    DescricaoBandeira = e.DescricaoBandeira,
                    IdGrupo = e.IdGrupo,
                    CodBandeiraERP = e.CodBandeiraERP,
                    CodBandeiraHostPagamento = e.CodBandeiraHostPagamento,
                    TaxaAdministracao = e.TaxaAdministracao,
                    IdTipoPagamento = e.IdTipoPagamento,
                    Sacado = e.Sacado,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionBandeiras = query.Select(e => new
                {

                    IdBandeira = e.IdBandeira,
                    DescricaoBandeira = e.DescricaoBandeira,
                    IdGrupo = e.IdGrupo,
                    CodBandeiraERP = e.CodBandeiraERP,
                    CodBandeiraHostPagamento = e.CodBandeiraHostPagamento,
                    TaxaAdministracao = e.TaxaAdministracao,
                    IdTipoPagamento = e.IdTipoPagamento,
                    Sacado = e.Sacado,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionBandeiras;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Bandeiras
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Bandeira param)
        {
            _db.Bandeiras.Add(param);
            _db.SaveChanges();
            return param.IdBandeira;
        }


        /// <summary>
        /// Apaga uma Bandeiras
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 IdBandeira)
        {
            _db.Bandeiras.Remove(_db.Bandeiras.Where(e => e.IdBandeira.Equals(IdBandeira)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera Bandeiras
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Bandeira param)
        {
            Bandeira value = _db.Bandeiras
                    .Where(e => e.IdBandeira.Equals(param.IdBandeira))
                    .First<Bandeira>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.IdBandeira != null && param.IdBandeira != value.IdBandeira)
                value.IdBandeira = param.IdBandeira;
            if (param.DescricaoBandeira != null && param.DescricaoBandeira != value.DescricaoBandeira)
                value.DescricaoBandeira = param.DescricaoBandeira;
            if (param.IdGrupo != null && param.IdGrupo != value.IdGrupo)
                value.IdGrupo = param.IdGrupo;
            if (param.CodBandeiraERP != null && param.CodBandeiraERP != value.CodBandeiraERP)
                value.CodBandeiraERP = param.CodBandeiraERP;
            if (param.CodBandeiraHostPagamento != null && param.CodBandeiraHostPagamento != value.CodBandeiraHostPagamento)
                value.CodBandeiraHostPagamento = param.CodBandeiraHostPagamento;
            if (param.TaxaAdministracao != null && param.TaxaAdministracao != value.TaxaAdministracao)
                value.TaxaAdministracao = param.TaxaAdministracao;
            if (param.IdTipoPagamento != null && param.IdTipoPagamento != value.IdTipoPagamento)
                value.IdTipoPagamento = param.IdTipoPagamento;
            if (param.Sacado != null && param.Sacado != value.Sacado)
                value.Sacado = param.Sacado;
            _db.SaveChanges();

        }

    }
}
