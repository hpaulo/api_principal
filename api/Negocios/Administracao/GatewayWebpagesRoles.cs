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
    public class GatewayWebpagesRoles
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesRoles()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ROLEID = 100,
            ROLENAME = 101,

        };

        /// <summary>
        /// Get Webpages_Roles/Webpages_Roles
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_Roles> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_Roles.AsQueryable<webpages_Roles>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.ROLEID:
                        Int32 RoleId = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.RoleId.Equals(RoleId)).AsQueryable<webpages_Roles>();
                        break;
                    case CAMPOS.ROLENAME:
                        string RoleName = Convert.ToString(item.Value);
                        if (RoleName.Contains("%")) // usa LIKE
                        {
                            string busca = RoleName.Replace("%", "").ToString();
                            entity = _db.webpages_Roles.Where(e => e.RoleName.Contains(busca));
                        }
                        else
                        entity = entity.Where(e => e.RoleName.Equals(RoleName)).AsQueryable<webpages_Roles>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.ROLEID:
                    if (orderby == 0) entity = entity.OrderBy(e => e.RoleId).AsQueryable<webpages_Roles>();
                    else entity = entity.OrderByDescending(e => e.RoleId).AsQueryable<webpages_Roles>();
                    break;
                case CAMPOS.ROLENAME:
                    if (orderby == 0) entity = entity.OrderBy(e => e.RoleName).AsQueryable<webpages_Roles>();
                    else entity = entity.OrderByDescending(e => e.RoleName).AsQueryable<webpages_Roles>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_Roles/Webpages_Roles
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionWebpages_Roles = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);
            query = query.Where(e => e.RoleId > 50);
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
                CollectionWebpages_Roles = query
                .Select(e => new
                {

                    RoleId = e.RoleId,
                    RoleName = e.RoleName,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionWebpages_Roles = query
                .Select(e => new
                {

                    RoleId = e.RoleId,
                    RoleName = e.RoleName,
                }).ToList<dynamic>();
            }
            else if (colecao == 2)
            {
                CollectionWebpages_Roles = query
                .Select(e => new
                {

                    RoleId = e.RoleId,
                    RoleName = e.RoleName,

                    PaginaInicial = (e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.id_subController != null
                                     && e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.webpages_Controllers2.id_subController != null ?
                                    e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.webpages_Controllers2.webpages_Controllers2.ds_controller
                                    + " > ": "") + 
                                    (e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.id_subController != null ?
                                    e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.webpages_Controllers2.ds_controller
                                    + " > ": "") +
                                    e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.ds_controller
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionWebpages_Roles;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Webpages_Roles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_Roles param)
        {
            _db.webpages_Roles.Add(param);
            _db.SaveChanges();
            return param.RoleId;
        }


        /// <summary>
        /// Apaga uma Webpages_Roles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 RoleId)
        {
            GatewayWebpagesPermissions.Delete(token, RoleId);
            GatewayWebpagesUsersInRoles.Delete(token, RoleId, true);
            _db.webpages_Roles.Remove(_db.webpages_Roles.Where(e => e.RoleId.Equals(RoleId)).First());
            _db.SaveChanges();
        }



        /// <summary>
        /// Altera webpages_Roles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_Roles param)
        {
            webpages_Roles value = _db.webpages_Roles
                    .Where(e => e.RoleId.Equals(param.RoleId))
                    .First<webpages_Roles>();

            if (param.RoleName != null && param.RoleName != value.RoleName)
                value.RoleName = param.RoleName;
            _db.SaveChanges();

        }

    }
}
