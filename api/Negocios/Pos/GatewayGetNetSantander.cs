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
    public class GatewayGetNetSantander
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayGetNetSantander()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            BANDEIRA = 101,
            PRODUTO = 102,
            DESCRICAOTRANSACAO = 103,
            DTATRANSACAO = 104,
            HRATRANSACAO = 105,
            DTAHRATRANSACAO = 106,
            NUMCARTAO = 107,
            NUMCV = 108,
            NUMAUTORIZACAO = 109,
            VALORTOTALTRANSACAO = 110,
            TOTALPARCELAS = 111,
            CNPJ = 112,
            IDOPERADORA = 113,
            IDBANDEIRA = 114,
            DTARECEBIMENTO = 115,
            IDTERMINALLOGICO = 116,

        };

        /// <summary>
        /// Get GetNetSantander/GetNetSantander
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<GetNetSantander> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.GetNetSantanders.AsQueryable<GetNetSantander>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.BANDEIRA:
                        string bandeira = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.bandeira.Equals(bandeira)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.PRODUTO:
                        string produto = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.produto.Equals(produto)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.DESCRICAOTRANSACAO:
                        string descricaoTransacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.descricaoTransacao.Equals(descricaoTransacao)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.DTATRANSACAO:
                        DateTime dtaTransacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaTransacao.Equals(dtaTransacao)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.HRATRANSACAO:
                        DateTime hraTransacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.hraTransacao.Equals(hraTransacao)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.DTAHRATRANSACAO:
                        DateTime dtahraTransacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtahraTransacao.Equals(dtahraTransacao)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.NUMCV:
                        string numCv = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCv.Equals(numCv)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.NUMAUTORIZACAO:
                        string numAutorizacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numAutorizacao.Equals(numAutorizacao)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.VALORTOTALTRANSACAO:
                        decimal valorTotalTransacao = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorTotalTransacao.Equals(valorTotalTransacao)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.TOTALPARCELAS:
                        Int32 totalParcelas = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.totalParcelas.Equals(totalParcelas)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<GetNetSantander>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<GetNetSantander>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.BANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.bandeira).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.bandeira).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.PRODUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.produto).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.produto).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.DESCRICAOTRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.descricaoTransacao).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.descricaoTransacao).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.DTATRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaTransacao).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.dtaTransacao).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.HRATRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.hraTransacao).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.hraTransacao).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.DTAHRATRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtahraTransacao).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.dtahraTransacao).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.NUMCV:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCv).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.numCv).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.NUMAUTORIZACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numAutorizacao).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.numAutorizacao).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.VALORTOTALTRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTotalTransacao).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.valorTotalTransacao).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.TOTALPARCELAS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.totalParcelas).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.totalParcelas).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<GetNetSantander>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<GetNetSantander>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<GetNetSantander>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna GetNetSantander/GetNetSantander
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionGetNetSantander = new List<dynamic>();
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
                CollectionGetNetSantander = query.Select(e => new
                {

                    id = e.id,
                    bandeira = e.bandeira,
                    produto = e.produto,
                    descricaoTransacao = e.descricaoTransacao,
                    dtaTransacao = e.dtaTransacao,
                    hraTransacao = e.hraTransacao,
                    dtahraTransacao = e.dtahraTransacao,
                    numCartao = e.numCartao,
                    numCv = e.numCv,
                    numAutorizacao = e.numAutorizacao,
                    valorTotalTransacao = e.valorTotalTransacao,
                    totalParcelas = e.totalParcelas,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionGetNetSantander = query.Select(e => new
                {

                    id = e.id,
                    bandeira = e.bandeira,
                    produto = e.produto,
                    descricaoTransacao = e.descricaoTransacao,
                    dtaTransacao = e.dtaTransacao,
                    hraTransacao = e.hraTransacao,
                    dtahraTransacao = e.dtahraTransacao,
                    numCartao = e.numCartao,
                    numCv = e.numCv,
                    numAutorizacao = e.numAutorizacao,
                    valorTotalTransacao = e.valorTotalTransacao,
                    totalParcelas = e.totalParcelas,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionGetNetSantander;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova GetNetSantander
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, GetNetSantander param)
        {
            _db.GetNetSantanders.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma GetNetSantander
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.GetNetSantanders.Remove(_db.GetNetSantanders.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera GetNetSantander
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, GetNetSantander param)
        {
            GetNetSantander value = _db.GetNetSantanders
                    .Where(e => e.id.Equals(param.id))
                    .First<GetNetSantander>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.bandeira != null && param.bandeira != value.bandeira)
                value.bandeira = param.bandeira;
            if (param.produto != null && param.produto != value.produto)
                value.produto = param.produto;
            if (param.descricaoTransacao != null && param.descricaoTransacao != value.descricaoTransacao)
                value.descricaoTransacao = param.descricaoTransacao;
            if (param.dtaTransacao != null && param.dtaTransacao != value.dtaTransacao)
                value.dtaTransacao = param.dtaTransacao;
            if (param.hraTransacao != null && param.hraTransacao != value.hraTransacao)
                value.hraTransacao = param.hraTransacao;
            if (param.dtahraTransacao != null && param.dtahraTransacao != value.dtahraTransacao)
                value.dtahraTransacao = param.dtahraTransacao;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.numCv != null && param.numCv != value.numCv)
                value.numCv = param.numCv;
            if (param.numAutorizacao != null && param.numAutorizacao != value.numAutorizacao)
                value.numAutorizacao = param.numAutorizacao;
            if (param.valorTotalTransacao != null && param.valorTotalTransacao != value.valorTotalTransacao)
                value.valorTotalTransacao = param.valorTotalTransacao;
            if (param.totalParcelas != null && param.totalParcelas != value.totalParcelas)
                value.totalParcelas = param.totalParcelas;
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
