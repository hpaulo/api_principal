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
    public class GatewayBaneseCard
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayBaneseCard()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            OPERACAO = 101,
            DTAVENDA = 102,
            NSU = 103,
            MODALIDADE = 104,
            TOTALPARCELAS = 105,
            VALORBRUTO = 106,
            VALORLIQUIDO = 107,
            CNPJ = 108,
            IDOPERADORA = 109,
            IDBANDEIRA = 110,
            DTARECEBIMENTO = 111,
            IDTERMINALLOGICO = 112,

        };

        /// <summary>
        /// Get BaneseCard/BaneseCard
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<BaneseCard> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.BaneseCards.AsQueryable<BaneseCard>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.OPERACAO:
                        string operacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.operacao.Equals(operacao)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.DTAVENDA:
                        DateTime dtaVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaVenda.Equals(dtaVenda)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nsu.Equals(nsu)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.MODALIDADE:
                        string modalidade = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.modalidade.Equals(modalidade)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.TOTALPARCELAS:
                        Int32 totalParcelas = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.totalParcelas.Equals(totalParcelas)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.VALORBRUTO:
                        decimal valorBruto = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorBruto.Equals(valorBruto)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.VALORLIQUIDO:
                        decimal valorLiquido = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorLiquido.Equals(valorLiquido)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<BaneseCard>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<BaneseCard>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.OPERACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.operacao).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.operacao).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.DTAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaVenda).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.dtaVenda).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.NSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nsu).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.nsu).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.MODALIDADE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.modalidade).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.modalidade).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.TOTALPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.totalParcelas).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.totalParcelas).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.VALORBRUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorBruto).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.valorBruto).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.VALORLIQUIDO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorLiquido).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.valorLiquido).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<BaneseCard>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<BaneseCard>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<BaneseCard>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna BaneseCard/BaneseCard
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionBaneseCard = new List<dynamic>();
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
                CollectionBaneseCard = query.Select(e => new
                {

                    id = e.id,
                    operacao = e.operacao,
                    dtaVenda = e.dtaVenda,
                    nsu = e.nsu,
                    modalidade = e.modalidade,
                    totalParcelas = e.totalParcelas,
                    valorBruto = e.valorBruto,
                    valorLiquido = e.valorLiquido,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionBaneseCard = query.Select(e => new
                {

                    id = e.id,
                    operacao = e.operacao,
                    dtaVenda = e.dtaVenda,
                    nsu = e.nsu,
                    modalidade = e.modalidade,
                    totalParcelas = e.totalParcelas,
                    valorBruto = e.valorBruto,
                    valorLiquido = e.valorLiquido,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionBaneseCard;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova BaneseCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, BaneseCard param)
        {
            _db.BaneseCards.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma BaneseCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.BaneseCards.Remove(_db.BaneseCards.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera BaneseCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, BaneseCard param)
        {
            BaneseCard value = _db.BaneseCards
                    .Where(e => e.id.Equals(param.id))
                    .First<BaneseCard>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.operacao != null && param.operacao != value.operacao)
                value.operacao = param.operacao;
            if (param.dtaVenda != null && param.dtaVenda != value.dtaVenda)
                value.dtaVenda = param.dtaVenda;
            if (param.nsu != null && param.nsu != value.nsu)
                value.nsu = param.nsu;
            if (param.modalidade != null && param.modalidade != value.modalidade)
                value.modalidade = param.modalidade;
            if (param.totalParcelas != null && param.totalParcelas != value.totalParcelas)
                value.totalParcelas = param.totalParcelas;
            if (param.valorBruto != null && param.valorBruto != value.valorBruto)
                value.valorBruto = param.valorBruto;
            if (param.valorLiquido != null && param.valorLiquido != value.valorLiquido)
                value.valorLiquido = param.valorLiquido;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                value.idOperadora = param.idOperadora;
            if (param.idBandeira != null && param.idBandeira != value.idBandeira)
                value.idBandeira = param.idBandeira;
            if (param.dtaRecebimento != null && param.dtaRecebimento != value.dtaRecebimento)
                value.dtaRecebimento = param.dtaRecebimento;
            if (param.idTerminalLogico != null && param.idTerminalLogico != value.idTerminalLogico)
                value.idTerminalLogico = param.idTerminalLogico;
            _db.SaveChanges();

        }

    }
}
