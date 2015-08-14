using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using System.Data.Entity.Validation;

namespace api.Negocios.Admin
{
    public class GatewayTbLogAcessoUsuario
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayTbLogAcessoUsuario()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            IDLOGACESSOUSUARIO = 100,
            IDUSER = 101,
            DSURL = 102,
            IDCONTROLLER = 103,
            //IDMETHOD = 104, /* CAMPO REMOVIDO */
            DSPARAMETROS = 104,
            DSFILTROS = 105,
            DTACESSO = 106,
            DSAPLICACAO = 107,
            CODRESPOSTA = 108,
            MSGERRO = 109,
            DSJSON = 110,
            DSUSERAGENT = 111,
            DSMETHOD = 112,

            // WEBPAGESUSERS
            NU_CNPJ = 200,
            DS_LOGIN = 201,
            ID_GRUPO = 203,

            // WEBPAGESCONTROLLERS
            DS_CONTROLLER = 301,
        };

        /// <summary>
        /// Get TbLogAcessoUsuario/TbLogAcessoUsuario
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<tbLogAcessoUsuario> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.tbLogAcessoUsuarios.AsQueryable<tbLogAcessoUsuario>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {
                    case CAMPOS.IDLOGACESSOUSUARIO:
                        Int32 idLogAcessoUsuario = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idLogAcessoUsuario == idLogAcessoUsuario).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.IDUSER:
                        Int32 idUser = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idUser == idUser).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSURL:
                        string dsUrl = Convert.ToString(item.Value);
                        if (dsUrl.Contains("%")) // usa LIKE
                        {
                            string busca = dsUrl.Replace("%", "").ToString();
                            entity = entity.Where(e => e.dsUrl.Contains(busca)).AsQueryable<tbLogAcessoUsuario>();
                        }
                        else
                            entity = entity.Where(e => e.dsUrl.Equals(dsUrl)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.IDCONTROLLER:
                        Int32 idController = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.idController == idController).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSPARAMETROS:
                        string dsParametros = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsParametros.Equals(dsParametros)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSFILTROS:
                        string dsFiltros = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsFiltros.Equals(dsFiltros)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DTACESSO:
                        DateTime dtAcesso = Convert.ToDateTime(item.Value);
                        entity = entity.Where(e => e.dtAcesso.Equals(dtAcesso)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSAPLICACAO:
                        string dsAplicacao = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsAplicacao.Equals(dsAplicacao)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.CODRESPOSTA:
                        Int32 codResposta = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.codResposta == codResposta).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.MSGERRO:
                        string msgErro = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.msgErro.Equals(msgErro)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSJSON:
                        string dsJson = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsJson.Equals(dsJson)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSUSERAGENT:
                        string dsUserAgent = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsUserAgent.Equals(dsUserAgent)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DSMETHOD:
                        string dsMethod = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.dsMethod.Equals(dsMethod)).AsQueryable<tbLogAcessoUsuario>();
                        break;


                    // PERSONALIZADO
                    case CAMPOS.NU_CNPJ:
                        string nu_cnpj = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.webpages_Users.nu_cnpjEmpresa.Equals(nu_cnpj)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.DS_LOGIN:
                        string ds_login = Convert.ToString(item.Value);
                        if (ds_login.Contains("%")) // usa LIKE
                        {
                            string busca = ds_login.Replace("%", "").ToString();
                            entity = entity.Where(e => e.webpages_Users.ds_login.Contains(busca)).AsQueryable<tbLogAcessoUsuario>();
                        }
                        else
                            entity = entity.Where(e => e.webpages_Users.ds_login.Equals(ds_login)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.webpages_Users.id_grupo == id_grupo).AsQueryable<tbLogAcessoUsuario>();
                        break;

                    case CAMPOS.DS_CONTROLLER:
                        string ds_controller = Convert.ToString(item.Value);
                        if (ds_controller.Contains("%")) // usa LIKE
                        {
                            string busca = ds_controller.Replace("%", "").ToString();
                            entity = entity.Where(e => e.webpages_Controllers.ds_controller.Contains(busca)).AsQueryable<tbLogAcessoUsuario>();
                        }
                        else
                            entity = entity.Where(e => e.webpages_Controllers.ds_controller.Equals(ds_controller)).AsQueryable<tbLogAcessoUsuario>();
                        break;
                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {
                case CAMPOS.IDLOGACESSOUSUARIO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idLogAcessoUsuario).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idLogAcessoUsuario).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.IDUSER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idUser).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idUser).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSURL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsUrl).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsUrl).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.IDCONTROLLER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.idController).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.idController).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSPARAMETROS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsParametros).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsParametros).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSFILTROS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsFiltros).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsFiltros).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DTACESSO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dtAcesso).ThenBy(e => e.webpages_Users.ds_login).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dtAcesso).ThenBy(e => e.webpages_Users.ds_login).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSAPLICACAO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsAplicacao).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsAplicacao).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.CODRESPOSTA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.codResposta).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.codResposta).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.MSGERRO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.msgErro).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.msgErro).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSJSON:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsJson).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsJson).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSUSERAGENT:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsUserAgent).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsUserAgent).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.DSMETHOD:
                    if (orderby == 0) entity = entity.OrderBy(e => e.dsMethod).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.dsMethod).AsQueryable<tbLogAcessoUsuario>();
                    break;


                // PERSONALIZADO
                case CAMPOS.DS_LOGIN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.webpages_Users.ds_login).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.webpages_Users.ds_login).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    break;
                case CAMPOS.ID_GRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.webpages_Users.id_grupo).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.webpages_Users.id_grupo).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    break;

                case CAMPOS.DS_CONTROLLER:
                    if (orderby == 0) entity = entity.OrderBy(e => e.webpages_Controllers.ds_controller).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    else entity = entity.OrderByDescending(e => e.webpages_Controllers.ds_controller).ThenByDescending(e => e.dtAcesso).AsQueryable<tbLogAcessoUsuario>();
                    break;
            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna TbLogAcessoUsuario/TbLogAcessoUsuario
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            try
            {
            // Implementar o filtro por Grupo apartir do TOKEN do Usuário
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
                if (queryString.TryGetValue("" + (int)CAMPOS.NU_CNPJ, out outValue))
                    queryString["" + (int)CAMPOS.NU_CNPJ] = CnpjEmpresa;
                else
                    queryString.Add("" + (int)CAMPOS.NU_CNPJ, CnpjEmpresa);
            }


            //DECLARAÇÕES
            List<dynamic> CollectionTbLogAcessoUsuario = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

            // Restringe consulta pelo perfil do usuário logado
            Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
            bool isAtosVendedor = Permissoes.isAtosRoleVendedor(token);
            if (IdGrupo == 0 && isAtosVendedor)
            {
                // Perfil Comercial tem uma carteira de clientes específica
                List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token);
                query = query.Where(e => e.webpages_Users.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin
                                            && e.webpages_Users.id_grupo != null && listaIdsGruposEmpresas.Contains(e.webpages_Users.id_grupo ?? -1)).AsQueryable<tbLogAcessoUsuario>();
            }
            else if (Permissoes.isAtosRole(token) && !isAtosVendedor)
                // ATOS de nível mais alto: Lista os usuários que não tem role associada ou aqueles de RoleLevel permitido para o usuário logado consultar
                query = query.Where(e => e.webpages_Users.webpages_Membership.webpages_UsersInRoles.ToList<dynamic>().Count == 0 || e.webpages_Users.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin).AsQueryable<tbLogAcessoUsuario>();
            else
                // Só exibe os usuários de RoleLevelMin
                query = query.Where(e => e.webpages_Users.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin).AsQueryable<tbLogAcessoUsuario>();

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
                CollectionTbLogAcessoUsuario = query.Select(e => new
                {

                    idLogAcessoUsuario = e.idLogAcessoUsuario,
                    idUser = e.idUser,
                    dsUrl = e.dsUrl,
                    idController = e.idController,
                    dsParametros = e.dsParametros,
                    dsFiltros = e.dsFiltros,
                    dtAcesso = e.dtAcesso,
                    dsAplicacao = e.dsAplicacao,
                    codResposta = e.codResposta,
                    msgErro = e.msgErro,
                    dsJson = e.dsJson,
                    dsUserAgent = e.dsUserAgent,
                    dsMethod = e.dsMethod,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionTbLogAcessoUsuario = query.Select(e => new
                {

                    idLogAcessoUsuario = e.idLogAcessoUsuario,
                    idUser = e.idUser,
                    dsUrl = e.dsUrl,
                    idController = e.idController,
                    dsParametros = e.dsParametros,
                    dsFiltros = e.dsFiltros,
                    dtAcesso = e.dtAcesso,
                    dsAplicacao = e.dsAplicacao,
                    codResposta = e.codResposta,
                    msgErro = e.msgErro,
                    dsJson = e.dsJson,
                    dsUserAgent = e.dsUserAgent,
                    dsMethod = e.dsMethod,

                }).ToList<dynamic>();
            }
            else if (colecao == 2) // [Portal] Acesso de usuários => POST, PUT e DELETE (desenvolvedor)
            {
                CollectionTbLogAcessoUsuario = query.Select(e => new
                {

                    idLogAcessoUsuario = e.idLogAcessoUsuario,
                    webpagesusers = new { id_users = e.idUser,
                                          ds_login = e.webpages_Users.ds_login
                                        },
                    dsUrl = e.dsUrl,
                    dsParametros = e.dsParametros,
                    dsFiltros = e.dsFiltros,
                    dtAcesso = e.dtAcesso,
                    dsAplicacao = e.dsAplicacao.ToUpper() == "M" ? "Mobile" : 
                                  e.dsAplicacao.ToUpper() == "P" ? "Portal" : e.dsAplicacao,
                    codResposta = e.codResposta,
                    msgErro = e.msgErro,
                    dsJson = e.dsJson,
                    dsMethod = e.dsMethod,
                    controller = new
                    {
                        id_controller = e.idController,
                        ds_controller = e.webpages_Controllers != null && e.idController > 50 ?
                                             (e.webpages_Controllers.id_subController != null && e.webpages_Controllers.webpages_Controllers2.id_subController != null ?
                                                e.webpages_Controllers.webpages_Controllers2.webpages_Controllers2.ds_controller + " > " : "") +
                                              (e.webpages_Controllers.id_subController != null ?
                                                       e.webpages_Controllers.webpages_Controllers2.ds_controller + " > " : "") +
                                                       e.webpages_Controllers.ds_controller :
                                                       "Login",

                    },
                    dsUserAgent = e.dsUserAgent,
                }).ToList<dynamic>();
            }

            retorno.Registros = CollectionTbLogAcessoUsuario;

            return retorno;
        }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao listar Log de Acesso de Usuário " : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Adiciona nova TbLogAcessoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, tbLogAcessoUsuario param)
        {
            try
            {
            _db.tbLogAcessoUsuarios.Add(param);
            _db.SaveChanges();
            return param.idLogAcessoUsuario;
        }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao salvar Log de Acesso de Usuário " : erro);
                }
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Apaga uma TbLogAcessoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Delete(string token, Int32 idLogAcessoUsuario)
        {
            try
            {
            _db.tbLogAcessoUsuarios.Remove(_db.tbLogAcessoUsuarios.Where(e => e.idLogAcessoUsuario == idLogAcessoUsuario).First());
            _db.SaveChanges();
        }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao apagar Log de Acesso de Usuário " : erro);
                }
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Altera tbLogAcessoUsuario
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, tbLogAcessoUsuario param)
        {
            try
            {
            tbLogAcessoUsuario value = _db.tbLogAcessoUsuarios
                    .Where(e => e.idLogAcessoUsuario == param.idLogAcessoUsuario)
                    .First<tbLogAcessoUsuario>();

            // OBSERVAÇÂO: VERIFICAR SE EXISTE ALTERAÇÃO NO PARAMETROS


            //if (param.idLogAcessoUsuario != null && param.idLogAcessoUsuario != value.idLogAcessoUsuario)
            //    value.idLogAcessoUsuario = param.idLogAcessoUsuario;
            //if (param.idUser != null && param.idUser != value.idUser)
            //    value.idUser = param.idUser;
            if (param.dsUrl != null && param.dsUrl != value.dsUrl)
                value.dsUrl = param.dsUrl;
            if (param.idController != null && param.idController != value.idController)
                value.idController = param.idController;
            if (param.dsParametros != null && param.dsParametros != value.dsParametros)
                value.dsParametros = param.dsParametros;
            if (param.dsFiltros != null && param.dsFiltros != value.dsFiltros)
                value.dsFiltros = param.dsFiltros;
            if (param.dtAcesso != null && param.dtAcesso != value.dtAcesso)
                value.dtAcesso = param.dtAcesso;
            if (param.dsAplicacao != null && param.dsAplicacao != value.dsAplicacao)
                value.dsAplicacao = param.dsAplicacao;
            if (param.codResposta != value.codResposta)
                value.codResposta = param.codResposta;
            if (param.msgErro != null && param.msgErro != value.msgErro)
                value.msgErro = param.msgErro;
            if (param.dsJson != null && param.dsJson != value.dsJson)
                value.dsJson = param.dsJson;
            if (param.dsMethod != null && param.dsMethod != value.dsMethod)
                value.dsMethod = param.dsMethod;
            _db.SaveChanges();
            }
            catch (Exception e)
            {
                if (e is DbEntityValidationException)
                {
                    string erro = MensagemErro.getMensagemErro((DbEntityValidationException)e);
                    throw new Exception(erro.Equals("") ? "Falha ao alterar Log de Acesso de Usuário " : erro);
                }
                throw new Exception(e.Message);
            }
        }

    }
}
