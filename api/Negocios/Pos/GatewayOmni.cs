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
    public class GatewayOmni
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayOmni()
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
            PRODUTO = 103,
            NUMCPF = 104,
            NUMCARTAO = 105,
            VALORTOTAL = 106,
            METODO = 107,
            SITUACAO = 108,
            CDAUTORIZACAO = 109,
            USUARIO = 110,
            CNPJ = 111,
            IDOPERADORA = 112,
            IDBANDEIRA = 113,
            DTARECEBIMENTO = 114,
            IDTERMINALLOGICO = 115,

        };

        /// <summary>
        /// Get Omni/Omni
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Omni> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Omnis.AsQueryable<Omni>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.DTATRANSACAO:
                        DateTime dtaTransacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaTransacao.Equals(dtaTransacao)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.DESCRICAO:
                        string descricao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.descricao.Equals(descricao)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.PRODUTO:
                        string produto = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.produto.Equals(produto)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.NUMCPF:
                        string numCpf = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCpf.Equals(numCpf)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.VALORTOTAL:
                        decimal valorTotal = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorTotal.Equals(valorTotal)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.METODO:
                        string metodo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.metodo.Equals(metodo)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.SITUACAO:
                        string situacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.situacao.Equals(situacao)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.CDAUTORIZACAO:
                        string cdAutorizacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizacao.Equals(cdAutorizacao)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.USUARIO:
                        string usuario = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.usuario.Equals(usuario)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<Omni>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<Omni>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<Omni>();
                    break;
                case CAMPOS.DTATRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaTransacao).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.dtaTransacao).AsQueryable<Omni>();
                    break;
                case CAMPOS.DESCRICAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.descricao).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.descricao).AsQueryable<Omni>();
                    break;
                case CAMPOS.PRODUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.produto).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.produto).AsQueryable<Omni>();
                    break;
                case CAMPOS.NUMCPF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCpf).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.numCpf).AsQueryable<Omni>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<Omni>();
                    break;
                case CAMPOS.VALORTOTAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorTotal).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.valorTotal).AsQueryable<Omni>();
                    break;
                case CAMPOS.METODO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.metodo).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.metodo).AsQueryable<Omni>();
                    break;
                case CAMPOS.SITUACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.situacao).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.situacao).AsQueryable<Omni>();
                    break;
                case CAMPOS.CDAUTORIZACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizacao).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizacao).AsQueryable<Omni>();
                    break;
                case CAMPOS.USUARIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.usuario).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.usuario).AsQueryable<Omni>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<Omni>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<Omni>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<Omni>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<Omni>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<Omni>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<Omni>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Omni/Omni
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionOmni = new List<dynamic>();
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
                CollectionOmni = query.Select(e => new
                {

                    id = e.id,
                    dtaTransacao = e.dtaTransacao,
                    descricao = e.descricao,
                    produto = e.produto,
                    numCpf = e.numCpf,
                    numCartao = e.numCartao,
                    valorTotal = e.valorTotal,
                    metodo = e.metodo,
                    situacao = e.situacao,
                    cdAutorizacao = e.cdAutorizacao,
                    usuario = e.usuario,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionOmni = query.Select(e => new
                {

                    id = e.id,
                    dtaTransacao = e.dtaTransacao,
                    descricao = e.descricao,
                    produto = e.produto,
                    numCpf = e.numCpf,
                    numCartao = e.numCartao,
                    valorTotal = e.valorTotal,
                    metodo = e.metodo,
                    situacao = e.situacao,
                    cdAutorizacao = e.cdAutorizacao,
                    usuario = e.usuario,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionOmni;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova Omni
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Omni param)
        {
            _db.Omnis.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma Omni
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.Omnis.Remove(_db.Omnis.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera Omni
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Omni param)
        {
            Omni value = _db.Omnis
                    .Where(e => e.id.Equals(param.id))
                    .First<Omni>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.dtaTransacao != null && param.dtaTransacao != value.dtaTransacao)
                value.dtaTransacao = param.dtaTransacao;
            if (param.descricao != null && param.descricao != value.descricao)
                value.descricao = param.descricao;
            if (param.produto != null && param.produto != value.produto)
                value.produto = param.produto;
            if (param.numCpf != null && param.numCpf != value.numCpf)
                value.numCpf = param.numCpf;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.valorTotal != null && param.valorTotal != value.valorTotal)
                value.valorTotal = param.valorTotal;
            if (param.metodo != null && param.metodo != value.metodo)
                value.metodo = param.metodo;
            if (param.situacao != null && param.situacao != value.situacao)
                value.situacao = param.situacao;
            if (param.cdAutorizacao != null && param.cdAutorizacao != value.cdAutorizacao)
                value.cdAutorizacao = param.cdAutorizacao;
            if (param.usuario != null && param.usuario != value.usuario)
                value.usuario = param.usuario;
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
