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
    public class GatewayWebpagesControllers
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesControllers()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID_CONTROLLER = 100,
            DS_CONTROLLER = 101,
            NM_CONTROLLER = 102,
            FL_MENU = 103,
            ID_SUBCONTROLLER = 104,

            // PERSONALIZADO

            ROLEID = 200,

        };

        /// <summary>
        /// Get Webpages_Controllers/Webpages_Controllers
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_Controllers> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_Controllers.AsQueryable<webpages_Controllers>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.ID_CONTROLLER:
                        Int32 id_controller = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_controller.Equals(id_controller)).AsQueryable<webpages_Controllers>();
                        break;
                    case CAMPOS.DS_CONTROLLER:
                        string ds_controller = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ds_controller.Equals(ds_controller)).AsQueryable<webpages_Controllers>();
                        break;
                    case CAMPOS.NM_CONTROLLER:
                        string nm_controller = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nm_controller.Equals(nm_controller)).AsQueryable<webpages_Controllers>();
                        break;
                    case CAMPOS.FL_MENU:
                        Boolean fl_menu = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.fl_menu.Equals(fl_menu)).AsQueryable<webpages_Controllers>();
                        break;
                    case CAMPOS.ID_SUBCONTROLLER:
                        Int32 id_subController = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_subController.Equals(id_subController)).AsQueryable<webpages_Controllers>();
                        break;

                    //case CAMPOS.ROLEID:
                        //Int32 roleId = Convert.ToInt32(item.Value);
                        //entity = entity.Where( e => e.id_subController.Equals(roleId) ).AsQueryable<webpages_Controllers>();
                        //break;




                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.ID_CONTROLLER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_controller).AsQueryable<webpages_Controllers>();
                    else entity = entity.OrderByDescending(e => e.id_controller).AsQueryable<webpages_Controllers>();
                    break;
                case CAMPOS.DS_CONTROLLER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_controller).AsQueryable<webpages_Controllers>();
                    else entity = entity.OrderByDescending(e => e.ds_controller).AsQueryable<webpages_Controllers>();
                    break;
                case CAMPOS.NM_CONTROLLER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nm_controller).AsQueryable<webpages_Controllers>();
                    else entity = entity.OrderByDescending(e => e.nm_controller).AsQueryable<webpages_Controllers>();
                    break;
                case CAMPOS.FL_MENU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.fl_menu).AsQueryable<webpages_Controllers>();
                    else entity = entity.OrderByDescending(e => e.fl_menu).AsQueryable<webpages_Controllers>();
                    break;
                //case CAMPOS.ROLEID:
                //    if (orderby == 0) entity = entity.OrderBy(e => e.webpages_Methods. .id_subController).AsQueryable<webpages_Controllers>();
                //    else entity = entity.OrderByDescending(e => e.id_subController).AsQueryable<webpages_Controllers>();
                //    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_Controllers/Webpages_Controllers
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionWebpages_Controllers = new List<dynamic>();
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
                CollectionWebpages_Controllers = query.Select(e => new
                {

                    id_controller = e.id_controller,
                    ds_controller = e.ds_controller,
                    nm_controller = e.nm_controller,
                    fl_menu = e.fl_menu,
                    id_subController = e.id_subController,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionWebpages_Controllers = query.Select(e => new
                {

                    id_controller = e.id_controller,
                    ds_controller = e.ds_controller,
                    nm_controller = e.nm_controller,
                    fl_menu = e.fl_menu,
                    id_subController = e.id_subController,
                }).ToList<dynamic>();
            }
            else if (colecao == 2)
            {
                List<int> sub2List = new List<int>();

                CollectionWebpages_Controllers = query
                .Where(e => e.id_subController == null)
                .Where(e => e.id_controller >= 50)
                .OrderBy(e => e.ds_controller)
                .Select(e => new
                {

                    id_controller = e.id_controller,
                    ds_controller = e.ds_controller,
                    fl_menu = e.fl_menu,
                    methods = e.webpages_Methods
                                                .OrderBy(m => m.ds_method)
                                                .Select(m => new
                                                {
                                                    id_method = m.id_method,
                                                    ds_method = m.ds_method,
                                                })
                                                .ToList<dynamic>(),
                    subControllers = _db.webpages_Controllers
                                                             .Where(sub => sub.id_subController == e.id_controller)
                                                             .OrderBy(sub => sub.ds_controller)
                                                             .Select(sub => new
                                                             {
                                                                 id_controller = sub.id_controller,
                                                                 ds_controller = sub.ds_controller,
                                                                 fl_menu = sub.fl_menu,
                                                                 methods = sub.webpages_Methods
                                                                             .OrderBy(ms => ms.ds_method)
                                                                             .Select(ms => new
                                                                             {
                                                                                 id_method = ms.id_method,
                                                                                 ds_method = ms.ds_method,
                                                                             }),
                                                                 subControllers = _db.webpages_Controllers
                                                                                      .OrderBy(sub2 => sub2.ds_controller)
                                                                                      .Where(sub2 => sub2.id_subController == sub.id_controller)
                                                                                      .Select(sub2 => new
                                                                                      {
                                                                                          id_controller = sub2.id_controller,
                                                                                          ds_controller = sub2.ds_controller,
                                                                                          fl_menu = sub2.fl_menu,
                                                                                          methods = sub2.webpages_Methods
                                                                                                         .OrderBy(ms2 => ms2.ds_method)
                                                                                                         .Select(ms2 => new
                                                                                                         {
                                                                                                             id_method = ms2.id_method,
                                                                                                             ds_method = ms2.ds_method,
                                                                                                         }),
                                                                                          subControllers = sub2List
                                                                                      }).ToList<dynamic>()


                                                             }).ToList<dynamic>()
                }).ToList<dynamic>();

                retorno.TotalDeRegistros = CollectionWebpages_Controllers.Count;
            }
            else if (colecao == 3)
            {
                List<int> sub2List = new List<int>();
                //case CAMPOS.ROLEID:
                Int32 roleId = Convert.ToInt32(queryString[((int)CAMPOS.ROLEID).ToString()]);
                //query = query.Where(e => e.id_subController.Equals(roleId)).AsQueryable<webpages_Controllers>();

                CollectionWebpages_Controllers = query
                .OrderBy(e => e.ds_controller)
                .Where(e => e.id_subController == null)
                .Where(e => e.id_controller >= 50)
                .Select(e => new
                {

                    id_controller = e.id_controller,
                    ds_controller = e.ds_controller,
                    principal = _db.webpages_Permissions
                                    .Where(p => p.id_roles == roleId)
                                    .Where(p => p.webpages_Methods.id_controller == e.id_controller)
                                    .Where(p => p.fl_principal == true).ToList<webpages_Permissions>().Count > 0,
                    methods = e.webpages_Methods
                                                .OrderBy(m => m.ds_method)
                                                .Select(m => new 
                                                                 {
                                                                     id_method = m.id_method,
                                                                     ds_method = m.ds_method,
                                                                     selecionado = (m.webpages_Permissions.Where(p => p.id_roles == roleId).ToList().Count > 0) ? true : false
                                                                 })
                                                .ToList<dynamic>(),
                    subControllers = _db.webpages_Controllers
                                                             .OrderBy(sub => sub.ds_controller)
                                                             .Where(sub => sub.id_subController == e.id_controller)
                                                             .Select(sub => new
                                                                {
                                                                    id_controller = sub.id_controller,
                                                                    ds_controller = sub.ds_controller,
                                                                    principal = _db.webpages_Permissions
                                                                                    .Where(p => p.id_roles == roleId)
                                                                                    .Where(p => p.webpages_Methods.id_controller == sub.id_controller)
                                                                                    .Where(p => p.fl_principal == true).ToList<webpages_Permissions>().Count > 0,
                                                                    methods = sub.webpages_Methods
                                                                                .OrderBy(ms => ms.ds_method)
                                                                                .Select(ms => new
                                                                                {
                                                                                    id_method = ms.id_method,
                                                                                    ds_method = ms.ds_method,
                                                                                    selecionado = (ms.webpages_Permissions.Where(ps => ps.id_roles == roleId).ToList().Count > 0) ? true : false
                                                                                }),
                                                                    subControllers = _db.webpages_Controllers
                                                                                         .OrderBy(sub2 => sub2.ds_controller)
                                                                                         .Where(sub2 => sub2.id_subController == sub.id_controller)
                                                                                         .Select(sub2 => new
                                                                                         {
                                                                                             id_controller = sub2.id_controller,
                                                                                             ds_controller = sub2.ds_controller,
                                                                                             principal = _db.webpages_Permissions
                                                                                                            .Where(p => p.id_roles == roleId)
                                                                                                            .Where(p => p.webpages_Methods.id_controller == sub2.id_controller)
                                                                                                            .Where(p => p.fl_principal == true).ToList<webpages_Permissions>().Count > 0,
                                                                                             methods = sub2.webpages_Methods
                                                                                                            .OrderBy(ms2 => ms2.ds_method)
                                                                                                            .Select(ms2 => new
                                                                                                            {
                                                                                                                id_method = ms2.id_method,
                                                                                                                ds_method = ms2.ds_method,
                                                                                                                selecionado = (ms2.webpages_Permissions.Where(ps2 => ps2.id_roles == roleId).ToList().Count > 0) ? true : false
                                                                                                            }),
                                                                                             subControllers = sub2List
                                                                                         }).ToList<dynamic>()


                                                                }).ToList<dynamic>()
                }).ToList<dynamic>();

                retorno.TotalDeRegistros = CollectionWebpages_Controllers.Count;
            }
            else if (colecao == 4)
            {
                Int32 IdController = Convert.ToInt32(queryString[((int)CAMPOS.ID_CONTROLLER).ToString()]);
                Int32 IdUser = Permissoes.GetIdUser(token);


                List<dynamic> List = _db.webpages_UsersInRoles.Where(r => r.UserId == IdUser)
                                                    .Where(r => r.RoleId > 50)
                                                    .Select(r => new
                                                    {
                                                        methods = _db.webpages_Permissions
                                                                         .Where(p => p.id_roles == r.RoleId)
                                                                         .Where(p => p.webpages_Methods.webpages_Controllers.id_controller == IdController)
                                                                         .Select(p => new { ds_method = p.webpages_Methods.ds_method, id_method = p.webpages_Methods.id_method })
                                                                         .ToList<dynamic>()
                                                    }
                                                            ).ToList<dynamic>();

                CollectionWebpages_Controllers.Add(List[0].methods);

                retorno.TotalDeRegistros = CollectionWebpages_Controllers.Count;
            }


            retorno.Registros = CollectionWebpages_Controllers;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Webpages_Controllers
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Models.Object.CadastroController param)
        {
            if (param.Webpagescontrollers.nm_controller == null)
                param.Webpagescontrollers.nm_controller = param.Webpagescontrollers.ds_controller;

            _db.webpages_Controllers.Add(param.Webpagescontrollers);
            _db.SaveChanges();

            if (param.Methodspadrao)
            {
                webpages_Methods put = new webpages_Methods { ds_method = "Atualização", id_controller = param.Webpagescontrollers.id_controller };
                webpages_Methods post = new webpages_Methods { ds_method = "Cadastro", id_controller = param.Webpagescontrollers.id_controller };
                webpages_Methods get = new webpages_Methods { ds_method = "Leitura", id_controller = param.Webpagescontrollers.id_controller };
                webpages_Methods delete = new webpages_Methods { ds_method = "Remoção", id_controller = param.Webpagescontrollers.id_controller };
                //webpages_Methods filtroempresa = new webpages_Methods { ds_method = "Filtro Empresa", id_controller = param.Webpagescontrollers.id_controller };

                GatewayWebpagesMethods.Add(token, put);
                GatewayWebpagesMethods.Add(token, post);
                GatewayWebpagesMethods.Add(token, get);
                GatewayWebpagesMethods.Add(token, delete);
                //GatewayWebpagesMethods.Add(token, filtroempresa);
            }

            return param.Webpagescontrollers.id_controller;
        }


        /// <summary>
        /// Apaga uma Webpages_Controllers
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_controller)
        {
            List<Int32> subControllers = _db.webpages_Controllers
                                            .Where(e => e.id_subController == id_controller)
                                            .Select(e => e.id_controller)
                                            .ToList<Int32>();

            foreach (var item in subControllers)
            {
                List<Int32> subControllers2 = _db.webpages_Controllers
                                                .Where(e => e.id_subController == item)
                                                .Select(e => e.id_controller)
                                                .ToList<Int32>();

                foreach (var subItem in subControllers2)
                {
                    List<Int32> subControllers3 = _db.webpages_Controllers
                                                    .Where(e => e.id_subController == subItem)
                                                    .Select(e => e.id_controller)
                                                    .ToList<Int32>();
                    foreach (var subItem2 in subControllers3)
	                {
                        DeleteSubController(token, subItem2);
	                }

                    DeleteSubController(token, subItem);
                }
                DeleteSubController(token, item);
            }
            DeleteSubController(token, id_controller);
        }


                /// <summary>
        /// Apaga uma Webpages_Controllers
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private static void DeleteSubController(string token, Int32 id_controller)
        {
            GatewayWebpagesMethods.DeleteControllerMethods(token, id_controller);
            _db.webpages_Controllers.RemoveRange( _db.webpages_Controllers.Where(e => e.id_controller == id_controller ) );
            _db.SaveChanges();
        }


        /// <summary>
        /// Altera webpages_Controllers
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_Controllers param)
        {
            webpages_Controllers value = _db.webpages_Controllers
                    .Where(e => e.id_controller.Equals(param.id_controller))
                    .First<webpages_Controllers>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            //if (param.id_controller != null && param.id_controller != value.id_controller)
            //    value.id_controller = param.id_controller;
            if (param.ds_controller != null && param.ds_controller != value.ds_controller)
                value.ds_controller = param.ds_controller;
            if (param.nm_controller == null && param.ds_controller != null && param.ds_controller != value.ds_controller)
                value.nm_controller = param.ds_controller;
            else if (param.nm_controller != null && param.nm_controller != value.nm_controller)
                value.nm_controller = param.nm_controller;
            if (param.fl_menu != null && param.fl_menu != value.fl_menu)
                value.fl_menu = param.fl_menu;
            if (param.id_subController != null && param.id_subController != value.id_subController)
            {
                if (param.id_subController == -1)
                    value.id_subController = null;
                else
                    value.id_subController = param.id_subController;
            }
            _db.SaveChanges();

        }

    }
}
