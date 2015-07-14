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
    public class GatewayAmex
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayAmex()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DATARECEBIMENTO = 101,
            DATAVENDA = 102,
            NSU = 103,
            CDAUTORIZADOR = 104,
            CNPJ = 105,
            NUMCARTAO = 106,
            TOTALPARCELAS = 107,
            VALORTOTAL = 108,
            NUMSUBMISSAO = 109,
            IDOPERADORA = 110,
            IDBANDEIRA = 111,
            IDTERMINALLOGICO = 112,

        };

        /// <summary>
        /// Get Amex/Amex
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Amex> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Amexes.AsQueryable<Amex>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.DATARECEBIMENTO:
                        DateTime dataRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dataRecebimento.Equals(dataRecebimento)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.DATAVENDA:
                        DateTime dataVenda = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dataVenda.Equals(dataVenda)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.NSU:
                        string nsu = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nsu.Equals(nsu)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.TOTALPARCELAS:
                        Int32 totalParcelas = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.totalParcelas.Equals(totalParcelas)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.VALORTOTAL:
                        decimal valorTotal = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorTotal.Equals(valorTotal)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.NUMSUBMISSAO:
                        string numSubmissao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numSubmissao.Equals(numSubmissao)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<Amex>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<Amex>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<Amex>();
                    break;
                case CAMPOS.DATARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dataRecebimento).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.dataRecebimento).AsQueryable<Amex>();
                    break;
                case CAMPOS.DATAVENDA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dataVenda).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.dataVenda).AsQueryable<Amex>();
                    break;
                case CAMPOS.NSU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nsu).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.nsu).AsQueryable<Amex>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable<Amex>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<Amex>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<Amex>();
                    break;
                case CAMPOS.TOTALPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.totalParcelas).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.totalParcelas).AsQueryable<Amex>();
                    break;
                case CAMPOS.VALORTOTAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTotal).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.valorTotal).AsQueryable<Amex>();
                    break;
                case CAMPOS.NUMSUBMISSAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numSubmissao).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.numSubmissao).AsQueryable<Amex>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<Amex>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<Amex>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<Amex>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<Amex>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Amex/Amex
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionAmex = new List<dynamic>();
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
                CollectionAmex = query.Select(e => new
                {

                    id = e.id,
                    dataRecebimento = e.dataRecebimento,
                    dataVenda = e.dataVenda,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    cnpj = e.cnpj,
                    numCartao = e.numCartao,
                    totalParcelas = e.totalParcelas,
                    valorTotal = e.valorTotal,
                    numSubmissao = e.numSubmissao,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionAmex = query.Select(e => new
                {

                    id = e.id,
                    dataRecebimento = e.dataRecebimento,
                    dataVenda = e.dataVenda,
                    nsu = e.nsu,
                    cdAutorizador = e.cdAutorizador,
                    cnpj = e.cnpj,
                    numCartao = e.numCartao,
                    totalParcelas = e.totalParcelas,
                    valorTotal = e.valorTotal,
                    numSubmissao = e.numSubmissao,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionAmex;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova Amex
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Amex param)
        {
            _db.Amexes.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma Amex
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.Amexes.Remove(_db.Amexes.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera Amex
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Amex param)
        {
            Amex value = _db.Amexes
                    .Where(e => e.id.Equals(param.id))
                    .First<Amex>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.dataRecebimento != null && param.dataRecebimento != value.dataRecebimento)
                value.dataRecebimento = param.dataRecebimento;
            if (param.dataVenda != null && param.dataVenda != value.dataVenda)
                value.dataVenda = param.dataVenda;
            if (param.nsu != null && param.nsu != value.nsu)
                value.nsu = param.nsu;
            if (param.cdAutorizador != null && param.cdAutorizador != value.cdAutorizador)
                value.cdAutorizador = param.cdAutorizador;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.totalParcelas != null && param.totalParcelas != value.totalParcelas)
                value.totalParcelas = param.totalParcelas;
            if (param.valorTotal != null && param.valorTotal != value.valorTotal)
                value.valorTotal = param.valorTotal;
            if (param.numSubmissao != null && param.numSubmissao != value.numSubmissao)
                value.numSubmissao = param.numSubmissao;
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
