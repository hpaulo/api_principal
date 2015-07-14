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
    public class GatewayGoodCard
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayGoodCard()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            LANCAMENTO = 101,
            DTAHORA = 102,
            QTDTOTALPARCELAS = 103,
            LOTE = 104,
            CNPJ = 105,
            NUMCARTAO = 106,
            REDECAPTURA = 107,
            VALORTRANSACAO = 108,
            VALORREEMBOLSO = 109,
            VALORDESCONTADO = 110,
            DTARECEBIMENTO = 111,
            IDOPERADORA = 112,
            IDBANDEIRA = 113,
            IDTERMINALLOGICO = 114,

        };

        /// <summary>
        /// Get GoodCard/GoodCard
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<GoodCard> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.GoodCards.AsQueryable<GoodCard>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.LANCAMENTO:
                        string lancamento = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.lancamento.Equals(lancamento)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.DTAHORA:
                        DateTime dtaHora = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaHora.Equals(dtaHora)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.QTDTOTALPARCELAS:
                        Int32 qtdTotalParcelas = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.qtdTotalParcelas.Equals(qtdTotalParcelas)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.LOTE:
                        string lote = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.lote.Equals(lote)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.REDECAPTURA:
                        string redeCaptura = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.redeCaptura.Equals(redeCaptura)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.VALORTRANSACAO:
                        decimal valorTransacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorTransacao.Equals(valorTransacao)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.VALORREEMBOLSO:
                        decimal valorReembolso = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorReembolso.Equals(valorReembolso)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.VALORDESCONTADO:
                        decimal valorDescontado = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorDescontado.Equals(valorDescontado)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<GoodCard>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<GoodCard>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.LANCAMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.lancamento).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.lancamento).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.DTAHORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaHora).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.dtaHora).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.QTDTOTALPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtdTotalParcelas).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.qtdTotalParcelas).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.LOTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.lote).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.lote).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.REDECAPTURA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.redeCaptura).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.redeCaptura).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.VALORTRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTransacao).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.valorTransacao).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.VALORREEMBOLSO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorReembolso).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.valorReembolso).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.VALORDESCONTADO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorDescontado).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.valorDescontado).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<GoodCard>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<GoodCard>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<GoodCard>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna GoodCard/GoodCard
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionGoodCard = new List<dynamic>();
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
                CollectionGoodCard = query.Select(e => new
                {

                    id = e.id,
                    lancamento = e.lancamento,
                    dtaHora = e.dtaHora,
                    qtdTotalParcelas = e.qtdTotalParcelas,
                    lote = e.lote,
                    cnpj = e.cnpj,
                    numCartao = e.numCartao,
                    redeCaptura = e.redeCaptura,
                    valorTransacao = e.valorTransacao,
                    valorReembolso = e.valorReembolso,
                    valorDescontado = e.valorDescontado,
                    dtaRecebimento = e.dtaRecebimento,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionGoodCard = query.Select(e => new
                {

                    id = e.id,
                    lancamento = e.lancamento,
                    dtaHora = e.dtaHora,
                    qtdTotalParcelas = e.qtdTotalParcelas,
                    lote = e.lote,
                    cnpj = e.cnpj,
                    numCartao = e.numCartao,
                    redeCaptura = e.redeCaptura,
                    valorTransacao = e.valorTransacao,
                    valorReembolso = e.valorReembolso,
                    valorDescontado = e.valorDescontado,
                    dtaRecebimento = e.dtaRecebimento,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionGoodCard;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova GoodCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, GoodCard param)
        {
            _db.GoodCards.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma GoodCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.GoodCards.Remove(_db.GoodCards.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera GoodCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, GoodCard param)
        {
            GoodCard value = _db.GoodCards
                    .Where(e => e.id.Equals(param.id))
                    .First<GoodCard>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.lancamento != null && param.lancamento != value.lancamento)
                value.lancamento = param.lancamento;
            if (param.dtaHora != null && param.dtaHora != value.dtaHora)
                value.dtaHora = param.dtaHora;
            if (param.qtdTotalParcelas != null && param.qtdTotalParcelas != value.qtdTotalParcelas)
                value.qtdTotalParcelas = param.qtdTotalParcelas;
            if (param.lote != null && param.lote != value.lote)
                value.lote = param.lote;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.redeCaptura != null && param.redeCaptura != value.redeCaptura)
                value.redeCaptura = param.redeCaptura;
            if (param.valorTransacao != null && param.valorTransacao != value.valorTransacao)
                value.valorTransacao = param.valorTransacao;
            if (param.valorReembolso != null && param.valorReembolso != value.valorReembolso)
                value.valorReembolso = param.valorReembolso;
            if (param.valorDescontado != null && param.valorDescontado != value.valorDescontado)
                value.valorDescontado = param.valorDescontado;
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
