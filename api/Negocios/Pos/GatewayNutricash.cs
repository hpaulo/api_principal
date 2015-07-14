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
    public class GatewayNutricash
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayNutricash()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            CDAUTORIZADOR = 101,
            STATUS = 102,
            DTAHORA = 103,
            NUMCARTAO = 104,
            CLIENTE = 105,
            VALOR = 106,
            CNPJ = 107,
            IDOPERADORA = 108,
            IDBANDEIRA = 109,
            DTARECEBIMENTO = 110,
            IDTERMINALLOGICO = 111,

        };

        /// <summary>
        /// Get Nutricash/Nutricash
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<Nutricash> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.Nutricashes.AsQueryable<Nutricash>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.CDAUTORIZADOR:
                        string cdAutorizador = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cdAutorizador.Equals(cdAutorizador)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.STATUS:
                        string status = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.status.Equals(status)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.DTAHORA:
                        DateTime dtaHora = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaHora.Equals(dtaHora)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.NUMCARTAO:
                        string numCartao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.numCartao.Equals(numCartao)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.CLIENTE:
                        string cliente = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.credenciado.Equals(cliente)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.VALOR:
                        decimal valor = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.valor.Equals(valor)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.CNPJ:
                        string cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.cnpj.Equals(cnpj)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.IDBANDEIRA:
                        Int32 idBandeira = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idBandeira.Equals(idBandeira)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.DTARECEBIMENTO:
                        DateTime dtaRecebimento = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaRecebimento.Equals(dtaRecebimento)).AsQueryable<Nutricash>();
                        break;
                    case CAMPOS.IDTERMINALLOGICO:
                        Int32 idTerminalLogico = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idTerminalLogico.Equals(idTerminalLogico)).AsQueryable<Nutricash>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.CDAUTORIZADOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdAutorizador).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.cdAutorizador).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.STATUS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.status).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.status).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.DTAHORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaHora).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.dtaHora).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.NUMCARTAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.numCartao).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.numCartao).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.CLIENTE:
                    if (orderby == 0) entity = entity.OrderBy(e => e.credenciado).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.credenciado).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.VALOR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.valor).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.valor).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.CNPJ:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cnpj).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.cnpj).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.IDBANDEIRA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idBandeira).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.idBandeira).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.DTARECEBIMENTO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaRecebimento).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.dtaRecebimento).AsQueryable<Nutricash>();
                    break;
                case CAMPOS.IDTERMINALLOGICO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idTerminalLogico).AsQueryable<Nutricash>();
                    else entity = entity.OrderByDescending(e => e.idTerminalLogico).AsQueryable<Nutricash>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Nutricash/Nutricash
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionNutricash = new List<dynamic>();
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
                CollectionNutricash = query.Select(e => new
                {

                    id = e.id,
                    cdAutorizador = e.cdAutorizador,
                    status = e.status,
                    dtaHora = e.dtaHora,
                    numCartao = e.numCartao,
                    cliente = e.credenciado,
                    valor = e.valor,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionNutricash = query.Select(e => new
                {

                    id = e.id,
                    cdAutorizador = e.cdAutorizador,
                    status = e.status,
                    dtaHora = e.dtaHora,
                    numCartao = e.numCartao,
                    cliente = e.credenciado,
                    valor = e.valor,
                    cnpj = e.cnpj,
                    idOperadora = e.idOperadora,
                    idBandeira = e.idBandeira,
                    dtaRecebimento = e.dtaRecebimento,
                    idTerminalLogico = e.idTerminalLogico,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionNutricash;

            return retorno;
        }
        /// <summary>
        /// Adiciona nova Nutricash
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Nutricash param)
        {
            _db.Nutricashes.Add(param);
            _db.SaveChanges();
            return param.id;
        }


        /// <summary>
        /// Apaga uma Nutricash
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            _db.Nutricashes.Remove(_db.Nutricashes.Where(e => e.id.Equals(id)).First());
            _db.SaveChanges();
        }
        /// <summary>
        /// Altera Nutricash
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Nutricash param)
        {
            Nutricash value = _db.Nutricashes
                    .Where(e => e.id.Equals(param.id))
                    .First<Nutricash>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.id != null && param.id != value.id)
                value.id = param.id;
            if (param.cdAutorizador != null && param.cdAutorizador != value.cdAutorizador)
                value.cdAutorizador = param.cdAutorizador;
            if (param.status != null && param.status != value.status)
                value.status = param.status;
            if (param.dtaHora != null && param.dtaHora != value.dtaHora)
                value.dtaHora = param.dtaHora;
            if (param.numCartao != null && param.numCartao != value.numCartao)
                value.numCartao = param.numCartao;
            if (param.credenciado != null && param.credenciado != value.credenciado)
                value.credenciado = param.credenciado;
            if (param.valor != null && param.valor != value.valor)
                value.valor = param.valor;
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
