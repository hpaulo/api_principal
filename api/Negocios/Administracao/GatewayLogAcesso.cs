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
		public class GatewayLogAcesso
		{
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayLogAcesso()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDUSERS = 100,
            IDCONTROLLER = 101,
            IDMETHOD = 102,
            DTACESSO = 103,
            FLMOBILE = 104,
            DSUSERAGENT = 105,

            // RELACIONAMENTOS
            DS_LOGIN = 201,
            ID_GRUPO = 203,
            NU_CNPJEMPRESA = 204,

            DS_CONTROLLER = 301,

        };

        /// <summary>
        /// Get LogAcesso/LogAcesso
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<LogAcesso1> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.LogAcesso1.AsQueryable<LogAcesso1>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

                // ADICIONA OS FILTROS A QUERY
                foreach (var item in queryString)
                {
                    int key = Convert.ToInt16(item.Key);
                    CAMPOS filtroEnum = (CAMPOS)key;
                    switch (filtroEnum)
                    {
						case CAMPOS.IDUSERS:
							Int32 idUsers = Convert.ToInt32(item.Value);
							entity = entity.Where(e => e.idUsers.Equals(idUsers)).AsQueryable<LogAcesso1>();
						    break;
						case CAMPOS.IDCONTROLLER:
							Int32 idController = Convert.ToInt32(item.Value);
							entity = entity.Where(e => e.idController.Equals(idController)).AsQueryable<LogAcesso1>();
						    break;
						case CAMPOS.IDMETHOD:
							Int32 idMethod = Convert.ToInt32(item.Value);
							entity = entity.Where(e => e.idMethod.Equals(idMethod)).AsQueryable<LogAcesso1>();
						    break;
                        case CAMPOS.DTACESSO:
                            DateTime dtAcesso = Convert.ToDateTime(item.Value);
                            entity = entity.Where(e => e.dtAcesso.Equals(dtAcesso)).AsQueryable<LogAcesso1>();
                            break;
                        case CAMPOS.FLMOBILE:
                            bool flMobile = Convert.ToBoolean(item.Value);
                            entity = entity.Where(e => e.flMobile.Equals(flMobile)).AsQueryable<LogAcesso1>();
                            break;
                        case CAMPOS.DSUSERAGENT:
                            string dsUserAgent = Convert.ToString(item.Value);
                            entity = entity.Where(e => e.dsUserAgent.Equals(dsUserAgent)).AsQueryable<LogAcesso1>();
                            break;

                        // PERSONALIZADO
                        case CAMPOS.DS_LOGIN:
                            string ds_login = Convert.ToString(item.Value);
                            if (ds_login.Contains("%")) // usa LIKE
                            {
                                string busca = ds_login.Replace("%", "").ToString();
                                entity = entity.Where(e => e.webpages_Users.ds_login.Contains(busca)).AsQueryable<LogAcesso1>();
                            }
                            else
                            entity = entity.Where(e => e.webpages_Users.ds_login.Equals(ds_login)).AsQueryable<LogAcesso1>();
                            break;
                        case CAMPOS.ID_GRUPO:
                            Int32 id_grupo = Convert.ToInt32(item.Value);
							entity = entity.Where(e => e.webpages_Users.id_grupo == id_grupo).AsQueryable<LogAcesso1>();
						    break;

                        case CAMPOS.DS_CONTROLLER:
                            string ds_controller = Convert.ToString(item.Value);
                            if (ds_controller.Contains("%")) // usa LIKE
                            {
                                string busca = ds_controller.Replace("%", "").ToString();
                                entity = entity.Where(e => e.webpages_Controllers.ds_controller.Contains(busca) || 
                                                      (e.webpages_Controllers.id_subController != null && e.webpages_Controllers.webpages_Controllers2.ds_controller.Contains(busca)) ||
                                                      (e.webpages_Controllers.id_subController != null && e.webpages_Controllers.webpages_Controllers2.id_subController != null && e.webpages_Controllers.webpages_Controllers2.webpages_Controllers2.ds_controller.Contains(busca)))
                                               .AsQueryable<LogAcesso1>();
                            }
                            else
                                entity = entity.Where(e => e.webpages_Controllers.ds_controller.Equals(ds_controller) ||
                                                      (e.webpages_Controllers.id_subController != null && e.webpages_Controllers.webpages_Controllers2.ds_controller.Equals(ds_controller)) ||
                                                      (e.webpages_Controllers.id_subController != null && e.webpages_Controllers.webpages_Controllers2.id_subController != null && e.webpages_Controllers.webpages_Controllers2.webpages_Controllers2.ds_controller.Equals(ds_controller)))
                                               .AsQueryable<LogAcesso1>();
                        break;
                }
                }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
                // ADICIONA A ORDENAÇÃO A QUERY
                CAMPOS filtro = (CAMPOS)campo;
                switch (filtro)
                {

						case CAMPOS.IDUSERS: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.idUsers).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.idUsers).AsQueryable<LogAcesso1>();
						    break;
						case CAMPOS.IDCONTROLLER: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.idController).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.idController).AsQueryable<LogAcesso1>();
						    break;
						case CAMPOS.IDMETHOD: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.idMethod).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.idMethod).AsQueryable<LogAcesso1>();
						    break;
						case CAMPOS.DTACESSO: 
							if (orderby == 0)  entity = entity.OrderBy(e => e.dtAcesso).ThenBy(e => e.webpages_Users.ds_login).AsQueryable<LogAcesso1>();
							else entity = entity.OrderByDescending(e =>  e.dtAcesso).ThenBy(e => e.webpages_Users.ds_login).AsQueryable<LogAcesso1>();
						    break;
                        case CAMPOS.FLMOBILE:
                            if (orderby == 0) entity = entity.OrderBy(e => e.flMobile).AsQueryable<LogAcesso1>();
                            else entity = entity.OrderByDescending(e => e.flMobile).AsQueryable<LogAcesso1>();
                         break;
                        case CAMPOS.DSUSERAGENT:
                            if (orderby == 0) entity = entity.OrderBy(e => e.dsUserAgent).AsQueryable<LogAcesso1>();
                            else entity = entity.OrderByDescending(e => e.dsUserAgent).AsQueryable<LogAcesso1>();
                            break;

                        // PERSONALIZADO
                        case CAMPOS.DS_LOGIN:
                            if (orderby == 0) entity = entity.OrderBy(e => e.webpages_Users.ds_login).ThenByDescending(e => e.dtAcesso).AsQueryable<LogAcesso1>();
                            else entity = entity.OrderByDescending(e => e.webpages_Users.ds_login).ThenBy(e => e.dtAcesso).AsQueryable<LogAcesso1>();
                            break;
                        case CAMPOS.ID_GRUPO:
                            if (orderby == 0) entity = entity.OrderBy(e => e.webpages_Users.id_grupo).ThenByDescending(e => e.dtAcesso).AsQueryable<LogAcesso1>();
                            else entity = entity.OrderByDescending(e => e.webpages_Users.id_grupo).ThenBy(e => e.dtAcesso).AsQueryable<LogAcesso1>();
                            break;

                        case CAMPOS.DS_CONTROLLER:
                            if (orderby == 0) entity = entity.OrderBy(e => e.webpages_Controllers.ds_controller).ThenByDescending(e => e.dtAcesso).AsQueryable<LogAcesso1>();
                            else entity = entity.OrderByDescending(e => e.webpages_Controllers.ds_controller).ThenBy(e => e.dtAcesso).AsQueryable<LogAcesso1>();
                            break;

                }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna LogAcesso/LogAcesso
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionLogAcesso = new List<dynamic>();
            Retorno retorno = new Retorno();
            string outValue = null;

            Int32 IdGrupo = Permissoes.GetIdGrupo(token);
            if (IdGrupo != 0)
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.ID_GRUPO, out outValue))
                    queryString["" + (int)CAMPOS.ID_GRUPO] = IdGrupo.ToString();
                else
                    queryString.Add("" + (int)CAMPOS.ID_GRUPO, IdGrupo.ToString());
            }
            string CnpjEmpresa = Permissoes.GetCNPJEmpresa(token);
            if (CnpjEmpresa != "")
            {
                if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJEMPRESA, out outValue))
                    queryString["" + (int)CAMPOS.NU_CNPJEMPRESA] = CnpjEmpresa;
                else
                    queryString.Add("" + (int)CAMPOS.NU_CNPJEMPRESA, CnpjEmpresa);
            }

            // GET QUERY
            var query = getQuery( colecao, campo, orderBy, pageSize, pageNumber, queryString);

            // Restringe consulta pelo perfil do usuário logado
            Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
            bool isAtosVendedor = Permissoes.isAtosRoleVendedor(token);
            if (IdGrupo == 0 && isAtosVendedor)
            {
                // Perfil Comercial tem uma carteira de clientes específica
                List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token);
                query = query.Where(e => e.webpages_Users.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin
                                            && e.webpages_Users.id_grupo != null && listaIdsGruposEmpresas.Contains(e.webpages_Users.id_grupo ?? -1)).AsQueryable<LogAcesso1>();
            }
            else if (Permissoes.isAtosRole(token) && !isAtosVendedor)
                // ATOS de nível mais alto: Lista os usuários que não tem role associada ou aqueles de RoleLevel permitido para o usuário logado consultar
                query = query.Where(e => e.webpages_Users.webpages_Membership.webpages_UsersInRoles.ToList<dynamic>().Count == 0 || e.webpages_Users.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin).AsQueryable<LogAcesso1>();
            else
                // Só exibe os usuários de RoleLevelMin
                query = query.Where(e => e.webpages_Users.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin).AsQueryable<LogAcesso1>();

            // TOTAL DE REGISTROS
            retorno.TotalDeRegistros = query.Count();


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
                CollectionLogAcesso = query.Select(e => new
                {

					idUsers = e.idUsers,
					idController = e.idController,
					idMethod = e.idMethod,
					dtAcesso = e.dtAcesso,
                    flMobile = e.flMobile,
                    dsUserAgent = e.dsUserAgent
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionLogAcesso = query.Select(e => new
                {

					idUsers = e.idUsers,
					idController = e.idController,
					idMethod = e.idMethod,
					dtAcesso = e.dtAcesso,
                    flMobile = e.flMobile,
                    dsUserAgent = e.dsUserAgent
                }).ToList<dynamic>();
            }
            else if (colecao == 2)
            {
                CollectionLogAcesso = query.Select(e => new
                {

                    webpagesusers = new { id_users = e.idUsers,
                                          ds_login = e.webpages_Users.ds_login
                                        },
                    controller = new { id_controller = e.idController,
                                       ds_controller = e.webpages_Controllers != null && e.idController > 50 ?
                                                       (e.webpages_Controllers.id_subController != null && e.webpages_Controllers.webpages_Controllers2.id_subController != null ?
                                                       e.webpages_Controllers.webpages_Controllers2.webpages_Controllers2.ds_controller + " > " : "") +
                                                       (e.webpages_Controllers.id_subController != null ?
                                                       e.webpages_Controllers.webpages_Controllers2.ds_controller + " > " : "") +
                                                       e.webpages_Controllers.ds_controller : 
                                                       "Login",
                    },
                    dtAcesso = e.dtAcesso,
                    dsAplicacao = e.flMobile ? "Mobile" : "Portal",
                    dsUserAgent = e.dsUserAgent
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionLogAcesso;

            return retorno;
        }


	
        /// <summary>
        /// Adiciona nova LogAcesso
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, LogAcesso1 param)
        {
            _db.LogAcesso1.Add(param);
            _db.SaveChanges();
            return (Int32)param.idUsers;
        }

        /// <summary>
        /// Adiciona nova LogAcesso
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Add(string token, Int32 idController)
        {
            LogAcesso1 log = new LogAcesso1();
            log.idUsers = Bibliotecas.Permissoes.GetIdUser(token);
            log.idController = idController;
            log.dtAcesso = DateTime.Now;
            log.dsUserAgent = HttpContext.Current.Request.UserAgent;
            log.flMobile = Bibliotecas.Device.IsMobile();

            _db.LogAcesso1.Add(log);
            _db.SaveChanges();
        }



        /// <summary>
        /// Apaga uma LogAcesso
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idUsers, Int32 idController, Int32 idMethod)
        {
            _db.LogAcesso1.Remove(
                                    _db.LogAcesso1.Where(e => e.idUsers.Equals(idUsers))
                                                .Where(e => e.idController.Equals(idController))
                                                .Where(e => e.idMethod.Equals(idMethod)).First()
                                 );
            _db.SaveChanges();
        }
	


        /// <summary>
        /// Altera LogAcesso
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, LogAcesso1 param)
        {
            LogAcesso1 value = _db.LogAcesso1
                    .Where(e => e.idUsers.Equals(param.idUsers))
                    .Where(e => e.idController.Equals(param.idController))
                    .Where(e => e.idMethod.Equals(param.idMethod))
                    .First<LogAcesso1>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS
            
            
                if (param.idUsers != null && param.idUsers != value.idUsers)
					value.idUsers = param.idUsers;
                if (param.idController != null && param.idController != value.idController)
					value.idController = param.idController;
                if (param.idMethod != null && param.idMethod != value.idMethod)
					value.idMethod = param.idMethod;
                if (param.dtAcesso != null && param.dtAcesso != value.dtAcesso)
					value.dtAcesso = param.dtAcesso;
            _db.SaveChanges();

        }

    }
}