using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;
using System.Linq.Expressions;
using api.Bibliotecas;
using api.Models.Object;
using WebMatrix.WebData;

namespace api.Negocios.Administracao
{
    public class GatewayWebpagesUsers
    {
        static painel_taxservices_dbContext _db = new painel_taxservices_dbContext();

        /// <summary>
        /// Auto Loader
        /// </summary>
        public GatewayWebpagesUsers()
        {
            _db.Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// Enum CAMPOS
        /// </summary>
        public enum CAMPOS
        {
            ID_USERS = 100,
            DS_LOGIN = 101,
            DS_EMAIL = 102,
            ID_GRUPO = 103,
            NU_CNPJEMPRESA = 104,
            NU_CNPJBASEEMPRESA = 105,
            ID_PESSOA = 106,


            // PERSONALIZADO
            PESSOA = 200,
            DS_NOME = 301,
            DS_FANTASIA = 404,
            USERSINROLES = 500,


        };

        /// <summary>
        /// Get Webpages_Users/Webpages_Users
        /// </summary>
        /// <param name="colecao"></param>
        /// <param name="campo"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private static IQueryable<webpages_Users> getQuery(int colecao, int campo, int orderby, int pageSize, int pageNumber, Dictionary<string, string> queryString)
        {
            // DEFINE A QUERY PRINCIPAL 
            var entity = _db.webpages_Users.AsQueryable<webpages_Users>();

            #region WHERE - ADICIONA OS FILTROS A QUERY

            // ADICIONA OS FILTROS A QUERY
            foreach (var item in queryString)
            {
                int key = Convert.ToInt16(item.Key);
                CAMPOS filtroEnum = (CAMPOS)key;
                switch (filtroEnum)
                {


                    case CAMPOS.ID_USERS:
                        Int32 id_users = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_users.Equals(id_users)).AsQueryable<webpages_Users>();
                        break;
                    case CAMPOS.DS_LOGIN:
                        string ds_login = Convert.ToString(item.Value);
                        if (ds_login.Contains("%")) // usa LIKE
                        {
                            string busca = ds_login.Replace("%", "").ToString();
                            entity = _db.webpages_Users.Where(e => e.ds_login.Contains(busca));
                        }
                        else
                        entity = entity.Where(e => e.ds_login.Equals(ds_login)).AsQueryable<webpages_Users>();
                        break;
                    case CAMPOS.DS_EMAIL:
                        string ds_email = Convert.ToString(item.Value);
                        if (ds_email.Contains("%")) // usa LIKE
                        {
                            string busca = ds_email.Replace("%", "").ToString();
                            entity = _db.webpages_Users.Where(e => e.ds_email.Contains(busca));
                        }
                        else
                            entity = entity.Where(e => e.ds_email.Equals(ds_email)).AsQueryable<webpages_Users>();
                        break;
                    case CAMPOS.ID_GRUPO:
                        Int32 id_grupo = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_grupo == id_grupo).AsQueryable<webpages_Users>();
                        //var ent = entity.Where(e => e.id_grupo == id_grupo);
                        break;
                    case CAMPOS.NU_CNPJEMPRESA:
                        string nu_cnpjEmpresa = Convert.ToString(item.Value);
                        
                        entity = entity.Where(e => e.nu_cnpjEmpresa.Equals(nu_cnpjEmpresa)).AsQueryable<webpages_Users>();
                        break;
                    case CAMPOS.NU_CNPJBASEEMPRESA:
                        string nu_cnpjBaseEmpresa = Convert.ToString(item.Value);
                        entity = entity.Where(e => e.nu_cnpjBaseEmpresa.Equals(nu_cnpjBaseEmpresa)).AsQueryable<webpages_Users>();
                        break;
                    case CAMPOS.ID_PESSOA:
                        Int32 id_pessoa = Convert.ToInt32(item.Value);
                        entity = entity.Where(e => e.id_pessoa.Equals(id_pessoa)).AsQueryable<webpages_Users>();
                        break;


                    // PERSONALIZADO


                    case CAMPOS.DS_NOME:
                        string ds_nome = Convert.ToString(item.Value);
                        if (ds_nome.Contains("%")) // usa LIKE
                        {
                            string busca = ds_nome.Replace("%", "").ToString();
                            entity = entity.Where(e => e.grupo_empresa.ds_nome.Contains(busca)).AsQueryable<webpages_Users>();
                        }
                        else
                            entity = entity.Where(e => e.grupo_empresa.ds_nome.Equals(ds_nome)).AsQueryable<webpages_Users>();
                        break;


                    case CAMPOS.DS_FANTASIA:
                        string ds_fantasia = Convert.ToString(item.Value);
                        if (ds_fantasia.Contains("%")) // usa LIKE
                        {
                            string busca = ds_fantasia.Replace("%", "").ToString();
                            entity = entity.Where(e => e.empresa.ds_fantasia.Contains(busca)).AsQueryable<webpages_Users>();
                        }
                        else
                            entity = entity.Where(e => e.empresa.ds_fantasia.Equals(ds_fantasia)).AsQueryable<webpages_Users>();
                        break;


                }
            }
            #endregion

