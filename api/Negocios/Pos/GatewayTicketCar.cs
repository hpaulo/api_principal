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
    public class GatewayTicketCar
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTicketCar()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DTATRANSACAO = 101,
            DESCRICAO = 102,
            TIPOTRANSACAO = 103,
            REEMBOLSO = 104,
            NUMCARTAO = 105,
            NUMOS = 106,
            MERCADORIA = 107,
            QTDE = 108,
            VALORUNITARIO = 109,
            VALORDESCONTO = 110,
            VALORBRUTO = 111,
            EMPRESA = 112,
            CNPJ = 113,
            IDOPERADORA = 114,
            IDBANDEIRA = 115,
            DTARECEBIMENTO = 116,
            IDTERMINALLOGICO = 117,

        };

        /// <summary>
        /// Get TicketCar/TicketCar
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<TicketCar> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.TicketCars.AsQueryable<TicketCar>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.DTATRANSACAO:
                        DateTime dtaTransacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaTransacao.Equals(dtaTransacao)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.DESCRICAO:
                        string descricao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.descricao.Equals(descricao)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.TIPOTRANSACAO:
                        string tipoTransacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tipoTransacao.Equals(tipoTransacao)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.REEMBOLSO:
                        string reembolso = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.reembolso.Equals(reembolso)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.NUMOS:
                        string numOS = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numOS.Equals(numOS)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.MERCADORIA:
                        string mercadoria = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.mercadoria.Equals(mercadoria)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.QTDE:
                        string qtde = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.qtde.Equals(qtde)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.VALORUNITARIO:
                        decimal valorUnitario = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorUnitario.Equals(valorUnitario)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.VALORDESCONTO:
                        decimal valorDesconto = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorDesconto.Equals(valorDesconto)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.VALORBRUTO:
                        decimal valorBruto = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorBruto.Equals(valorBruto)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.EMPRESA:
                        string empresa = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.empresa.Equals(empresa)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<TicketCar>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<TicketCar>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.DTATRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaTransacao).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.dtaTransacao).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.DESCRICAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.descricao).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.descricao).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.TIPOTRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tipoTransacao).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.tipoTransacao).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.REEMBOLSO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.reembolso).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.reembolso).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.NUMOS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numOS).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.numOS).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.MERCADORIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.mercadoria).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.mercadoria).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.QTDE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtde).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.qtde).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.VALORUNITARIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorUnitario).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.valorUnitario).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.VALORDESCONTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorDesconto).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.valorDesconto).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.VALORBRUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorBruto).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.valorBruto).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.EMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.empresa).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.empresa).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<TicketCar>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<TicketCar>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<TicketCar>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TicketCar/TicketCar
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionTicketCar = new List<dynamic>();
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
                CollectionTicketCar = query.Select(e => new
                {

                    id = e.id,
                    dtaTransacao = e.dtaTransacao,
                    descricao = e.descricao,
                    tipoTransacao = e.tipoTransacao,
                    reembolso = e.reembolso,
                    numCartao = e.numCartao,
                    numOS = e.numOS,
                    mercadoria = e.mercadoria,
                    qtde = e.qtde,
                    valorUnitario = e.valorUnitario,
                    valorDesconto = e.valorDesconto,
                    valorBruto = e.valorBruto,
                    empresa = e.empresa,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTicketCar = query.Select(e => new
                {

                    id = e.id,
                    dtaTransacao = e.dtaTransacao,
                    descricao = e.descricao,
                    tipoTransacao = e.tipoTransacao,
                    reembolso = e.reembolso,
                    numCartao = e.numCartao,
                    numOS = e.numOS,
                    mercadoria = e.mercadoria,
                    qtde = e.qtde,
                    valorUnitario = e.valorUnitario,
                    valorDesconto = e.valorDesconto,
                    valorBruto = e.valorBruto,
                    empresa = e.empresa,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTicketCar;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova TicketCar
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, TicketCar param)
        {
            _db.TicketCars.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma TicketCar
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.TicketCars.Remove(_db.TicketCars.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera TicketCar
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, TicketCar param)
        {
            TicketCar value = _db.TicketCars
                    .Where(e => e.id.Equals(param.id))
                    .First<TicketCar>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.dtaTransacao != null && param.dtaTransacao != value.dtaTransacao)
                value.dtaTransacao = param.dtaTransacao;
            if (param.descricao != null && param.descricao != value.descricao)
                value.descricao = param.descricao;
            if (param.tipoTransacao != null && param.tipoTransacao != value.tipoTransacao)
                value.tipoTransacao = param.tipoTransacao;
            if (param.reembolso != null && param.reembolso != value.reembolso)
                value.reembolso = param.reembolso;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.numOS != null && param.numOS != value.numOS)
                value.numOS = param.numOS;
            if (param.mercadoria != null && param.mercadoria != value.mercadoria)
                value.mercadoria = param.mercadoria;
            if (param.qtde != null && param.qtde != value.qtde)
                value.qtde = param.qtde;
            if (param.valorUnitario != null && param.valorUnitario != value.valorUnitario)
                value.valorUnitario = param.valorUnitario;
            if (param.valorDesconto != null && param.valorDesconto != value.valorDesconto)
                value.valorDesconto = param.valorDesconto;
            if (param.valorBruto != null && param.valorBruto != value.valorBruto)
                value.valorBruto = param.valorBruto;
            if (param.empresa != null && param.empresa != value.empresa)
                value.empresa = param.empresa;
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
