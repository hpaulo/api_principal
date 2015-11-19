using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Pos
{
    public class GatewayLogExecution
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayLogExecution()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            IDOPERADORA = 101,
            DTAEXECUTION = 102,
            DTAFILTROTRANSACOES = 103,
            QTDTRANSACOES = 104,
            VLTOTALTRANSACOES = 105,
            STATUSEXECUTION = 106,
            IDLOGINOPERADORA = 107,
            DTAEXECUCAOINICIO = 108,
            DTAEXECUCAOFIM = 109,
            DTAFILTROTRANSACOESFINAL = 110,
            DTAEXECUCAOPROXIMA = 111,

        };

        /// <summary>
        /// Get LogExecution/LogExecution
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<LogExecution> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.LogExecutions.AsQueryable<LogExecution>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.IDOPERADORA:
                        Int32 idOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idOperadora.Equals(idOperadora)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.DTAEXECUTION:
                        DateTime dtaExecution = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaExecution.Equals(dtaExecution)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.DTAFILTROTRANSACOES:
                        DateTime dtaFiltroTransacoes = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaFiltroTransacoes.Equals(dtaFiltroTransacoes)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.QTDTRANSACOES:
                        Int32 qtdTransacoes = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.qtdTransacoes.Equals(qtdTransacoes)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.VLTOTALTRANSACOES:
                        decimal vlTotalTransacoes = Convert.ToDecimal(item.Value);
                        entity = entity.Where(e => e.vlTotalTransacoes.Equals(vlTotalTransacoes)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.STATUSEXECUTION:
                        string statusExecution = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.statusExecution.Equals(statusExecution)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.IDLOGINOPERADORA:
                        Int32 idLoginOperadora = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLoginOperadora.Equals(idLoginOperadora)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.DTAEXECUCAOINICIO:
                        DateTime dtaExecucaoInicio = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaExecucaoInicio.Equals(dtaExecucaoInicio)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.DTAEXECUCAOFIM:
                        DateTime dtaExecucaoFim = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaExecucaoFim.Equals(dtaExecucaoFim)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.DTAFILTROTRANSACOESFINAL:
                        DateTime dtaFiltroTransacoesFinal = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaFiltroTransacoesFinal.Equals(dtaFiltroTransacoesFinal)).AsQueryable<LogExecution>();
                        break;
                    case CAMPOS.DTAEXECUCAOPROXIMA:
                        DateTime dtaExecucaoProxima = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtaExecucaoProxima.Equals(dtaExecucaoProxima)).AsQueryable<LogExecution>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.IDOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idOperadora).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.idOperadora).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.DTAEXECUTION:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaExecution).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.dtaExecution).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.DTAFILTROTRANSACOES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaFiltroTransacoes).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.dtaFiltroTransacoes).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.QTDTRANSACOES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.qtdTransacoes).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.qtdTransacoes).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.VLTOTALTRANSACOES:
                    if (orderby == 0) entity = entity.OrderBy(e => e.vlTotalTransacoes).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.vlTotalTransacoes).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.STATUSEXECUTION:
                    if (orderby == 0) entity = entity.OrderBy(e => e.statusExecution).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.statusExecution).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.IDLOGINOPERADORA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLoginOperadora).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.idLoginOperadora).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.DTAEXECUCAOINICIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaExecucaoInicio).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.dtaExecucaoInicio).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.DTAEXECUCAOFIM:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaExecucaoFim).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.dtaExecucaoFim).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.DTAFILTROTRANSACOESFINAL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaFiltroTransacoesFinal).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.dtaFiltroTransacoesFinal).AsQueryable<LogExecution>();
                    break;
                case CAMPOS.DTAEXECUCAOPROXIMA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtaExecucaoProxima).AsQueryable<LogExecution>();
                    else entity = entity.OrderByDescending(e => e.dtaExecucaoProxima).AsQueryable<LogExecution>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna LogExecution/LogExecution
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionLogExecution = new List<dynamic>();
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
                    CollectionLogExecution = query.Select(e => new
                    {

                        id = e.id,
                        idOperadora = e.idOperadora,
                        dtaExecution = e.dtaExecution,
                        dtaFiltroTransacoes = e.dtaFiltroTransacoes,
                        qtdTransacoes = e.qtdTransacoes,
                        vlTotalTransacoes = e.vlTotalTransacoes,
                        statusExecution = e.statusExecution,
                        idLoginOperadora = e.idLoginOperadora,
                        dtaExecucaoInicio = e.dtaExecucaoInicio,
                        dtaExecucaoFim = e.dtaExecucaoFim,
                        dtaFiltroTransacoesFinal = e.dtaFiltroTransacoesFinal,
                        dtaExecucaoProxima = e.dtaExecucaoProxima,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionLogExecution = query.Select(e => new
                    {

                        id = e.id,
                        idOperadora = e.idOperadora,
                        dtaExecution = e.dtaExecution,
                        dtaFiltroTransacoes = e.dtaFiltroTransacoes,
                        qtdTransacoes = e.qtdTransacoes,
                        vlTotalTransacoes = e.vlTotalTransacoes,
                        statusExecution = e.statusExecution,
                        idLoginOperadora = e.idLoginOperadora,
                        dtaExecucaoInicio = e.dtaExecucaoInicio,
                        dtaExecucaoFim = e.dtaExecucaoFim,
                        dtaFiltroTransacoesFinal = e.dtaFiltroTransacoesFinal,
                        dtaExecucaoProxima = e.dtaExecucaoProxima,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionLogExecution;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar logexecution" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova LogExecution
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, LogExecution param)
        {
            try
            {
                _db.LogExecutions.Add(param);
                _db.SaveChanges();
                return param.id;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar logexecution" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma LogExecution
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            try
            {
                _db.LogExecutions.RemoveRange(_db.LogExecutions.Where(e => e.id == id));
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar logexecution" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera LogExecution
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, LogExecution param)
        {
            try
            {
                LogExecution value = _db.LogExecutions
                        .Where(e => e.id.Equals(param.id))
                        .First<LogExecution>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.id != null && param.id != value.id)
                    value.id = param.id;
                if (param.idOperadora != null && param.idOperadora != value.idOperadora)
                    value.idOperadora = param.idOperadora;
                if (param.dtaExecution != null && param.dtaExecution != value.dtaExecution)
                    value.dtaExecution = param.dtaExecution;
                if (param.dtaFiltroTransacoes != null && param.dtaFiltroTransacoes != value.dtaFiltroTransacoes)
                    value.dtaFiltroTransacoes = param.dtaFiltroTransacoes;
                if (param.qtdTransacoes != null && param.qtdTransacoes != value.qtdTransacoes)
                    value.qtdTransacoes = param.qtdTransacoes;
                if (param.vlTotalTransacoes != null && param.vlTotalTransacoes != value.vlTotalTransacoes)
                    value.vlTotalTransacoes = param.vlTotalTransacoes;
                if (param.statusExecution != null && param.statusExecution != value.statusExecution)
                    value.statusExecution = param.statusExecution;
                if (param.idLoginOperadora != null && param.idLoginOperadora != value.idLoginOperadora)
                    value.idLoginOperadora = param.idLoginOperadora;
                if (param.dtaExecucaoInicio != null && param.dtaExecucaoInicio != value.dtaExecucaoInicio)
                    value.dtaExecucaoInicio = param.dtaExecucaoInicio;
                if (param.dtaExecucaoFim != null && param.dtaExecucaoFim != value.dtaExecucaoFim)
                    value.dtaExecucaoFim = param.dtaExecucaoFim;
                if (param.dtaFiltroTransacoesFinal != null && param.dtaFiltroTransacoesFinal != value.dtaFiltroTransacoesFinal)
                    value.dtaFiltroTransacoesFinal = param.dtaFiltroTransacoesFinal;
                if (param.dtaExecucaoProxima != null && param.dtaExecucaoProxima != value.dtaExecucaoProxima)
                    value.dtaExecucaoProxima = param.dtaExecucaoProxima;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar logexecution" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
