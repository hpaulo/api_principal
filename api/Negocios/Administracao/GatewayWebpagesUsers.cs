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
                    if (orderby == 0) entity = entity.OrderBy(e => e.ds_email).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.ds_email).AsQueryable<webpages_Users>();
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
                    if (orderby == 0) entity = entity.OrderBy(e => e.grupo_empresa.ds_nome).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.grupo_empresa.ds_nome).AsQueryable<webpages_Users>();
                    break;


                case CAMPOS.DS_FANTASIA:
                    if (orderby == 0) entity = entity.OrderBy(e => e.empresa.ds_fantasia).AsQueryable<webpages_Users>();
                    else entity = entity.OrderByDescending(e => e.empresa.ds_fantasia).AsQueryable<webpages_Users>();
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
            

            //DECLARAÇÕES
            List<dynamic> CollectionWebpages_Users = new List<dynamic>();
            Retorno retorno = new Retorno();

            // GET QUERY
            var query = getQuery(colecao, campo, orderBy, pageSize, pageNumber, queryString);

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
            else if (colecao == 2)
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
                    webpagesusersinroles = _db.webpages_UsersInRoles.Where(r => r.UserId == e.id_users).Select(r => new { RoleId = r.RoleId, RolePrincipal = r.RolePrincipal }).ToList(),
                    grupoempresa = e.grupo_empresa.ds_nome,
                    empresa = e.empresa.ds_fantasia,
                    //gruposempresasvendedor2 = _db.grupo_empresa.Where(g => g.id_vendedor == e.id_users).Select(g => new { g.id_grupo, g.ds_nome }).ToList(),
                    gruposvendedor = e.grupo_empresa_vendedor.Select( g => new { g.id_grupo, g.ds_nome }).ToList()

                }).ToList<dynamic>();
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

            _db.pessoas.Add(param.Pessoa);
            _db.SaveChanges();

            WebSecurity.CreateUserAndAccount(param.Webpagesusers.ds_login, "atos123", null, false);
            param.Webpagesusers.id_users = WebSecurity.GetUserId(param.Webpagesusers.ds_login);

            webpages_Users usr = _db.webpages_Users.Find(param.Webpagesusers.id_users);
            usr.ds_email = param.Webpagesusers.ds_email;
            usr.id_grupo = param.Webpagesusers.id_grupo;
            usr.nu_cnpjBaseEmpresa = param.Webpagesusers.nu_cnpjBaseEmpresa;
            usr.nu_cnpjEmpresa = param.Webpagesusers.nu_cnpjEmpresa;
            usr.id_pessoa = param.Pessoa.id_pesssoa;
            usr.fl_ativo = true;
            _db.SaveChanges();

            foreach (var item in param.Webpagesusersinroles)
            {
                if (item.UserId == 0)
                {
                    item.UserId = param.Webpagesusers.id_users;
                    _db.webpages_UsersInRoles.Add(item);
                    _db.SaveChanges();
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
                    {
                        value.id_grupo = null;
                        _db.SaveChanges();
                    }
                    else
                    {
                        value.id_grupo = param.Id_grupo;
                        _db.SaveChanges();
                    }
                }
                else
                    throw new Exception("Usuário inválido inválido!");
            }
            else
            {
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
                        value.ds_login = param.Webpagesusers.ds_login;
                    if (param.Webpagesusers.ds_email != null && param.Webpagesusers.ds_email != value.ds_email)
                        value.ds_email = param.Webpagesusers.ds_email;
                    if (param.Webpagesusers.fl_ativo != value.fl_ativo)
                    {
                        value.fl_ativo = param.Webpagesusers.fl_ativo;
                    }
                    if (param.Webpagesusers.id_grupo != null && param.Webpagesusers.id_grupo != 0 && param.Webpagesusers.id_grupo != value.id_grupo)
                    {
                        if (param.Webpagesusers.id_grupo == -1)
                            value.id_grupo = null;
                        else
                            value.id_grupo = param.Webpagesusers.id_grupo;
                    }
                    if (param.Webpagesusers.nu_cnpjEmpresa != null && param.Webpagesusers.nu_cnpjEmpresa != value.nu_cnpjEmpresa)
                    {
                        if (param.Webpagesusers.nu_cnpjEmpresa == "")
                            value.nu_cnpjEmpresa = null;
                        else
                            value.nu_cnpjEmpresa = param.Webpagesusers.nu_cnpjEmpresa;
                    }

                    _db.SaveChanges();
                }
            }

        }


    }
}