            #region ORDER BY - ADICIONA A ORDENAÇÃO A QUERY
            // ADICIONA A ORDENAÇÃO A QUERY
            CAMPOS filtro = (CAMPOS)campo;
            switch (filtro)
            {

                case CAMPOS.ID_USERS:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_users).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.id_users).AsQueryable<webpages_Users>();
                    break;
                case CAMPOS.DS_LOGIN:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_login).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.ds_login).AsQueryable<webpages_Users>();
                    break;
                case CAMPOS.DS_EMAIL:
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_email).ThenBy(e => e.ds_login).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.ds_email).ThenByDescending(e => e.ds_login).AsQueryable<webpages_Users>();
                    break;
                case CAMPOS.ID_GRUPO:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_grupo).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.id_grupo).AsQueryable<webpages_Users>();
                    break;
                case CAMPOS.NU_CNPJEMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_cnpjEmpresa).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.nu_cnpjEmpresa).AsQueryable<webpages_Users>();
                    break;
                case CAMPOS.NU_CNPJBASEEMPRESA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.nu_cnpjBaseEmpresa).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.nu_cnpjBaseEmpresa).AsQueryable<webpages_Users>();
                    break;
                case CAMPOS.ID_PESSOA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.id_pessoa).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.id_pessoa).AsQueryable<webpages_Users>();
                    break;




                // PERSONALIZADO


                case CAMPOS.DS_NOME:
                    if (orderby == 0) entity = entity.OrderBy(e => e.grupo_empresa.ds_nome).ThenBy(e => e.empresa.ds_fantasia).ThenBy(e => e.ds_login).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.grupo_empresa.ds_nome).ThenByDescending(e => e.empresa.ds_fantasia).ThenByDescending(e => e.ds_login).AsQueryable<webpages_Users>();
                    break;


                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.empresa.ds_fantasia).ThenBy(e => e.ds_login).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.empresa.ds_fantasia).ThenByDescending(e => e.ds_login).AsQueryable<webpages_Users>();
                    break;

            }
            #endregion

            return entity;


        }


        /// <summary>
        /// Retorna Webpages_Users/Webpages_Users
        /// </summary>
        /// <returns></returns>
        public static Retorno Get(string token, int colecao = 0, int campo = 0, int orderBy = 0, int pageSize = 0, int pageNumber = 0, Dictionary<string, string> queryString = null)
        {
            //DECLARAÇÕES
            List<dynamic> CollectionWebpages_Users = new List<dynamic>();
            Retorno retorno = new Retorno();

            // Se for uma consulta por um login ou e-mail específico na coleção 0, não força filtro por empresa, filial e rolelevel
            string outValue = null;
            Boolean FiltroForcado = true;

            if (colecao == 0)
            {
                Boolean filtroLogin = queryString.TryGetValue("" + (int)CAMPOS.DS_LOGIN, out outValue);
                Boolean filtroEmail = queryString.TryGetValue("" + (int)CAMPOS.DS_EMAIL, out outValue);

                if (filtroLogin && filtroEmail)
                    FiltroForcado = queryString["" + (int)CAMPOS.DS_LOGIN].Contains("%") || queryString["" + (int)CAMPOS.DS_EMAIL].Contains("%");
                else if (filtroLogin)
                    FiltroForcado = queryString["" + (int)CAMPOS.DS_LOGIN].Contains("%");
                else if (filtroEmail)
                    FiltroForcado = queryString["" + (int)CAMPOS.DS_EMAIL].Contains("%");
            }

            // Implementar o filtro por Grupo apartir do TOKEN do Usuário
            Int32 IdGrupo = 0;
            if (FiltroForcado)
            {
                IdGrupo = Permissoes.GetIdGrupo(token);
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
            }

            if (colecao == 3)
            {
                int IdUsers = Permissoes.GetIdUser(token);
                if (IdUsers != 0)
                {
                    if (queryString.TryGetValue("" + (int)CAMPOS.ID_USERS, out outValue))
                        queryString["" + (int)CAMPOS.ID_USERS] = IdUsers.ToString();
                    else
                        queryString.Add("" + (int)CAMPOS.ID_USERS, IdUsers.ToString());
                }
            }

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);


            if (colecao != 3) // [WEB] A coleção 3 permite que o usuário de qualquer perfil obtenha os seus próprios dados
            {

                if (FiltroForcado)
                {
                    // Restringe consulta pelo perfil do usuário logado
                    Int32 RoleLevelMin = Permissoes.GetRoleLevelMin(token);
                    String RoleName = Permissoes.GetRoleName(token).ToUpper();
                    if (IdGrupo == 0 && RoleName.Equals("COMERCIAL"))
                    {
                        // Perfil Comercial tem uma carteira de clientes específica
                        List<Int32> listaIdsGruposEmpresas = Permissoes.GetIdsGruposEmpresasVendedor(token);
                        query = query.Where(e => e.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin
                                                 && e.id_grupo != null && listaIdsGruposEmpresas.Contains(e.id_grupo ?? -1)).AsQueryable<webpages_Users>();
                    }
                    else if (Permissoes.isAtosRole(token) && !RoleName.Equals("COMERCIAL"))
                        // ATOS de nível mais alto: Lista os usuários que não tem role associada ou aqueles de RoleLevel permitido para o usuário logado consultar
                        query = query.Where(e => e.webpages_Membership.webpages_UsersInRoles.ToList<dynamic>().Count == 0 || e.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin).AsQueryable<webpages_Users>();
                    else
                        // Só exibe os usuários de RoleLevelMin
                        query = query.Where(e => e.webpages_Membership.webpages_UsersInRoles.FirstOrDefault().webpages_Roles.RoleLevel >= RoleLevelMin).AsQueryable<webpages_Users>();
                }
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
                CollectionWebpages_Users = query.Select(e => new
                {

                    id_users = e.id_users,
                    ds_login = e.ds_login,
                    ds_email = e.ds_email,
                    id_grupo = e.id_grupo,
                    fl_ativo = e.fl_ativo,
                    nu_cnpjEmpresa = e.nu_cnpjEmpresa,
                    nu_cnpjBaseEmpresa = e.nu_cnpjBaseEmpresa,
                    id_pessoa = e.id_pessoa,
                }).ToList<dynamic>();
            }
            else if (colecao == 0)
            {
                CollectionWebpages_Users = query.Select(e => new
                {

                    id_users = e.id_users,
                    ds_login = e.ds_login,
                    ds_email = e.ds_email,
                    id_grupo = e.id_grupo,
                    fl_ativo = e.fl_ativo,
                    nu_cnpjEmpresa = e.nu_cnpjEmpresa,
                    nu_cnpjBaseEmpresa = e.nu_cnpjBaseEmpresa,
                    id_pessoa = e.id_pessoa,
                }).ToList<dynamic>();
            }
            else if (colecao == 2 || colecao == 3) // [WEB] Dados do Usuário Logado COLEÇÃO 3
            {
                CollectionWebpages_Users = query.Select(
                e => new
                {
                    webpagesusers = new
                    {
                        id_users = e.id_users,
                        ds_login = e.ds_login,
                        ds_email = e.ds_email,
                        id_grupo = e.id_grupo,
                        fl_ativo = e.fl_ativo,
                        nu_cnpjEmpresa = e.nu_cnpjEmpresa,
                        nu_cnpjBaseEmpresa = e.nu_cnpjBaseEmpresa,
                        id_pessoa = e.id_pessoa
                    },
                    pessoa = new
                    {
                        nm_pessoa = e.pessoa.nm_pessoa,
                        dt_nascimento = e.pessoa.dt_nascimento,
                        nu_telefone = e.pessoa.nu_telefone,
                        nu_ramal = e.pessoa.nu_ramal
                    },
                    webpagesusersinroles = _db.webpages_UsersInRoles.Where(r => r.UserId == e.id_users).Select(r => new { RoleId = r.RoleId, RoleName = r.webpages_Roles.RoleName, RolePrincipal = r.RolePrincipal }).ToList(),
                    grupoempresa = e.grupo_empresa.ds_nome,
                    empresa = e.empresa.ds_fantasia + (e.empresa.filial != null ? " " + e.empresa.filial : ""),
                    //gruposempresasvendedor2 = _db.grupo_empresa.Where(g => g.id_vendedor == e.id_users).Select(g => new { g.id_grupo, g.ds_nome }).ToList(),
                    gruposvendedor = e.grupo_empresa_vendedor.Select(g => new { g.id_grupo, g.ds_nome }).ToList()

                }).ToList<dynamic>();
            }
            else if (colecao == 3) // [WEB] Dados do Usuário Logado
            {
                // OBS: UTILIZADO EM CONJUNTO COM A COLEÇÃO 2
            }

            else if (colecao == 4)
	        {
                string ds_login = queryString[((int)CAMPOS.DS_LOGIN).ToString()];
                string ds_email = queryString[((int)CAMPOS.DS_EMAIL).ToString()];
                var o = new { 
                            login = _db.webpages_Users.Where(e => e.ds_login.Equals(ds_login)).Count() > 0,
                            Email = _db.webpages_Users.Where(e => e.ds_email.Equals(ds_email)).Count() > 0, 
                };
                CollectionWebpages_Users.Add(o);
	        }

            retorno.Registros = CollectionWebpages_Users;

            return retorno;
        }



        /// <summary>
        /// Adiciona nova Webpages_Users
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Int32 Add(string token, Models.Object.Usuario param)
        {
            // Adiciona os dados da pessoa
            param.Pessoa.id_pesssoa = GatewayPessoa.Add(token, param.Pessoa);
            //_db.pessoas.Add(param.Pessoa);
            //_db.SaveChanges();

            // Cria a conta com o login informado e a senha padrão "atos123"
            try {
                WebSecurity.CreateUserAndAccount(param.Webpagesusers.ds_login, "atos123", null, false);
            }catch
            {
                // Remove a pessoa criada
                GatewayPessoa.Delete(token, param.Pessoa.id_pesssoa);
                // Reporta a falha
                throw new Exception("500");
            }
            param.Webpagesusers.id_users = WebSecurity.GetUserId(param.Webpagesusers.ds_login);

            // Cria o usuário
            webpages_Users usr = _db.webpages_Users.Find(param.Webpagesusers.id_users);
            usr.ds_email = param.Webpagesusers.ds_email;
            usr.id_grupo = param.Webpagesusers.id_grupo;
            usr.nu_cnpjBaseEmpresa = param.Webpagesusers.nu_cnpjBaseEmpresa;
            usr.nu_cnpjEmpresa = param.Webpagesusers.nu_cnpjEmpresa;
            usr.id_pessoa = param.Pessoa.id_pesssoa;
            usr.fl_ativo = true;
            try {
                _db.SaveChanges();
            }
            catch
            {
                // Remova a pessoa e a conta criada
                GatewayPessoa.Delete(token, param.Pessoa.id_pesssoa);
                GatewayWebpagesMembership.Delete(token, param.Webpagesusers.id_users);
                // Reporta a falha
                throw new Exception("500");
            }

            foreach (var item in param.Webpagesusersinroles)
            {
                if (item.UserId == 0)
                {
                    item.UserId = param.Webpagesusers.id_users;
                    _db.webpages_UsersInRoles.Add(item);
                    try {
                        _db.SaveChanges();
                    }catch
                    {
                        // não é porque não associou alguma role que deve retornar erro por completo
                    }
                }
            }

            // Associa grupos empresas ao vendedor
            if (param.Addidsgrupoempresavendedor != null)
            {
                foreach (var idGrupo in param.Addidsgrupoempresavendedor)
                {

                    grupo_empresa grupo = _db.grupo_empresa.Where(g => g.id_grupo == idGrupo).FirstOrDefault();

                    if (grupo != null)
                    {
                        grupo.id_vendedor = param.Webpagesusers.id_users;
                        try {
                            _db.SaveChanges();
                        }catch
                        {
                            // não é porque não associou algum grupo ao vendedor que deve retornar erro por completo
                        }
                    }
                }
            }

            return param.Webpagesusers.id_users;
        }


        /// <summary>
        /// Apaga uma Webpages_Users
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        //public static void Delete(string token, Int32 id_users)
        //{
        //    _db.webpages_Users.Remove(_db.webpages_Users.Where(e => e.id_users.Equals(id_users)).First());
        //    _db.SaveChanges();
        //}

        public static void Delete(string token, Int32 id_users)
        {
            if (_db.LogAcesso1.Where(e => e.idUsers == id_users).ToList().Count == 0)
            {
                GatewayWebpagesUsersInRoles.Delete(token, id_users, false);
                GatewayWebpagesMembership.Delete(token, id_users);
                // Obtem o usuário com o id_users
                webpages_Users value = _db.webpages_Users
                                          .Where(e => e.id_users.Equals(id_users))
                                          .First<webpages_Users>();

                int id_pessoa = (value.id_pessoa != null) ? Convert.ToInt32(value.id_pessoa) : 0;

                _db.webpages_Users.RemoveRange(_db.webpages_Users.Where(e => e.id_users == id_users));
                _db.SaveChanges();
                if (id_pessoa > 0)
                {
                    GatewayPessoa.Delete(token, id_pessoa);
                }
            }
            else
                throw new Exception("Usuário não pode ser deletado!");

        }



        /// <summary>
        /// Altera webpages_Users
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Update(string token, Models.Object.Usuario param)
        {
            if (param.Id_grupo != 0)
            {
                // Altera grupo empresa do usuário logado
                Int32 IdUser = Permissoes.GetIdUser(token);
                webpages_Users value = _db.webpages_Users
                        .Where(e => e.id_users == IdUser)
                        .FirstOrDefault<webpages_Users>();

                if (value != null)
                {
                    // VALIDAR PERMISSÂO PARA FUNCIONALIDADE

                    if (param.Id_grupo == -1)
                        value.id_grupo = null;
                    else
                        value.id_grupo = param.Id_grupo;

                    value.nu_cnpjEmpresa = null;
                    _db.SaveChanges();
                }
                else
                    throw new Exception("Usuário inválido!");
            }
            else
            {
                if (param.Webpagesusers.id_users == 0) throw new Exception("Falha ao parâmetro");

                // Altera um usuário que não necessiariamente é o logado
                webpages_Users value = _db.webpages_Users
                        .Where(e => e.id_users == param.Webpagesusers.id_users)
                        .First<webpages_Users>();

                if (value != null)
                {


                    if (param.Pessoa != null)
                    {
                        param.Pessoa.id_pesssoa = (int)value.id_pessoa;
                        GatewayPessoa.Update(token, param.Pessoa);
                    }

                    if (param.Webpagesusersinroles != null)
                    {
                        foreach (var item in param.Webpagesusersinroles)
                        {
                            if (item.UserId == -1)
                            {
                                item.UserId = param.Webpagesusers.id_users;
                                GatewayWebpagesUsersInRoles.Delete(token, item);
                            }
                            else
                            {
                                item.UserId = param.Webpagesusers.id_users;
                                webpages_UsersInRoles verificacao = _db.webpages_UsersInRoles.Where(p => p.UserId == item.UserId).Where(p => p.RoleId == item.RoleId).FirstOrDefault();
                                if (verificacao != null)
                                {
                                    webpages_UsersInRoles principal = _db.webpages_UsersInRoles
                                                                        .Where(p => p.UserId == item.UserId)
                                                                        .Where(p => p.RolePrincipal == true).FirstOrDefault();
                                    if (principal != null)
                                        principal.RolePrincipal = false;

                                    verificacao.RolePrincipal = item.RolePrincipal;
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    GatewayWebpagesUsersInRoles.Add(token, item);
                                }
                            }
                        }
                    }
                    // Associa grupos empresas ao vendedor
                    if (param.Addidsgrupoempresavendedor != null)
                    {
                        foreach (var idGrupo in param.Addidsgrupoempresavendedor)
                        {

                            grupo_empresa grupo = _db.grupo_empresa.Where(g => g.id_grupo == idGrupo).FirstOrDefault();

                            if (grupo != null)
                            {
                                grupo.id_vendedor = param.Webpagesusers.id_users;
                                _db.SaveChanges();
                            }
                        }
                    }
                    // Desassocia grupos empresas
                    if (param.Removeidsgrupoempresavendedor != null)
                    {
                        foreach (var idGrupo in param.Removeidsgrupoempresavendedor)
                        {

                            grupo_empresa grupo = _db.grupo_empresa.Where(g => g.id_grupo == idGrupo).FirstOrDefault();

                            if (grupo != null)
                            {
                                grupo.id_vendedor = null;
                                _db.SaveChanges();
                            }
                        }
                    }


                    if (param.Webpagesusers.ds_login != null && param.Webpagesusers.ds_login != value.ds_login)
                    {
                        webpages_Users old = _db.webpages_Users.Where(e => e.ds_login.ToLower().Equals(param.Webpagesusers.ds_login.ToLower()))
                                                               .FirstOrDefault();
                        if (old == null || old.id_users == value.id_users) value.ds_login = param.Webpagesusers.ds_login;
                    }
                    if (param.Webpagesusers.ds_email != null && param.Webpagesusers.ds_email != value.ds_email)
                    {
                        webpages_Users old = _db.webpages_Users.Where(e => e.ds_email.ToLower().Equals(param.Webpagesusers.ds_email.ToLower()))
                                                               .FirstOrDefault();
                        if (old == null || old.id_users == value.id_users)
                            value.ds_email = param.Webpagesusers.ds_email;
                    }
                    if (param.Webpagesusers.fl_ativo != value.fl_ativo)
                    {
                        value.fl_ativo = param.Webpagesusers.fl_ativo;
                    }

                    Boolean grupoEmpresaAlterado = false;
                    if (param.Webpagesusers.nu_cnpjEmpresa != null && param.Webpagesusers.nu_cnpjEmpresa != value.nu_cnpjEmpresa)
                    {
                        if (param.Webpagesusers.nu_cnpjEmpresa == "")
                            value.nu_cnpjEmpresa = null;
                        else
                        {
                            value.nu_cnpjEmpresa = param.Webpagesusers.nu_cnpjEmpresa;
                            value.id_grupo = _db.empresas.Where(f => f.nu_cnpj.Equals(param.Webpagesusers.nu_cnpjEmpresa)).Select(f => f.id_grupo).FirstOrDefault();
                            grupoEmpresaAlterado = true; // já forçou o grupo pela filial
                        }
                    }// só pode colocar grupo empresa ao qual a filial está ou sem nenhuma filial

                    if (!grupoEmpresaAlterado && param.Webpagesusers.id_grupo != null && param.Webpagesusers.id_grupo != 0 && param.Webpagesusers.id_grupo != value.id_grupo)
                    {
                        if (param.Webpagesusers.id_grupo == -1)
                        {
                            value.id_grupo = null;
                            value.nu_cnpjEmpresa = null; // Não pode estar associado a uma filial sem estar associado a um grupo
                        }
                        else
                        {
                            value.id_grupo = param.Webpagesusers.id_grupo;
                            // Avalia se tem empresa associado => A filial TEM QUE SER associada ao grupo
                            if (value.nu_cnpjEmpresa != null)
                            {
                                Int32 id_grupo = _db.empresas.Where(f => f.nu_cnpj.Equals(value.nu_cnpjEmpresa)).Select(f => f.id_grupo).FirstOrDefault();
                                if (id_grupo != value.id_grupo)
                                    value.nu_cnpjEmpresa = null; // filial que estava associado é de um grupo diferente do grupo recém associado
                            }
                        }
                    }

                    _db.SaveChanges();
                }
                else
                    throw new Exception("Usuário não cadastrado");
            }

        }


    }
}
