using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Dbo
{
    public class GatewayWebpagesRoleLevels
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesRoleLevels()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            LEVELID = 100,
            LEVELNAME = 101,

        };

        /// <summary>
        /// Get Webpages_RoleLevels/Webpages_RoleLevels
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_RoleLevels> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_RoleLevels.AsQueryable<webpages_RoleLevels>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.LEVELID:
                        Int32 LevelId = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.LevelId == LevelId).AsQueryable<webpages_RoleLevels>();
                        break;
                    case CAMPOS.LEVELNAME:
                        string LevelName = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.LevelName.Equals(LevelName)).AsQueryable<webpages_RoleLevels>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.LEVELID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.LevelId).AsQueryable<webpages_RoleLevels>();
                    else entity = entity.OrderByDescending(e => e.LevelId).AsQueryable<webpages_RoleLevels>();
                    break;
                case CAMPOS.LEVELNAME:
                    if (orderby == 0) entity = entity.OrderBy(e => e.LevelName).AsQueryable<webpages_RoleLevels>();
                    else entity = entity.OrderByDescending(e => e.LevelName).AsQueryable<webpages_RoleLevels>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_RoleLevels/Webpages_RoleLevels
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionWebpages_RoleLevels = new List<dynamic>();
                Retorno retorno = new Retorno();

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

                // só exibe a partir do RoleLevelMin
                Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
                query = query.Where(e => e.LevelId >= RoleLevelMin).AsQueryable<webpages_RoleLevels>();

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
                    CollectionWebpages_RoleLevels = query.Select(e => new
                    {

                        LevelId = e.LevelId,
                        LevelName = e.LevelName,
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionWebpages_RoleLevels = query.Select(e => new
                    {

                        LevelId = e.LevelId,
                        LevelName = e.LevelName,
                    }).ToList<dynamic>();
                }

                retorno.Registros = CollectionWebpages_RoleLevels;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar role levels" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Adiciona nova Webpages_RoleLevels
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_RoleLevels param)
        {
            try
            {
                _db.webpages_RoleLevels.Add(param);
                _db.SaveChanges();
                return param.LevelId;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar role levels" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }


        /// <summary>
        /// Apaga uma Webpages_RoleLevels
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 LevelId)
        {
            try
            {
                _db.webpages_RoleLevels.Remove(_db.webpages_RoleLevels.Where(e => e.LevelId == LevelId).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar role level" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }
        /// <summary>
        /// Altera webpages_RoleLevels
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_RoleLevels param)
        {
            try
            {
                webpages_RoleLevels value = _db.webpages_RoleLevels
                        .Where(e => e.LevelId.Equals(param.LevelId))
                        .First<webpages_RoleLevels>();

                // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


                if (param.LevelId != value.LevelId)
                    value.LevelId = param.LevelId;
                if (param.LevelName != null && param.LevelName != value.LevelName)
                    value.LevelName = param.LevelName;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar role level" : erro);
                }
                throw new Exception(e.InnerException == null ? e.Message : e.InnerException.InnerException == null ? e.InnerException.Message : e.InnerException.InnerException.Message);
            }
        }

    }
}
