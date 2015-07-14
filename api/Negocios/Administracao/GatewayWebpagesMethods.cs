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
    public class GatewayWebpagesMethods
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesMethods()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID_METHOD = 100,
            DS_METHOD = 101,
            NM_METHOD = 102,
            FL_MENU = 103,
            ID_CONTROLLER = 104,

        };

        /// <summary>
        /// Get Webpages_Methods/Webpages_Methods
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_Methods> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_Methods.AsQueryable<webpages_Methods>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.ID_METHOD:
                        Int32 id_method = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_method.Equals(id_method)).AsQueryable<webpages_Methods>();
                        break;
                    case CAMPOS.DS_METHOD:
                        string ds_method = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.ds_method.Equals(ds_method)).AsQueryable<webpages_Methods>();
                        break;
                    case CAMPOS.NM_METHOD:
                        string nm_method = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nm_method.Equals(nm_method)).AsQueryable<webpages_Methods>();
                        break;
                    case CAMPOS.FL_MENU:
                        Boolean fl_menu = Convert.ToBoolean(item.Value);
                        entity = entity.Where(e => e.fl_menu.Equals(fl_menu)).AsQueryable<webpages_Methods>();
                        break;
                    case CAMPOS.ID_CONTROLLER:
                        Int32 id_controller = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_controller.Equals(id_controller)).AsQueryable<webpages_Methods>();
                        break;

                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.ID_METHOD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_method).AsQueryable<webpages_Methods>();
                    else entity = entity.OrderByDescending(e => e.id_method).AsQueryable<webpages_Methods>();
                    break;
                case CAMPOS.DS_METHOD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_method).AsQueryable<webpages_Methods>();
                    else entity = entity.OrderByDescending(e => e.ds_method).AsQueryable<webpages_Methods>();
                    break;
                case CAMPOS.NM_METHOD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nm_method).AsQueryable<webpages_Methods>();
                    else entity = entity.OrderByDescending(e => e.nm_method).AsQueryable<webpages_Methods>();
                    break;
                case CAMPOS.FL_MENU:
                    if (orderby == 0) entity = entity.OrderBy(e => e.fl_menu).AsQueryable<webpages_Methods>();
                    else entity = entity.OrderByDescending(e => e.fl_menu).AsQueryable<webpages_Methods>();
                    break;
                case CAMPOS.ID_CONTROLLER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_controller).AsQueryable<webpages_Methods>();
                    else entity = entity.OrderByDescending(e => e.id_controller).AsQueryable<webpages_Methods>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_Methods/Webpages_Methods
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionWebpages_Methods = new List<dynamic>();
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
                CollectionWebpages_Methods = query.Select(e => new
                {

                    id_method = e.id_method,
                    ds_method = e.ds_method,
                    nm_method = e.nm_method,
                    fl_menu = e.fl_menu,
                    id_controller = e.id_controller,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionWebpages_Methods = query.Select(e => new
                {

                    id_method = e.id_method,
                    ds_method = e.ds_method,
                    nm_method = e.nm_method,
                    fl_menu = e.fl_menu,
                    id_controller = e.id_controller,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionWebpages_Methods;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Webpages_Methods
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_Methods param)
        {
            if (param.nm_method == null)
                param.nm_method = param.ds_method;

            param.fl_menu = false;
            _db.webpages_Methods.Add(param);
            _db.SaveChanges();
            return param.id_method;
        }


        /// <summary>
        /// Apaga uma Webpages_Methods
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 id_method)
        {
            GatewayWebpagesPermissions.DeleteMethod(token, id_method);

            _db.webpages_Methods.RemoveRange(_db.webpages_Methods.Where(e => e.id_method.Equals(id_method)));
            _db.SaveChanges();
        }


        /// <summary>
        /// Apaga uma Webpages_Methods
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void DeleteControllerMethods(string token, Int32 id_controller)
        {
            List<Int32> Methods = _db.webpages_Methods
                    .Where(e => e.id_controller == id_controller)
                    .Select(e => e.id_method )
                    .ToList<Int32>();

            foreach (var item in Methods)
            {
                Delete(token, item);
            }

            //GatewayWebpagesPermissions.DeleteMethod(token, id_method);
            //_db.webpages_Methods.RemoveRange( _db.webpages_Methods.Where(e => e.id_controller == id_controller ));
            //_db.SaveChanges();
        }



        /// <summary>
        /// Altera webpages_Methods
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_Methods param)
        {
            webpages_Methods value = _db.webpages_Methods
                    .Where(e => e.id_method.Equals(param.id_method))
                    .First<webpages_Methods>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS

            if (param.ds_method != null && param.ds_method != value.ds_method)
            {
                value.ds_method = param.ds_method;
                value.nm_method = param.ds_method;
            }

            _db.SaveChanges();

        }

    }
}
