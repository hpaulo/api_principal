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
    public class GatewayRedeCard
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayRedeCard()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            NSU = 101,
            NUMCARTAO = 102,
            DTAVENDA = 103,
            VALORBRUTO = 104,
            TOTALPARCELAS = 105,
            ESTABELECIMENTO = 106,
            TIPOCAPTURA = 107,
            VENDACANCELADA = 108,
            CNPJ = 109,
            IDOPERADORA = 110,
            IDBANDEIRA = 111,
            DTARECEBIMENTO = 112,
            IDLOGICOTERMINAL = 113,
            TIPOVENDA = 114,
            TAXAADMINISTRACAO = 115,
            CODRESUMOVENDA = 116,

        };

        /// <summary>
        /// Get RedeCard/RedeCard
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<RedeCard> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.RedeCards.AsQueryable<RedeCard>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nsu.Equals(nsu)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.DTAVENDA:
                        DateTime dtaVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaVenda.Equals(dtaVenda)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.VALORBRUTO:
                        decimal valorBruto = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorBruto.Equals(valorBruto)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.TOTALPARCELAS:
                        Int32 totalParcelas = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.totalParcelas.Equals(totalParcelas)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.ESTABELECIMENTO:
                        string estabelecimento = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.estabelecimento.Equals(estabelecimento)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.TIPOCAPTURA:
                        string tipoCaptura = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tipoCaptura.Equals(tipoCaptura)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.VENDACANCELADA:
                        string vendaCancelada = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.vendaCancelada.Equals(vendaCancelada)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.IDLOGICOTERMINAL:
                        Int32 idLogicoTerminal = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogicoTerminal.Equals(idLogicoTerminal)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.TIPOVENDA:
                        string tipoVenda = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tipoVenda.Equals(tipoVenda)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.TAXAADMINISTRACAO:
                        decimal taxaAdministracao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.taxaAdministracao.Equals(taxaAdministracao)).AsQueryable<RedeCard>();
                        break;
                    case CAMPOS.CODRESUMOVENDA:
                        string codResumoVenda = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.codResumoVenda.Equals(codResumoVenda)).AsQueryable<RedeCard>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.NSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nsu).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.nsu).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.DTAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaVenda).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.dtaVenda).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.VALORBRUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorBruto).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.valorBruto).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.TOTALPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.totalParcelas).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.totalParcelas).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.ESTABELECIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.estabelecimento).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.estabelecimento).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.TIPOCAPTURA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tipoCaptura).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.tipoCaptura).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.VENDACANCELADA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vendaCancelada).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.vendaCancelada).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.IDLOGICOTERMINAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogicoTerminal).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.idLogicoTerminal).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.TIPOVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tipoVenda).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.tipoVenda).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.TAXAADMINISTRACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.taxaAdministracao).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.taxaAdministracao).AsQueryable<RedeCard>();
                    break;
                case CAMPOS.CODRESUMOVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codResumoVenda).AsQueryable<RedeCard>();
                    else entity = entity.OrderByDescending(e => e.codResumoVenda).AsQueryable<RedeCard>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna RedeCard/RedeCard
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionRedeCard = new List<dynamic>();
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
                CollectionRedeCard = query.Select(e => new
                {

                    id = e.id,
                    nsu = e.nsu,
                    numCartao = e.numCartao,
                    dtaVenda = e.dtaVenda,
                    valorBruto = e.valorBruto,
                    totalParcelas = e.totalParcelas,
                    estabelecimento = e.estabelecimento,
                    tipoCaptura = e.tipoCaptura,
                    vendaCancelada = e.vendaCancelada,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idLogicoTerminal = e.idLogicoTerminal,
                    tipoVenda = e.tipoVenda,
                    taxaAdministracao = e.taxaAdministracao,
                    codResumoVenda = e.codResumoVenda,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionRedeCard = query.Select(e => new
                {

                    id = e.id,
                    nsu = e.nsu,
                    numCartao = e.numCartao,
                    dtaVenda = e.dtaVenda,
                    valorBruto = e.valorBruto,
                    totalParcelas = e.totalParcelas,
                    estabelecimento = e.estabelecimento,
                    tipoCaptura = e.tipoCaptura,
                    vendaCancelada = e.vendaCancelada,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idLogicoTerminal = e.idLogicoTerminal,
                    tipoVenda = e.tipoVenda,
                    taxaAdministracao = e.taxaAdministracao,
                    codResumoVenda = e.codResumoVenda,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionRedeCard;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova RedeCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, RedeCard param)
        {
            _db.RedeCards.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma RedeCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.RedeCards.Remove(_db.RedeCards.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera RedeCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, RedeCard param)
        {
            RedeCard value = _db.RedeCards
                    .Where(e => e.id.Equals(param.id))
                    .First<RedeCard>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.nsu != null && param.nsu != value.nsu)
                value.nsu = param.nsu;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.dtaVenda != null && param.dtaVenda != value.dtaVenda)
                value.dtaVenda = param.dtaVenda;
            if (param.valorBruto != null && param.valorBruto != value.valorBruto)
                value.valorBruto = param.valorBruto;
            if (param.totalParcelas != null && param.totalParcelas != value.totalParcelas)
                value.totalParcelas = param.totalParcelas;
            if (param.estabelecimento != null && param.estabelecimento != value.estabelecimento)
                value.estabelecimento = param.estabelecimento;
            if (param.tipoCaptura != null && param.tipoCaptura != value.tipoCaptura)
                value.tipoCaptura = param.tipoCaptura;
            if (param.vendaCancelada != null && param.vendaCancelada != value.vendaCancelada)
                value.vendaCancelada = param.vendaCancelada;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                value.idOperadora = param.idOperadora;
            if (param.idBandeira != null && param.idBandeira != value.idBandeira)
                value.idBandeira = param.idBandeira;
            if (param.dtaRecebimento != null && param.dtaRecebimento != value.dtaRecebimento)
                value.dtaRecebimento = param.dtaRecebimento;
            if (param.idLogicoTerminal != null && param.idLogicoTerminal != value.idLogicoTerminal)
                value.idLogicoTerminal = param.idLogicoTerminal;
            if (param.tipoVenda != null && param.tipoVenda != value.tipoVenda)
                value.tipoVenda = param.tipoVenda;
            if (param.taxaAdministracao != null && param.taxaAdministracao != value.taxaAdministracao)
                value.taxaAdministracao = param.taxaAdministracao;
            if (param.codResumoVenda != null && param.codResumoVenda != value.codResumoVenda)
                value.codResumoVenda = param.codResumoVenda;
            _db.SaveChanges();

        }

    }
}
