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
    public class GatewayGreenCard
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayGreenCard()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DTACOMPRA = 101,
            DTAVENCIMENTO = 102,
            CNPJ = 103,
            CDAUTORIZADOR = 104,
            VALORTRANSACAO = 105,
            DTARECEBIMENTO = 106,
            IDOPERADORA = 107,
            IDBANDEIRA = 108,
            IDTERMINALLOGICO = 109,

        };

        /// <summary>
        /// Get GreenCard/GreenCard
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<GreenCard> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.GreenCards.AsQueryable<GreenCard>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.ID:
                        Int32 id = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.DTACOMPRA:
                        DateTime dtaCompra = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaCompra.Equals(dtaCompra)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.DTAVENCIMENTO:
                        DateTime dtaVencimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaVencimento.Equals(dtaVencimento)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.VALORTRANSACAO:
                        decimal valorTransacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorTransacao.Equals(valorTransacao)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<GreenCard>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<GreenCard>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.ID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.DTACOMPRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaCompra).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.dtaCompra).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.DTAVENCIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaVencimento).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.dtaVencimento).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.VALORTRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTransacao).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.valorTransacao).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<GreenCard>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<GreenCard>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<GreenCard>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna GreenCard/GreenCard
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionGreenCard = new List<dynamic>();
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
                CollectionGreenCard = query.Select(e => new
                {

                    id = e.id,
                    dtaCompra = e.dtaCompra,
                    dtaVencimento = e.dtaVencimento,
                    cnpj = e.cnpj,
                    cdAutorizador = e.cdAutorizador,
                    valorTransacao = e.valorTransacao,
                    dtaRecebimento = e.dtaRecebimento,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionGreenCard = query.Select(e => new
                {

                    id = e.id,
                    dtaCompra = e.dtaCompra,
                    dtaVencimento = e.dtaVencimento,
                    cnpj = e.cnpj,
                    cdAutorizador = e.cdAutorizador,
                    valorTransacao = e.valorTransacao,
                    dtaRecebimento = e.dtaRecebimento,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionGreenCard;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova GreenCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, GreenCard param)
        {
            _db.GreenCards.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma GreenCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.GreenCards.Remove(_db.GreenCards.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera GreenCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, GreenCard param)
        {
            GreenCard value = _db.GreenCards
                    .Where(e => e.id.Equals(param.id))
                    .First<GreenCard>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.dtaCompra != null && param.dtaCompra != value.dtaCompra)
                value.dtaCompra = param.dtaCompra;
            if (param.dtaVencimento != null && param.dtaVencimento != value.dtaVencimento)
                value.dtaVencimento = param.dtaVencimento;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.cdAutorizador != null && param.cdAutorizador != value.cdAutorizador)
                value.cdAutorizador = param.cdAutorizador;
            if (param.valorTransacao != null && param.valorTransacao != value.valorTransacao)
                value.valorTransacao = param.valorTransacao;
            if (param.dtaRecebimento != null && param.dtaRecebimento != value.dtaRecebimento)
                value.dtaRecebimento = param.dtaRecebimento;
            if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                value.idOperadora = param.idOperadora;
            if (param.idBandeira != null && param.idBandeira != value.idBandeira)
                value.idBandeira = param.idBandeira;
            if (param.idTerminalLogico != null && param.idTerminalLogico != value.idTerminalLogico)
                value.idTerminalLogico = param.idTerminalLogico;
            _db.SaveChanges();

        }

    }
}
