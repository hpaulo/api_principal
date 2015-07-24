using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;

namespace api.Negocios.Administracao
{
    public class GatewayWebpagesUsersInRoles
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesUsersInRoles()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            USERID = 100,
            ROLEID = 101,

        };

        /// <summary>
        /// Get Webpages_UsersInRoles/Webpages_UsersInRoles
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_UsersInRoles> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_UsersInRoles .AsQueryable<webpages_UsersInRoles>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.USERID:
                        Int32 UserId = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.UserId.Equals(UserId)).AsQueryable<webpages_UsersInRoles>();
                        break;
                    case CAMPOS.ROLEID:
                        Int32 RoleId = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.RoleId.Equals(RoleId)).AsQueryable<webpages_UsersInRoles>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.USERID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.UserId).AsQueryable<webpages_UsersInRoles>();
                    else entity = entity.OrderByDescending(e => e.UserId).AsQueryable<webpages_UsersInRoles>();
                    break;
                case CAMPOS.ROLEID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.RoleId).AsQueryable<webpages_UsersInRoles>();
                    else entity = entity.OrderByDescending(e => e.RoleId).AsQueryable<webpages_UsersInRoles>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_UsersInRoles/Webpages_UsersInRoles
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionWebpages_UsersInRoles = new List<dynamic>();
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
                CollectionWebpages_UsersInRoles = query.Select(e => new
                {

                    UserId = e.UserId,
                    RoleId = e.RoleId,
                    RolePrincipal = e.RolePrincipal
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionWebpages_UsersInRoles = query.Select(e => new
                {

                    UserId = e.UserId,
                    RoleId = e.RoleId,
                    RolePrincipal = e.RolePrincipal
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionWebpages_UsersInRoles;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Webpages_UsersInRoles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_UsersInRoles param)
        {
            _db.webpages_UsersInRoles.Add(param);
            _db.SaveChanges();
            return param.UserId;
        }


        /// <summary>
        /// Apaga uma Webpages_UsersInRoles
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isRole"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id, Boolean isRole)
        {
            if (isRole) _db.webpages_UsersInRoles.RemoveRange(_db.webpages_UsersInRoles.Where(e => e.RoleId == id));
            else _db.webpages_UsersInRoles.RemoveRange(_db.webpages_UsersInRoles.Where(e => e.UserId == id));
            _db.SaveChanges();
        }

        /// <summary>
        /// Apaga uma Webpages_UsersInRoles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, webpages_UsersInRoles param)
        {
            _db.webpages_UsersInRoles.RemoveRange(_db.webpages_UsersInRoles.Where(e => e.UserId == param.UserId && e.RoleId == param.RoleId));
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera webpages_UsersInRoles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_UsersInRoles param)
        {
            webpages_UsersInRoles value = _db.webpages_UsersInRoles
                    .Where(e => e.UserId.Equals(param.UserId))
                    .First<webpages_UsersInRoles>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            if (param.UserId != null && param.UserId != value.UserId)
                value.UserId = param.UserId;
            if (param.RoleId != null && param.RoleId != value.RoleId)
                value.RoleId = param.RoleId;
            _db.SaveChanges();

        }

    }
}
