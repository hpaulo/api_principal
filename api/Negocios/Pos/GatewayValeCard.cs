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
    public class GatewayValeCard
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayValeCard()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DESCRICAO = 101,
            COMPRADOR = 102,
            CD_AUTORIZADOR = 103,
            DATA = 104,
            VALOR = 105,
            CNPJ = 106,
            PARCELATOTAL = 107,
            TERMINAL = 108,
            IDOPERADORA = 109,
            IDBANDEIRA = 110,
            DATA_RECEBIMENTO = 111,
            IDTERMINALLOGICO = 112,

        };

        /// <summary>
        /// Get ValeCard/ValeCard
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<ValeCard> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.ValeCards.AsQueryable<ValeCard>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.DESCRICAO:
                        string descricao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.descricao.Equals(descricao)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.COMPRADOR:
                        string comprador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.comprador.Equals(comprador)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.CD_AUTORIZADOR:
                        string cd_autorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cd_autorizador.Equals(cd_autorizador)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.DATA:
                        DateTime data = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data.Equals(data)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.VALOR:
                        decimal valor = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valor.Equals(valor)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.PARCELATOTAL:
                        string parcelaTotal = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.parcelaTotal.Equals(parcelaTotal)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.TERMINAL:
                        string terminal = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.terminal.Equals(terminal)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.DATA_RECEBIMENTO:
                        DateTime data_recebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data_recebimento.Equals(data_recebimento)).AsQueryable<ValeCard>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<ValeCard>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.DESCRICAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.descricao).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.descricao).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.COMPRADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.comprador).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.comprador).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.CD_AUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cd_autorizador).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.cd_autorizador).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.DATA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.data).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.VALOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valor).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.valor).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.PARCELATOTAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.parcelaTotal).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.parcelaTotal).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.TERMINAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.terminal).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.terminal).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.DATA_RECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data_recebimento).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.data_recebimento).AsQueryable<ValeCard>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<ValeCard>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<ValeCard>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna ValeCard/ValeCard
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionValeCard = new List<dynamic>();
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
                CollectionValeCard = query.Select(e => new
                {

                    id = e.id,
                    descricao = e.descricao,
                    comprador = e.comprador,
                    cd_autorizador = e.cd_autorizador,
                    data = e.data,
                    valor = e.valor,
                    cnpj = e.cnpj,
                    parcelaTotal = e.parcelaTotal,
                    terminal = e.terminal,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    data_recebimento = e.data_recebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionValeCard = query.Select(e => new
                {

                    id = e.id,
                    descricao = e.descricao,
                    comprador = e.comprador,
                    cd_autorizador = e.cd_autorizador,
                    data = e.data,
                    valor = e.valor,
                    cnpj = e.cnpj,
                    parcelaTotal = e.parcelaTotal,
                    terminal = e.terminal,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    data_recebimento = e.data_recebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionValeCard;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova ValeCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, ValeCard param)
        {
            _db.ValeCards.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma ValeCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.ValeCards.Remove(_db.ValeCards.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera ValeCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, ValeCard param)
        {
            ValeCard value = _db.ValeCards
                    .Where(e => e.id.Equals(param.id))
                    .First<ValeCard>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.descricao != null && param.descricao != value.descricao)
                value.descricao = param.descricao;
            if (param.comprador != null && param.comprador != value.comprador)
                value.comprador = param.comprador;
            if (param.cd_autorizador != null && param.cd_autorizador != value.cd_autorizador)
                value.cd_autorizador = param.cd_autorizador;
            if (param.data != null && param.data != value.data)
                value.data = param.data;
            if (param.valor != null && param.valor != value.valor)
                value.valor = param.valor;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.parcelaTotal != null && param.parcelaTotal != value.parcelaTotal)
                value.parcelaTotal = param.parcelaTotal;
            if (param.terminal != null && param.terminal != value.terminal)
                value.terminal = param.terminal;
            if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                value.idOperadora = param.idOperadora;
            if (param.idBandeira != null && param.idBandeira != value.idBandeira)
                value.idBandeira = param.idBandeira;
            if (param.data_recebimento != null && param.data_recebimento != value.data_recebimento)
                value.data_recebimento = param.data_recebimento;
            if (param.idTerminalLogico != null && param.idTerminalLogico != value.idTerminalLogico)
                value.idTerminalLogico = param.idTerminalLogico;
            _db.SaveChanges();

        }

    }
}
