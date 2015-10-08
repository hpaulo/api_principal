using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Card
{
    public class GatewayTbRecebimentoResumoManual
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbRecebimentoResumoManual()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDRECEBIMENTORESUMOMANUAL = 100,
            CDTERMINALLOGICO = 101,
            CDADQUIRENTE = 102,
            DTVENDA = 103,
            VLVENDA = 104,
            QTTRACACAO = 105,
            TPOPERACAO = 106,
            CDBANDEIRA = 107,

        };

        /// <summary>
        /// Get TbRecebimentoResumoManual/TbRecebimentoResumoManual
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbRecebimentoResumoManual> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbRecebimentoResumoManuals.AsQueryable<tbRecebimentoResumoManual>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.IDRECEBIMENTORESUMOMANUAL:
                        Int32 idRecebimentoResumoManual = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idRecebimentoResumoManual.Equals(idRecebimentoResumoManual)).AsQueryable<tbRecebimentoResumoManual>();
                        break;
                    case CAMPOS.CDTERMINALLOGICO:
                        string cdTerminalLogico = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdTerminalLogico.Equals(cdTerminalLogico)).AsQueryable<tbRecebimentoResumoManual>();
                        break;
                    case CAMPOS.CDADQUIRENTE:
                        Int32 cdAdquirente = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdAdquirente.Equals(cdAdquirente)).AsQueryable<tbRecebimentoResumoManual>();
                        break;
                    case CAMPOS.DTVENDA:
                        DateTime dtVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtVenda.Equals(dtVenda)).AsQueryable<tbRecebimentoResumoManual>();
                        break;
                    case CAMPOS.VLVENDA:
                        decimal vlVenda = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlVenda.Equals(vlVenda)).AsQueryable<tbRecebimentoResumoManual>();
                        break;
                    case CAMPOS.QTTRACACAO:
                        byte qtTracacao = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.qtTracacao.Equals(qtTracacao)).AsQueryable<tbRecebimentoResumoManual>();
                        break;
                    case CAMPOS.TPOPERACAO:
                        byte tpOperacao = Convert.ToByte(item.Value);
                        entity = entity.Where(e => e.tpOperacao.Equals(tpOperacao)).AsQueryable<tbRecebimentoResumoManual>();
                        break;
                    case CAMPOS.CDBANDEIRA:
                        Int32 cdBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdBandeira.Equals(cdBandeira)).AsQueryable<tbRecebimentoResumoManual>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.IDRECEBIMENTORESUMOMANUAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idRecebimentoResumoManual).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.idRecebimentoResumoManual).AsQueryable<tbRecebimentoResumoManual>();
                    break;
                case CAMPOS.CDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdTerminalLogico).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.cdTerminalLogico).AsQueryable<tbRecebimentoResumoManual>();
                    break;
                case CAMPOS.CDADQUIRENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAdquirente).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.cdAdquirente).AsQueryable<tbRecebimentoResumoManual>();
                    break;
                case CAMPOS.DTVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtVenda).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.dtVenda).AsQueryable<tbRecebimentoResumoManual>();
                    break;
                case CAMPOS.VLVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlVenda).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.vlVenda).AsQueryable<tbRecebimentoResumoManual>();
                    break;
                case CAMPOS.QTTRACACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtTracacao).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.qtTracacao).AsQueryable<tbRecebimentoResumoManual>();
                    break;
                case CAMPOS.TPOPERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tpOperacao).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.tpOperacao).AsQueryable<tbRecebimentoResumoManual>();
                    break;
                case CAMPOS.CDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdBandeira).AsQueryable<tbRecebimentoResumoManual>();
                    else entity = entity.OrderByDescending(e => e.cdBandeira).AsQueryable<tbRecebimentoResumoManual>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbRecebimentoResumoManual/TbRecebimentoResumoManual
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTbRecebimentoResumoManual = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);


            // TOTAL DE REGISTROS
            retorno.TotalDeRegistros = query.Count();


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
                CollectionTbRecebimentoResumoManual = query.Select(e => new
                {

                    idRecebimentoResumoManual = e.idRecebimentoResumoManual,
                    cdTerminalLogico = e.cdTerminalLogico,
                    cdAdquirente = e.cdAdquirente,
                    dtVenda = e.dtVenda,
                    vlVenda = e.vlVenda,
                    qtTracacao = e.qtTracacao,
                    tpOperacao = e.tpOperacao,
                    cdBandeira = e.cdBandeira,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbRecebimentoResumoManual = query.Select(e => new
                {

                    idRecebimentoResumoManual = e.idRecebimentoResumoManual,
                    cdTerminalLogico = e.cdTerminalLogico,
                    cdAdquirente = e.cdAdquirente,
                    dtVenda = e.dtVenda,
                    vlVenda = e.vlVenda,
                    qtTracacao = e.qtTracacao,
                    tpOperacao = e.tpOperacao,
                    cdBandeira = e.cdBandeira,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbRecebimentoResumoManual;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova TbRecebimentoResumoManual
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbRecebimentoResumoManual param)
        {
            _db.tbRecebimentoResumoManuals.Add(param);
            _db.SaveChanges();
            return param.idRecebimentoResumoManual;
        }


        /// <summary>
        /// Apaga uma TbRecebimentoResumoManual
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idRecebimentoResumoManual)
        {
            _db.tbRecebimentoResumoManuals.Remove(_db.tbRecebimentoResumoManuals.Where(e => e.idRecebimentoResumoManual.Equals(idRecebimentoResumoManual)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera tbRecebimentoResumoManual
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbRecebimentoResumoManual param)
        {
            tbRecebimentoResumoManual value = _db.tbRecebimentoResumoManuals
                    .Where(e => e.idRecebimentoResumoManual.Equals(param.idRecebimentoResumoManual))
                    .First<tbRecebimentoResumoManual>();


            if (param.idRecebimentoResumoManual != value.idRecebimentoResumoManual)
                value.idRecebimentoResumoManual = param.idRecebimentoResumoManual;
            if (param.cdTerminalLogico != null && param.cdTerminalLogico != value.cdTerminalLogico)
                value.cdTerminalLogico = param.cdTerminalLogico;
            if (param.cdAdquirente != null && param.cdAdquirente != value.cdAdquirente)
                value.cdAdquirente = param.cdAdquirente;
            if (param.dtVenda != null && param.dtVenda != value.dtVenda)
                value.dtVenda = param.dtVenda;
            if (param.vlVenda != value.vlVenda)
                value.vlVenda = param.vlVenda;
            if (param.qtTracacao != null && param.qtTracacao != value.qtTracacao)
                value.qtTracacao = param.qtTracacao;
            if (param.tpOperacao != value.tpOperacao)
                value.tpOperacao = param.tpOperacao;
            if (param.cdBandeira != null && param.cdBandeira != value.cdBandeira)
                value.cdBandeira = param.cdBandeira;
            _db.SaveChanges();

        }

    }
}