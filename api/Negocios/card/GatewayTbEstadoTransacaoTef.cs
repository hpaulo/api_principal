using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Card
{
    public class GatewayTbEstadoTransacaoTef
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbEstadoTransacaoTef()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            CDESTADOTRANSACAOTEF = 100,
            DSESTADOTRANSACAOTEF = 101,

        };

        /// <summary>
        /// Get TbEstadoTransacaoTef/TbEstadoTransacaoTef
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbEstadoTransacaoTef> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbEstadoTransacaoTefs.AsQueryable<tbEstadoTransacaoTef>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.CDESTADOTRANSACAOTEF:
                        Int32 cdEstadoTransacaoTef = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.cdEstadoTransacaoTef == cdEstadoTransacaoTef).AsQueryable<tbEstadoTransacaoTef>();
                        break;
                    case CAMPOS.DSESTADOTRANSACAOTEF:
                        string dsEstadoTransacaoTef = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsEstadoTransacaoTef.Equals(dsEstadoTransacaoTef)).AsQueryable<tbEstadoTransacaoTef>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.CDESTADOTRANSACAOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.cdEstadoTransacaoTef).AsQueryable<tbEstadoTransacaoTef>();
                    else entity = entity.OrderByDescending(e => e.cdEstadoTransacaoTef).AsQueryable<tbEstadoTransacaoTef>();
                    break;
                case CAMPOS.DSESTADOTRANSACAOTEF:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsEstadoTransacaoTef).AsQueryable<tbEstadoTransacaoTef>();
                    else entity = entity.OrderByDescending(e => e.dsEstadoTransacaoTef).AsQueryable<tbEstadoTransacaoTef>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbEstadoTransacaoTef/TbEstadoTransacaoTef
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionTbEstadoTransacaoTef = new List<dynamic>();
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
                    CollectionTbEstadoTransacaoTef = query.Select(e => new
                    {

                        cdEstadoTransacaoTef = e.cdEstadoTransacaoTef,
                        dsEstadoTransacaoTef = e.dsEstadoTransacaoTef,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionTbEstadoTransacaoTef = query.Select(e => new
                    {

                        cdEstadoTransacaoTef = e.cdEstadoTransacaoTef,
                        dsEstadoTransacaoTef = e.dsEstadoTransacaoTef,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionTbEstadoTransacaoTef;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar estadotransacaotef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbEstadoTransacaoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbEstadoTransacaoTef param)
        {
            try
            {
                _db.tbEstadoTransacaoTefs.Add(param);
                _db.SaveChanges();
                return param.cdEstadoTransacaoTef;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar estadotransacaotef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbEstadoTransacaoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, short cdEstadoTransacaoTef)
        {
            try
            {
                _db.tbEstadoTransacaoTefs.Remove(_db.tbEstadoTransacaoTefs.Where(e => e.cdEstadoTransacaoTef.Equals(cdEstadoTransacaoTef)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar estadotransacaotef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera tbEstadoTransacaoTef
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbEstadoTransacaoTef param)
        {
            try
            {
                tbEstadoTransacaoTef value = _db.tbEstadoTransacaoTefs
                        .Where(e => e.cdEstadoTransacaoTef.Equals(param.cdEstadoTransacaoTef))
                        .First<tbEstadoTransacaoTef>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.cdEstadoTransacaoTef != null && param.cdEstadoTransacaoTef != value.cdEstadoTransacaoTef)
                    value.cdEstadoTransacaoTef = param.cdEstadoTransacaoTef;
                if (param.dsEstadoTransacaoTef != null && param.dsEstadoTransacaoTef != value.dsEstadoTransacaoTef)
                    value.dsEstadoTransacaoTef = param.dsEstadoTransacaoTef;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar estadotransacaotef" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
