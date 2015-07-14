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
    public class GatewayPoliCard
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayPoliCard()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            DATA_TRANSACAO = 101,
            PRODUTO = 102,
            CNPJ = 103,
            PREVREPASSE = 104,
            USUARIO = 105,
            CD_AUTORIZADOR = 106,
            TIPO = 107,
            VALORCREDITO = 108,
            VALORDEBITO = 109,
            SALDO = 110,
            REDE = 111,
            IDOPERADORA = 112,
            IDBANDEIRA = 113,
            DATA_RECEBIMENTO = 114,
            IDTERMINALLOGICO = 115,

        };

        /// <summary>
        /// Get PoliCard/PoliCard
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<PoliCard> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.PoliCards.AsQueryable<PoliCard>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.DATA_TRANSACAO:
                        DateTime data_transacao = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data_transacao.Equals(data_transacao)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.PRODUTO:
                        string produto = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.produto.Equals(produto)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.PREVREPASSE:
                        DateTime prevRepasse = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.prevRepasse.Equals(prevRepasse)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.USUARIO:
                        string usuario = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.usuario.Equals(usuario)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.CD_AUTORIZADOR:
                        string cd_autorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cd_autorizador.Equals(cd_autorizador)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.TIPO:
                        string tipo = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.tipo.Equals(tipo)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.VALORCREDITO:
                        decimal valorCredito = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorCredito.Equals(valorCredito)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.VALORDEBITO:
                        decimal valorDebito = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valorDebito.Equals(valorDebito)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.SALDO:
                        decimal Saldo = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.Saldo.Equals(Saldo)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.REDE:
                        string rede = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.rede.Equals(rede)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.DATA_RECEBIMENTO:
                        DateTime data_recebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.data_recebimento.Equals(data_recebimento)).AsQueryable<PoliCard>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<PoliCard>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.DATA_TRANSACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data_transacao).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.data_transacao).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.PRODUTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.produto).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.produto).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.PREVREPASSE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.prevRepasse).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.prevRepasse).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.USUARIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.usuario).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.usuario).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.CD_AUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cd_autorizador).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.cd_autorizador).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.TIPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.tipo).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.tipo).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.VALORCREDITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorCredito).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.valorCredito).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.VALORDEBITO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valorDebito).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.valorDebito).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.SALDO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.Saldo).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.Saldo).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.REDE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.rede).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.rede).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.DATA_RECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.data_recebimento).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.data_recebimento).AsQueryable<PoliCard>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<PoliCard>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<PoliCard>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna PoliCard/PoliCard
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionPoliCard = new List<dynamic>();
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
                CollectionPoliCard = query.Select(e => new
                {

                    id = e.id,
                    data_transacao = e.data_transacao,
                    produto = e.produto,
                    cnpj = e.cnpj,
                    prevRepasse = e.prevRepasse,
                    usuario = e.usuario,
                    cd_autorizador = e.cd_autorizador,
                    tipo = e.tipo,
                    valorCredito = e.valorCredito,
                    valorDebito = e.valorDebito,
                    Saldo = e.Saldo,
                    rede = e.rede,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    data_recebimento = e.data_recebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionPoliCard = query.Select(e => new
                {

                    id = e.id,
                    data_transacao = e.data_transacao,
                    produto = e.produto,
                    cnpj = e.cnpj,
                    prevRepasse = e.prevRepasse,
                    usuario = e.usuario,
                    cd_autorizador = e.cd_autorizador,
                    tipo = e.tipo,
                    valorCredito = e.valorCredito,
                    valorDebito = e.valorDebito,
                    Saldo = e.Saldo,
                    rede = e.rede,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    data_recebimento = e.data_recebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionPoliCard;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova PoliCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, PoliCard param)
        {
            _db.PoliCards.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma PoliCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.PoliCards.Remove(_db.PoliCards.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera PoliCard
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, PoliCard param)
        {
            PoliCard value = _db.PoliCards
                    .Where(e => e.id.Equals(param.id))
                    .First<PoliCard>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.data_transacao != null && param.data_transacao != value.data_transacao)
                value.data_transacao = param.data_transacao;
            if (param.produto != null && param.produto != value.produto)
                value.produto = param.produto;
            if (param.cnpj != null && param.cnpj != value.cnpj)
                value.cnpj = param.cnpj;
            if (param.prevRepasse != null && param.prevRepasse != value.prevRepasse)
                value.prevRepasse = param.prevRepasse;
            if (param.usuario != null && param.usuario != value.usuario)
                value.usuario = param.usuario;
            if (param.cd_autorizador != null && param.cd_autorizador != value.cd_autorizador)
                value.cd_autorizador = param.cd_autorizador;
            if (param.tipo != null && param.tipo != value.tipo)
                value.tipo = param.tipo;
            if (param.valorCredito != null && param.valorCredito != value.valorCredito)
                value.valorCredito = param.valorCredito;
            if (param.valorDebito != null && param.valorDebito != value.valorDebito)
                value.valorDebito = param.valorDebito;
            if (param.Saldo != null && param.Saldo != value.Saldo)
                value.Saldo = param.Saldo;
            if (param.rede != null && param.rede != value.rede)
                value.rede = param.rede;
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
