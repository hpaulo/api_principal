using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

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

            ROLELEVEL = 103,

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
                    case CAMPOS.ROLELEVEL:
                        Int32 rolelevel = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.RoleLevel == rolelevel).AsQueryable<webpages_Roles>();
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
                case CAMPOS.ROLELEVEL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.RoleLevel).AsQueryable<webpages_Roles>();
                    else entity = entity.OrderByDescending(e => e.RoleLevel).AsQueryable<webpages_Roles>();
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
            try
            {
                //DECLARAÇÕES
                List<dynamic> CollectionWebpages_Roles = new List<dynamic>();
                Retorno retorno = new Retorno();

                string outValue = null;
                Boolean FiltroRoleName = false;
                if (colecao == 0 && queryString.TryGetValue("" + (int)CAMPOS.ROLENAME, out outValue))
                    FiltroRoleName = !queryString["" + (int)CAMPOS.ROLENAME].Contains("%");

                // GET QUERY
                var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

                if (!FiltroRoleName)
                {
                    // só exibe a partir do RoleId 51 e os que tiverem RoleLevel no mínimo igual ao RoleLevelMin
                    Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
                    query = query.Where(e => e.RoleId > 50 && e.RoleLevel >= RoleLevelMin).AsQueryable<webpages_Roles>();
                }

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
                        RoleLevels = new { LevelId = e.webpages_RoleLevels.LevelId, LevelName = e.webpages_RoleLevels.LevelName }
                    }).ToList<dynamic>();
                }
                else if (colecao == 0)
                {
                    CollectionWebpages_Roles = query
                    .Select(e => new
                    {

                        RoleId = e.RoleId,
                        RoleName = e.RoleName,
                        RoleLevel = e.RoleLevel,
                    }).ToList<dynamic>();
                }
                else if (colecao == 2)
                {
                    CollectionWebpages_Roles = query
                    .Select(e => new
                    {

                        RoleId = e.RoleId,
                        RoleName = e.RoleName,
                        RoleLevels = new { LevelId = e.webpages_RoleLevels.LevelId, LevelName = e.webpages_RoleLevels.LevelName },
                        PaginaInicial = (e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.id_subController != null
                                         && e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.webpages_Controllers2.id_subController != null ?
                                        e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.webpages_Controllers2.webpages_Controllers2.ds_controller
                                        + " > " : "") +
                                        (e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.id_subController != null ?
                                        e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.webpages_Controllers2.ds_controller
                                        + " > " : "") +
                                        e.webpages_Permissions.Where(p => p.fl_principal == true).FirstOrDefault().webpages_Methods.webpages_Controllers.ds_controller
                    }).ToList<dynamic>();
                }
                else if (colecao == 3)
                {
                    List<int> sub2List = new List<int>();
                    CollectionWebpages_Roles = query
                    .Select(r => new
                    {

                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        RoleLevels = new { LevelId = r.webpages_RoleLevels.LevelId, LevelName = r.webpages_RoleLevels.LevelName },
                        Controllers = _db.webpages_Permissions
                        .Where(e => e.id_roles == r.RoleId)
                        .GroupBy(e => new { e.webpages_Methods.webpages_Controllers })
                        .Where(e => e.Key.webpages_Controllers.id_subController == null)
                        .Where(e => e.Key.webpages_Controllers.id_controller >= 50)
                        .OrderBy(e => e.Key.webpages_Controllers.ds_controller)
                        .Select(e => new
                        {

                            id_controller = e.Key.webpages_Controllers.id_controller,
                            ds_controller = e.Key.webpages_Controllers.ds_controller,
                            principal = _db.webpages_Permissions
                                            .Where(p => p.id_roles == r.RoleId)
                                            .Where(p => p.webpages_Methods.id_controller == e.Key.webpages_Controllers.id_controller)
                                            .Where(p => p.fl_principal == true).ToList<webpages_Permissions>().Count > 0,
                            methods = e.Key.webpages_Controllers.webpages_Methods
                                                        .OrderBy(m => m.ds_method)
                                                        .Where(m => m.webpages_Permissions.Where(p => p.id_roles == r.RoleId).ToList().Count > 0)
                                                        .Select(m => new
                                                                         {
                                                                             id_method = m.id_method,
                                                                             ds_method = m.ds_method,
                                                                         })
                                                        .ToList<dynamic>(),
                            subControllers = _db.webpages_Permissions
                                                    .Where(sub => sub.id_roles == r.RoleId)
                                                    .GroupBy(sub => new { sub.webpages_Methods.webpages_Controllers })
                                                    .Where(sub => sub.Key.webpages_Controllers.id_subController == e.Key.webpages_Controllers.id_controller)
                                                    .OrderBy(sub => sub.Key.webpages_Controllers.ds_controller)
                                                    .Select(sub => new
                                                    {
                                                        id_controller = sub.Key.webpages_Controllers.id_controller,
                                                        ds_controller = sub.Key.webpages_Controllers.ds_controller,
                                                        principal = _db.webpages_Permissions
                                                                        .Where(p => p.id_roles == r.RoleId)
                                                                        .Where(p => p.webpages_Methods.id_controller == sub.Key.webpages_Controllers.id_controller)
                                                                        .Where(p => p.fl_principal == true).ToList<webpages_Permissions>().Count > 0,
                                                        methods = sub.Key.webpages_Controllers.webpages_Methods
                                                                    .OrderBy(ms => ms.ds_method)
                                                                    .Where(ms => ms.webpages_Permissions.Where(ps => ps.id_roles == r.RoleId).ToList().Count > 0)
                                                                    .Select(ms => new
                                                                    {
                                                                        id_method = ms.id_method,
                                                                        ds_method = ms.ds_method,
                                                                    }).ToList<dynamic>(),
                                                        subControllers = _db.webpages_Permissions
                                                                                .Where(sub2 => sub2.id_roles == r.RoleId)
                                                                                .GroupBy(sub2 => new { sub2.webpages_Methods.webpages_Controllers })
                                                                                .Where(sub2 => sub2.Key.webpages_Controllers.id_subController == sub.Key.webpages_Controllers.id_controller)
                                                                                .OrderBy(sub2 => sub2.Key.webpages_Controllers.ds_controller)
                                                                                .Select(sub2 => new
                                                                                {
                                                                                    id_controller = sub2.Key.webpages_Controllers.id_controller,
                                                                                    ds_controller = sub2.Key.webpages_Controllers.ds_controller,
                                                                                    principal = _db.webpages_Permissions
                                                                                                .Where(p => p.id_roles == r.RoleId)
                                                                                                .Where(p => p.webpages_Methods.id_controller == sub2.Key.webpages_Controllers.id_controller)
                                                                                                .Where(p => p.fl_principal == true).ToList<webpages_Permissions>().Count > 0,
                                                                                    methods = sub2.Key.webpages_Controllers.webpages_Methods
                                                                                                .OrderBy(ms2 => ms2.ds_method)
                                                                                                .Where(ms2 => ms2.webpages_Permissions.Where(ps2 => ps2.id_roles == r.RoleId).ToList().Count > 0)
                                                                                                .Select(ms2 => new
                                                                                                {
                                                                                                    id_method = ms2.id_method,
                                                                                                    ds_method = ms2.ds_method,
                                                                                                }).ToList<dynamic>(),
                                                                                    subControllers = sub2List
                                                                                }).ToList<dynamic>()


                                                    }).ToList<dynamic>()
                        }).ToList<dynamic>()

                    }).ToList<dynamic>();

                }

                retorno.Registros = CollectionWebpages_Roles;

                return retorno;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar role" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Adiciona nova Webpages_Roles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, webpages_Roles param)
        {
            try
            {
                // Por segurança, só deixa alterar se o usuário tiver permissão para setar aquela role 
                Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
                if (param.RoleLevel < RoleLevelMin) throw new Exception("401"); // não possui autorização para criar um privilégio com esse RoleLevel
                if (_db.webpages_Roles.Where(r => r.RoleName.ToUpper().Equals(param.RoleName.ToUpper())).FirstOrDefault() != null)
                    throw new Exception("Já existe uma role com o nome '" + param.RoleName.ToUpper() + "'"); // já existe um privilégio com esse nome
                _db.webpages_Roles.Add(param);
                _db.SaveChanges();
                return param.RoleId;
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar role" : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma Webpages_Roles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 RoleId)
        {
            try
            {
                webpages_Roles role = _db.webpages_Roles.Where(r => r.RoleId == RoleId).FirstOrDefault();

                if (role == null) throw new Exception("Role inexistente"); // não existe role com o Id informado

                Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
                //if (role.RoleName.ToUpper().Equals("COMERCIAL") || role.RoleLevel < RoleLevelMin) throw new Exception("401"); // não possui autorização para remover o privilégio
                if (Permissoes.isAtosRoleVendedor(role) || role.RoleLevel < RoleLevelMin) throw new Exception("401"); // não possui autorização para remover o privilégio


                GatewayWebpagesPermissions.Delete(token, RoleId);
                GatewayWebpagesUsersInRoles.Delete(token, RoleId, true);
                _db.webpages_Roles.Remove(_db.webpages_Roles.Where(e => e.RoleId.Equals(RoleId)).First());
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar role" : erro);
                }
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Altera webpages_Roles
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, webpages_Roles param)
        {
            try
            {
                webpages_Roles value = _db.webpages_Roles
                        .Where(e => e.RoleId == param.RoleId)
                        .First<webpages_Roles>();

                if (value == null) throw new Exception("Role inexistente"); // não existe role com o Id informado


                if (param.RoleName != null && param.RoleName != value.RoleName)
                {
                    //if(value.RoleName.ToUpper().Equals("COMERCIAL")) throw new Exception("401"); // não possui autorização para alterar o nome privilégio comercial
                    if (Permissoes.isAtosRoleVendedor(value)) throw new Exception("401"); // não possui autorização para alterar o nome privilégio comercial
                    value.RoleName = param.RoleName;
                }
                if (param.RoleLevel != value.RoleLevel)
                {
                    // Por segurança, só deixa alterar se o usuário tiver permissão para setar aquela role 
                    Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
                    if (param.RoleLevel >= RoleLevelMin)
                        value.RoleLevel = param.RoleLevel;
                    else throw new Exception("401"); // não possui autorização para criar um privilégio com esse RoleLevel
                }
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar role" : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
