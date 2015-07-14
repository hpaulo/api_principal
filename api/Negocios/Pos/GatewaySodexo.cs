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
    public class GatewaySodexo
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewaySodexo()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DTAPAGAMENTO = 101,
            DTAPROCESSAMENTO = 102,
            DTATRANSACAO = 103,
            REDE = 104,
            DESCRICAO = 105,
            NUMCARTAO = 106,
            NSU = 107,
            CDAUTORIZADOR = 108,
            VALORTOTAL = 109,
            CNPJ = 110,
            IDOPERADORA = 111,
            IDBANDEIRA = 112,
            DTARECEBIMENTO = 113,
            IDTERMINALLOGICO = 114,

        };

        /// <summary>
        /// Get Sodexo/Sodexo
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Sodexo> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Sodexoes.AsQueryable<Sodexo>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.DTAPAGAMENTO:
                        DateTime dtaPagamento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaPagamento.Equals(dtaPagamento)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.DTAPROCESSAMENTO:
                        DateTime dtaProcessamento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaProcessamento.Equals(dtaProcessamento)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.DTATRANSACAO:
                        DateTime dtaTransacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaTransacao.Equals(dtaTransacao)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.REDE:
                        string rede = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.rede.Equals(rede)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.DESCRICAO:
                        string descricao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.descricao.Equals(descricao)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nsu.Equals(nsu)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.VALORTOTAL:
                        string valorTotal = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.valorTotal.Equals(valorTotal)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<Sodexo>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<Sodexo>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.DTAPAGAMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaPagamento).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.dtaPagamento).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.DTAPROCESSAMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaProcessamento).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.dtaProcessamento).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.DTATRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaTransacao).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.dtaTransacao).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.REDE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.rede).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.rede).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.DESCRICAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.descricao).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.descricao).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.NSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nsu).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.nsu).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.VALORTOTAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTotal).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.valorTotal).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<Sodexo>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<Sodexo>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<Sodexo>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Sodexo/Sodexo
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionSodexo = new List<dynamic>();
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
                CollectionSodexo = query.Select(e => new
                {

                    id = e.id,
                    dtaPagamento = e.dtaPagamento,
                    dtaProcessamento = e.dtaProcessamento,
                    dtaTransacao = e.dtaTransacao,
                    rede = e.rede,
                    descricao = e.descricao,
                    numCartao = e.numCartao,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    valorTotal = e.valorTotal,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionSodexo = query.Select(e => new
                {

                    id = e.id,
                    dtaPagamento = e.dtaPagamento,
                    dtaProcessamento = e.dtaProcessamento,
                    dtaTransacao = e.dtaTransacao,
                    rede = e.rede,
                    descricao = e.descricao,
                    numCartao = e.numCartao,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    valorTotal = e.valorTotal,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionSodexo;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova Sodexo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Sodexo param)
        {
            _db.Sodexoes.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma Sodexo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.Sodexoes.Remove(_db.Sodexoes.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera Sodexo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Sodexo param)
        {
            Sodexo value = _db.Sodexoes
                    .Where(e => e.id.Equals(param.id))
                    .First<Sodexo>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.dtaPagamento != null && param.dtaPagamento != value.dtaPagamento)
                value.dtaPagamento = param.dtaPagamento;
            if (param.dtaProcessamento != null && param.dtaProcessamento != value.dtaProcessamento)
                value.dtaProcessamento = param.dtaProcessamento;
            if (param.dtaTransacao != null && param.dtaTransacao != value.dtaTransacao)
                value.dtaTransacao = param.dtaTransacao;
            if (param.rede != null && param.rede != value.rede)
                value.rede = param.rede;
            if (param.descricao != null && param.descricao != value.descricao)
                value.descricao = param.descricao;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.nsu != null && param.nsu != value.nsu)
                value.nsu = param.nsu;
            if (param.cdAutorizador != null && param.cdAutorizador != value.cdAutorizador)
                value.cdAutorizador = param.cdAutorizador;
            if (param.valorTotal != null && param.valorTotal != value.valorTotal)
                value.valorTotal = param.valorTotal;
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
