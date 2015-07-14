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
    public class GatewayTbRecebimentoTEF
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRecebimentoTEF()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTOTEF = 100,
            CDGRUPO = 101,
            NRCNPJ = 102,
            CDESTABELECIMENTOTEF = 103,
            NRPDVTEF = 104,
            NRNSU = 105,
            CDAUTORIZACAO = 106,
            CDSITEF = 107,
            DTVENDA = 108,
            HRVENDA = 109,
            VLVENDA = 110,
            NRCARTAO = 111,
            CDBANDEIRA = 112,
            NMOPERADORA = 113,

        };

        /// <summary>
        /// Get TbRecebimentoTEF /TbRecebimentoTEF 
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbRecebimentoTEF> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRecebimentoTEFs.AsQueryable<tbRecebimentoTEF>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDRECEBIMENTOTEF:
                        Int32 idRecebimentoTEF = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimentoTEF.Equals(idRecebimentoTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDGRUPO:
                        byte cdGrupo = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.cdGrupo.Equals(cdGrupo)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRCNPJ:
                        string nrCNPJ = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCNPJ.Equals(nrCNPJ)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDESTABELECIMENTOTEF:
                        string cdEstabelecimentoTEF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdEstabelecimentoTEF.Equals(cdEstabelecimentoTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRPDVTEF:
                        string nrPDVTEF = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrPDVTEF.Equals(nrPDVTEF)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRNSU:
                        string nrNSU = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrNSU.Equals(nrNSU)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDAUTORIZACAO:
                        string cdAutorizacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizacao.Equals(cdAutorizacao)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDSITEF:
                        Int32 cdSitef = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdSitef.Equals(cdSitef)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.DTVENDA:
                        DateTime dtVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtVenda.Equals(dtVenda)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.HRVENDA:
                        TimeSpan hrVenda = TimeSpan.Parse(item.Value);
                        entity = entity.Where(e => e.hrVenda.Equals(hrVenda)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.VLVENDA:
                        decimal vlVenda = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVenda.Equals(vlVenda)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NRCARTAO:
                        string nrCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nrCartao.Equals(nrCartao)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int16 cdBandeira = Convert.ToInt16(item.Value);
                        entity = entity.Where(e => e.cdBandeira.Equals(cdBandeira)).AsQueryable<tbRecebimentoTEF>();
                        break;
                    case CAMPOS.NMOPERADORA:
                        string nmOperadora = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nmOperadora.Equals(nmOperadora)).AsQueryable<tbRecebimentoTEF>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDRECEBIMENTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimentoTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.idRecebimentoTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDGRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdGrupo).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdGrupo).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRCNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCNPJ).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrCNPJ).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDESTABELECIMENTOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEstabelecimentoTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdEstabelecimentoTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRPDVTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrPDVTEF).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrPDVTEF).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRNSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrNSU).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrNSU).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDAUTORIZACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizacao).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizacao).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDSITEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdSitef).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdSitef).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVenda).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.dtVenda).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.HRVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.hrVenda).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.hrVenda).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.VLVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVenda).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.vlVenda).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NRCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nrCartao).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nrCartao).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbRecebimentoTEF>();
                    break;
                case CAMPOS.NMOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nmOperadora).AsQueryable<tbRecebimentoTEF>();
                    else entity = entity.OrderByDescending(e => e.nmOperadora).AsQueryable<tbRecebimentoTEF>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbRecebimentoTEF /TbRecebimentoTEF 
        /// </summary>
        /// <returns></returns> 
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbRecebimentoTEF = new List<dynamic>();
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
                CollectionTbRecebimentoTEF = query.Select(e => new
                {

                    idRecebimentoTEF = e.idRecebimentoTEF,
                    cdGrupo = e.cdGrupo,
                    nrCNPJ = e.nrCNPJ,
                    cdEstabelecimentoTEF = e.cdEstabelecimentoTEF,
                    nrPDVTEF = e.nrPDVTEF,
                    nrNSU = e.nrNSU,
                    cdAutorizacao = e.cdAutorizacao,
                    cdSitef = e.cdSitef,
                    dtVenda = e.dtVenda,
                    hrVenda = e.hrVenda,
                    vlVenda = e.vlVenda,
                    nrCartao = e.nrCartao,
                    cdBandeira = e.cdBandeira,
                    nmOperadora = e.nmOperadora,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbRecebimentoTEF = query.Select(e => new
                {

                    idRecebimentoTEF = e.idRecebimentoTEF,
                    cdGrupo = e.cdGrupo,
                    nrCNPJ = e.nrCNPJ,
                    cdEstabelecimentoTEF = e.cdEstabelecimentoTEF,
                    nrPDVTEF = e.nrPDVTEF,
                    nrNSU = e.nrNSU,
                    cdAutorizacao = e.cdAutorizacao,
                    cdSitef = e.cdSitef,
                    dtVenda = e.dtVenda,
                    hrVenda = e.hrVenda,
                    vlVenda = e.vlVenda,
                    nrCartao = e.nrCartao,
                    cdBandeira = e.cdBandeira,
                    nmOperadora = e.nmOperadora,
                }).ToList<dynamic>();
            }
            else if (colecao == 2)
            {
                CollectionTbRecebimentoTEF = query
                    .GroupBy(x => new { x.cdGrupo })
                    .Select(e => new
                {
                    cdGrupo = e.Key.cdGrupo,
                    nmGrupo = _db.grupo_empresa.Where( g => g.id_grupo == e.Key.cdGrupo).Select( g => g.ds_nome).FirstOrDefault(),
                    dtVenda = (e.Max(p => p.dthrVenda )),
                }).ToList<dynamic>();

                // TOTAL DE REGISTROS
                retorno.TotalDeRegistros = CollectionTbRecebimentoTEF.Count();


                // PAGINAÇÃO
                skipRows = (pageNumber - 1) * pageSize;
                if (retorno.TotalDeRegistros > pageSize && pageNumber > 0 && pageSize > 0)
                    query = query.Skip(skipRows).Take(pageSize);
                else
                    pageNumber = 1;

                retorno.PaginaAtual = pageNumber;
                retorno.ItensPorPagina = pageSize;
 
            }

            retorno.Registros = CollectionTbRecebimentoTEF;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TbRecebimentoTEF 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbRecebimentoTEF param)
        {
            _db.tbRecebimentoTEFs.Add(param);
            _db.SaveChanges();
            return param.idRecebimentoTEF;
        }


        /// <summary>
        /// Apaga uma TbRecebimentoTEF 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimentoTEF)
        {
            _db.tbRecebimentoTEFs.Remove(_db.tbRecebimentoTEFs.Where(e => e.idRecebimentoTEF.Equals(idRecebimentoTEF)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera tbRecebimentoTEF 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRecebimentoTEF param)
        {
            tbRecebimentoTEF value = _db.tbRecebimentoTEFs
                    .Where(e => e.idRecebimentoTEF.Equals(param.idRecebimentoTEF))
                    .First<tbRecebimentoTEF>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.idRecebimentoTEF != null && param.idRecebimentoTEF != value.idRecebimentoTEF)
                value.idRecebimentoTEF = param.idRecebimentoTEF;
            if (param.cdGrupo != null && param.cdGrupo != value.cdGrupo)
                value.cdGrupo = param.cdGrupo;
            if (param.nrCNPJ != null && param.nrCNPJ != value.nrCNPJ)
                value.nrCNPJ = param.nrCNPJ;
            if (param.cdEstabelecimentoTEF != null && param.cdEstabelecimentoTEF != value.cdEstabelecimentoTEF)
                value.cdEstabelecimentoTEF = param.cdEstabelecimentoTEF;
            if (param.nrPDVTEF != null && param.nrPDVTEF != value.nrPDVTEF)
                value.nrPDVTEF = param.nrPDVTEF;
            if (param.nrNSU != null && param.nrNSU != value.nrNSU)
                value.nrNSU = param.nrNSU;
            if (param.cdAutorizacao != null && param.cdAutorizacao != value.cdAutorizacao)
                value.cdAutorizacao = param.cdAutorizacao;
            if (param.cdSitef != null && param.cdSitef != value.cdSitef)
                value.cdSitef = param.cdSitef;
            if (param.dtVenda != null && param.dtVenda != value.dtVenda)
                value.dtVenda = param.dtVenda;
            if (param.hrVenda != null && param.hrVenda != value.hrVenda)
                value.hrVenda = param.hrVenda;
            if (param.vlVenda != null && param.vlVenda != value.vlVenda)
                value.vlVenda = param.vlVenda;
            if (param.nrCartao != null && param.nrCartao != value.nrCartao)
                value.nrCartao = param.nrCartao;
            if (param.cdBandeira != null && param.cdBandeira != value.cdBandeira)
                value.cdBandeira = param.cdBandeira;
            if (param.nmOperadora != null && param.nmOperadora != value.nmOperadora)
                value.nmOperadora = param.nmOperadora;
            _db.SaveChanges();

        }

    }
}
