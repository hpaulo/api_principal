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
    public class GatewayLogExecutionException
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayLogExecutionException()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID = 100,
            IDLOGEXECUTION = 101,
            TEXTERROR = 102,

        };

        /// <summary>
        /// Get LogExecutionException/LogExecutionException
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<LogExecutionException> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.LogExecutionExceptions.AsQueryable<LogExecutionException>();

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
                        entity = entity.Where(e => e.id.Equals(id)).AsQueryable<LogExecutionException>();
                        break;
                    case CAMPOS.IDLOGEXECUTION:
                        Int32 idLogExecution = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogExecution.Equals(idLogExecution)).AsQueryable<LogExecutionException>();
                        break;
                    case CAMPOS.TEXTERROR:
                        string textError = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.textError.Equals(textError)).AsQueryable<LogExecutionException>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.id).AsQueryable<LogExecutionException>();
                    else entity = entity.OrderByDescending(e => e.id).AsQueryable<LogExecutionException>();
                    break;
                case CAMPOS.IDLOGEXECUTION:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogExecution).AsQueryable<LogExecutionException>();
                    else entity = entity.OrderByDescending(e => e.idLogExecution).AsQueryable<LogExecutionException>();
                    break;
                case CAMPOS.TEXTERROR:
                    if (orderby == 0) entity = entity.OrderBy(e => e.textError).AsQueryable<LogExecutionException>();
                    else entity = entity.OrderByDescending(e => e.textError).AsQueryable<LogExecutionException>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna LogExecutionException/LogExecutionException
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionLogExecutionException = new List<dynamic>();
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
                    CollectionLogExecutionException = query.Select(e => new
                    {

                        id = e.id,
                        idLogExecution = e.idLogExecution,
                        textError = e.textError,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionLogExecutionException = query.Select(e => new
                    {

                        id = e.id,
                        idLogExecution = e.idLogExecution,
                        textError = e.textError,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionLogExecutionException;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar logexecutionexception" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Adiciona nova LogExecutionException
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, LogExecutionException param)
        {
            try
            {
                _db.LogExecutionExceptions.Add(param);
                _db.SaveChanges();
                return param.id;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar logexecutionexception" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma LogExecutionException
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id)
        {
            try
            {
                _db.LogExecutionExceptions.Remove(_db.LogExecutionExceptions.Where(e => e.id.Equals(id)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar logexecutionexception" : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Altera LogExecutionException
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, LogExecutionException param)
        {
            try
            {
                LogExecutionException value = _db.LogExecutionExceptions
                        .Where(e => e.id.Equals(param.id))
                        .First<LogExecutionException>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.id != null && param.id != value.id)
                    value.id = param.id;
                if (param.idLogExecution != null && param.idLogExecution != value.idLogExecution)
                    value.idLogExecution = param.idLogExecution;
                if (param.textError != null && param.textError != value.textError)
                    value.textError = param.textError;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar logexecutionexception" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
