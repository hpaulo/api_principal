using api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using api.Models.Object;
using api.Bibliotecas;
using System.Web.Helpers;
using System.Net.NetworkInformation;
using WebMatrix.WebData;
using System.Web;
using System.Collections;
using System.Web.Configuration;
using System.Collections.Specialized;

namespace api.Controllers.Login
{
    public class AutenticacaoController : ApiController
    {
        public Boolean Mobile = false;

        // GET api/autenticacao
        public IEnumerable<string> Get()
        {
            return new string[] { "value Aut - 1", "value Aut - 2" };
        }

        // GET api/autenticacao/5
        public Autenticado Get(string token)
        {
            try
            {
                Mobile = Bibliotecas.Device.IsMobile();

                if (Permissoes.Autenticado(token))
                {
                    using (var _db = new painel_taxservices_dbContext())
                    {
                        _db.Configuration.ProxyCreationEnabled = false;



                        var verify = (from v in _db.LoginAutenticacaos
                                      where v.token.Equals(token)
                                      select v
                                        ).Single();

                        if (verify != null)
                        {

                            #region Log de Acesso ao Sistema
                            api.Models.Object.Log log = new api.Models.Object.Log();
                            log.IdUser = verify.idUsers;
                            log.IdController = 45;
                            log.IdMethod = 51;
                            log.DtAcesso = DateTime.Now;

                            LogAcesso.New(log);
                            #endregion

                            var usuario = _db.webpages_Users.Where(u => u.id_users == verify.idUsers)
                                                            .Select(u => new
                                                            {
                                                                id_users = u.id_users,
                                                                nm_pessoa = u.pessoa.nm_pessoa,
                                                                ds_login = u.ds_login,
                                                                grupo_empresa = u.grupo_empresa,
                                                                empresa = u.empresa,
                                                                fl_ativo = u.fl_ativo,
                                                            }
                                                                    )
                                                            .FirstOrDefault();

                            if (!Permissoes.isAtosRole(token) && (usuario.grupo_empresa != null && !usuario.grupo_empresa.fl_ativo) ||
                               (usuario.empresa != null && usuario.empresa.fl_ativo == 0) || !usuario.fl_ativo)
                            {
                                // Usuário inativado
                                throw new Exception("401");
                            }


                            var rolesDoUsuario = _db.webpages_UsersInRoles
                                                                        .Where(r => r.UserId == usuario.id_users)
                                                                        .Where(r => r.RoleId > 50)
                                                                        .Where(r => (Mobile) ? r.webpages_Roles.RoleName.Contains("[Mobile]") : true)
                                                                        .AsQueryable();


                            List<dynamic> permissoes = rolesDoUsuario.Select(r => new
                            {
                                RoleId = r.RoleId,
                                RoleName = r.webpages_Roles.RoleName,
                                RoleLevel = r.webpages_Roles.RoleLevel,
                                RolePrincipal = r.RolePrincipal,
                                ControllerPrincipal = r.webpages_Roles.webpages_Permissions.Where(p => p.fl_principal == true).Where(p => p.webpages_Methods.ds_method.ToUpper().Equals("LEITURA")).Select(p => p.webpages_Methods.webpages_Controllers.id_controller).FirstOrDefault(), // .FirstOrDefault().webpages_Methods.webpages_Controllers.id_controller,
                                Controllers = _db.webpages_Permissions.Where(p => p.id_roles == r.RoleId).Where(p => p.webpages_Methods.ds_method.ToUpper().Equals("LEITURA")).Select(p => new { id_controller = p.webpages_Methods.id_controller, ds_controller = p.webpages_Methods.webpages_Controllers.ds_controller }).ToList<dynamic>(),
                                FiltroEmpresa = _db.webpages_Permissions.Where(p => p.id_roles == r.RoleId).Where(p => p.webpages_Methods.ds_method.ToUpper().Equals("FILTRO EMPRESA")).Select(p => new { id_controller = p.webpages_Methods.id_controller }).ToList<dynamic>().Count > 0,
                            }
                                                                            ).ToList<dynamic>();

                            // VER PERMISSÕES => OBTEM CONTROLLERS ASSOCIADOS AO CARD SERVICES, PROINFO, TAX SERVICES
                            // ESSAS PERMISSÕES SÓ VALERÃO SE A ROLE DO USUÁRIO NÃO FOR RELACIONADA AO PESSOAL DA ATOS ("ADMINISTRATIVO", "DESENVOLVEDOR", "COMERCIAL")
                            Boolean fl_cardservices = true;
                            Boolean fl_proinfo = true;
                            Boolean fl_taxservices = true;
                            List<webpages_Controllers> cardservices = new List<webpages_Controllers>();
                            List<webpages_Controllers> proinfo = new List<webpages_Controllers>();
                            List<webpages_Controllers> taxservices = new List<webpages_Controllers>();

                            if (!Permissoes.isAtosRole(token))
                            {
                                // Não é da ATOS!
                                if (usuario.grupo_empresa != null)
                                {
                                    fl_cardservices = usuario.grupo_empresa.fl_cardservices;
                                    fl_proinfo = usuario.grupo_empresa.fl_proinfo;
                                    fl_taxservices = usuario.grupo_empresa.fl_taxservices;
                                }
                                else
                                {
                                    // Não é usuário da ATOS e não está associado a um grupo empresa???!
                                    fl_cardservices = false;
                                    fl_proinfo = false;
                                    fl_taxservices = false;
                                }
                                cardservices = _db.Database.SqlQuery<webpages_Controllers>("WITH CTRL AS (SELECT ds_controller, id_controller, id_subController, nm_controller, fl_menu FROM dbo.webpages_Controllers WHERE ds_controller = 'Card Services' AND id_subController IS NULL AND id_controller > 50 UNION ALL SELECT c.ds_controller, c.id_controller, c.id_subController, c.nm_controller, c.fl_menu FROM dbo.webpages_Controllers c INNER JOIN CTRL s ON c.id_subController = s.id_controller) SELECT * FROM CTRL").ToList<webpages_Controllers>();
                                proinfo = _db.Database.SqlQuery<webpages_Controllers>("WITH CTRL AS (SELECT ds_controller, id_controller, id_subController, nm_controller, fl_menu FROM dbo.webpages_Controllers WHERE ds_controller = 'ProInfo' AND id_subController IS NULL AND id_controller > 50 UNION ALL SELECT c.ds_controller, c.id_controller, c.id_subController, c.nm_controller, c.fl_menu FROM dbo.webpages_Controllers c INNER JOIN CTRL s ON c.id_subController = s.id_controller) SELECT * FROM CTRL").ToList<webpages_Controllers>();
                                taxservices = _db.Database.SqlQuery<webpages_Controllers>("WITH CTRL AS (SELECT ds_controller, id_controller, id_subController, nm_controller, fl_menu FROM dbo.webpages_Controllers WHERE ds_controller = 'Tax Services' AND id_subController IS NULL AND id_controller > 50 UNION ALL SELECT c.ds_controller, c.id_controller, c.id_subController, c.nm_controller, c.fl_menu FROM dbo.webpages_Controllers c INNER JOIN CTRL s ON c.id_subController = s.id_controller) SELECT * FROM CTRL").ToList<webpages_Controllers>();
                            }

                            // Adiciona os controllers
                            List<Int32> list = new List<Int32>();
                            int id_ControllerPrincipal = 0;
                            Boolean filtro_empresa = false;
                            foreach (var item in permissoes)
                            {
                                if (item.RolePrincipal == true)
                                    id_ControllerPrincipal = item.ControllerPrincipal;

                                // Avalia controllers
                                foreach (var subItem in item.Controllers)
                                {
                                    // É de Card Service ?
                                    if (cardservices.Any(c => c.id_controller == subItem.id_controller))
                                    {
                                        if (fl_cardservices) list.Add(subItem.id_controller); // só add se tiver acesso ao card services
                                    }
                                    // É de Pro info ?
                                    else if (proinfo.Any(c => c.id_controller == subItem.id_controller))
                                    {
                                        if (fl_proinfo) list.Add(subItem.id_controller); // só add se tiver acesso ao pro info
                                    }
                                    // É de Tax Service ?
                                    else if (taxservices.Any(c => c.id_controller == subItem.id_controller))
                                    {
                                        if (fl_taxservices) list.Add(subItem.id_controller); // só add se tiver acesso ao tax services
                                    }
                                    else list.Add(subItem.id_controller);
                                }

                                if (item.FiltroEmpresa)
                                    filtro_empresa = true;
                            }

                            list = list.Distinct().ToList<Int32>();

                            // Retorna os controllers na estrutura de árvore
                            // Em cada controller é listado os métodos permitidos para o usuário logado
                            List<dynamic> controllers = _db.webpages_Controllers
                            .Where(e => e.id_subController == null)
                            .Where(e => list.Contains(e.id_controller))
                            .OrderBy(e => e.ds_controller)
                            .Select(e => new
                            {

                                id_controller = e.id_controller,
                                ds_controller = e.ds_controller,
                                home = e.id_controller == id_ControllerPrincipal,
                                methods = rolesDoUsuario.Select(r => new
                                {
                                    metodos = _db.webpages_Permissions
                                                  .Where(p => p.id_roles == r.RoleId)
                                                  .Where(p => p.webpages_Methods.id_controller == e.id_controller)
                                                  .Select(p => new
                                                  {
                                                      id_method = p.webpages_Methods.id_method,
                                                      ds_method = p.webpages_Methods.ds_method,
                                                      nm_method = p.webpages_Methods.nm_method,
                                                      id_controller = p.webpages_Methods.id_controller
                                                  }
                                                  ).ToList<dynamic>(),
                                }
                                                                    ).ToList<dynamic>(),
                                subControllers = _db.webpages_Controllers
                                                                            .Where(sub => sub.id_subController == e.id_controller)
                                                                            .Where(sub => list.Contains(sub.id_controller))
                                                                            .OrderBy(sub => sub.ds_controller)
                                                                            .Select(sub => new
                                                                            {
                                                                                id_controller = sub.id_controller,
                                                                                ds_controller = sub.ds_controller,
                                                                                home = sub.id_controller == id_ControllerPrincipal,
                                                                                methods = rolesDoUsuario.Select(r => new
                                                                                {
                                                                                    metodos = _db.webpages_Permissions
                                                                                                  .Where(p => p.id_roles == r.RoleId)
                                                                                                  .Where(p => p.webpages_Methods.id_controller == sub.id_controller)
                                                                                                  .Select(p => new
                                                                                                  {
                                                                                                      id_method = p.webpages_Methods.id_method,
                                                                                                      ds_method = p.webpages_Methods.ds_method,
                                                                                                      nm_method = p.webpages_Methods.nm_method,
                                                                                                      id_controller = p.webpages_Methods.id_controller
                                                                                                  }
                                                                                                  ).ToList<dynamic>(),
                                                                                }
                                                                                                                    ).ToList<dynamic>(),
                                                                                subControllers = _db.webpages_Controllers
                                                                                                    .OrderBy(sub2 => sub2.ds_controller)
                                                                                                    .Where(sub2 => sub2.id_subController == sub.id_controller)
                                                                                                    .Where(sub2 => list.Contains(sub2.id_controller))
                                                                                                    .Select(sub2 => new
                                                                                                    {
                                                                                                        id_controller = sub2.id_controller,
                                                                                                        ds_controller = sub2.ds_controller,
                                                                                                        home = sub2.id_controller == id_ControllerPrincipal,
                                                                                                        methods = rolesDoUsuario.Select(r => new
                                                                                                        {
                                                                                                            metodos = _db.webpages_Permissions
                                                                                                                          .Where(p => p.id_roles == r.RoleId)
                                                                                                                          .Where(p => p.webpages_Methods.id_controller == sub2.id_controller)
                                                                                                                          .Select(p => new
                                                                                                                          {
                                                                                                                              id_method = p.webpages_Methods.id_method,
                                                                                                                              ds_method = p.webpages_Methods.ds_method,
                                                                                                                              nm_method = p.webpages_Methods.nm_method,
                                                                                                                              id_controller = p.webpages_Methods.id_controller
                                                                                                                          }
                                                                                                                          ).ToList<dynamic>(),
                                                                                                        }
                                                                                                                    ).ToList<dynamic>(),
                                                                                                    }).ToList<dynamic>()


                                                                            }).ToList<dynamic>()
                            }).ToList<dynamic>();

                            // Obtém o array de métodos permitidos no mesmo nível dos outros campos
                            foreach (var controller in controllers)
                            {
                                if (controller.methods.Count > 0)
                                {
                                    var m = controller.methods[0];
                                    controller.methods.Clear();
                                    foreach (var method in m.metodos)
                                        controller.methods.Add(method);
                                }
                                foreach (var subController in controller.subControllers)
                                {
                                    if (subController.methods.Count > 0)
                                    {
                                        var m = subController.methods[0];
                                        subController.methods.Clear();
                                        foreach (var method in m.metodos)
                                            subController.methods.Add(method);
                                    }
                                    foreach (var subController2 in subController.subControllers)
                                    {
                                        if (subController2.methods.Count > 0)
                                        {
                                            var m = subController2.methods[0];
                                            subController2.methods.Clear();
                                            foreach (var method in m.metodos)
                                                subController2.methods.Add(method);
                                        }
                                    }
                                }
                            }

                            return new Models.Object.Autenticado
                            {
                                nome = usuario.nm_pessoa,
                                usuario = usuario.ds_login,
                                id_grupo = (usuario.grupo_empresa == null ? -1 : (Int32)usuario.grupo_empresa.id_grupo),
                                nu_cnpj =  (usuario.empresa == null ? null : usuario.empresa.nu_cnpj),
                                filtro_empresa = filtro_empresa,
                                token = token,
                                controllers = controllers,
                            };
                        }
                        else
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);


            }
            catch (Exception e)
            {
                if (e.Message.Equals("401")) throw new HttpResponseException(HttpStatusCode.Unauthorized);
                else throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }



        // POST api/autenticacao
        public Autenticado Post(Models.Object.Login data)
        {
            try
            {

                if (ModelState.IsValid && WebSecurity.Login(data.usuario, data.senha, persistCookie: false))
                {

                    int userId = WebSecurity.GetUserId(data.usuario);
                    using (var _db = new painel_taxservices_dbContext())
                    {
                        _db.Configuration.ProxyCreationEnabled = false;


                        #region Log de Acesso ao Sistema
                        api.Models.Object.Log log = new api.Models.Object.Log();
                        log.IdUser = userId;
                        log.IdController = 45;
                        log.IdMethod = 50;
                        log.DtAcesso = DateTime.Now;

                        LogAcesso.New(log);
                        #endregion

                        string token = "";



                        var verify = (from v in _db.LoginAutenticacaos
                                      where v.idUsers.Equals(userId)
                                      orderby v.idUsers
                                      select v
                                     ).FirstOrDefault();

                        if (verify == null)
                        {
                            token = Token.GetUniqueKey(data.usuario);
                            LoginAutenticacao la = new LoginAutenticacao();
                            la.idUsers = userId;
                            la.token = token;
                            la.dtValidade = DateTime.Now;
                            _db.LoginAutenticacaos.Add(la);
                            _db.SaveChanges();
                        }
                        else
                            token = verify.token;





                        var usuario = _db.webpages_Users.Where(u => u.id_users == verify.idUsers)
                                                            .Select(u => new
                                                            {
                                                                id_users = u.id_users,
                                                                nm_pessoa = u.pessoa.nm_pessoa,
                                                                ds_login = u.ds_login,
                                                                fl_ativo = u.fl_ativo,
                                                                grupo_empresa = u.grupo_empresa,
                                                                empresa = u.empresa,
                                                            }
                                                                    )
                                                            .FirstOrDefault();

                        if (!Permissoes.isAtosRole(token) && (usuario.grupo_empresa != null && !usuario.grupo_empresa.fl_ativo) ||
                               (usuario.empresa != null && usuario.empresa.fl_ativo == 0) || !usuario.fl_ativo)
                        {
                            throw new Exception("401");
                        }

                        List<dynamic> permissoes = _db.webpages_UsersInRoles.Where(r => r.UserId == usuario.id_users)
                                                                            .Where(r => r.RoleId > 50)
                                                                            .Select(
                                                                                        r => new
                                                                                        {
                                                                                            RoleId = r.RoleId,
                                                                                            RolePrincipal = r.RolePrincipal,
                                                                                            ControllerPrincipal = r.webpages_Roles.webpages_Permissions.Where(p => p.fl_principal == true).Select(p => p.webpages_Methods.webpages_Controllers.id_controller).FirstOrDefault(), // .FirstOrDefault().webpages_Methods.webpages_Controllers.id_controller,
                                                                                            Controllers = _db.webpages_Permissions.Where(p => p.id_roles == r.RoleId).Select(p => new { id_controller = p.webpages_Methods.id_controller }).ToList<dynamic>()
                                                                                        }
                                                                                    ).ToList<dynamic>();

                        List<Int32> list = new List<Int32>();
                        int id_ControllerPrincipal = 0;
                        foreach (var item in permissoes)
                        {
                            if (item.RolePrincipal == true)
                                id_ControllerPrincipal = item.ControllerPrincipal;

                            foreach (var subItem in item.Controllers)
                            {
                                list.Add(subItem.id_controller);
                            }
                        }

                        list = list.Distinct().ToList<Int32>();



                        List<dynamic> controllers = _db.webpages_Controllers
                        .Where(e => e.id_subController == null)
                        .Where(e => list.Contains(e.id_controller))
                        .OrderBy(e => e.ds_controller)
                        .Select(e => new
                        {

                            id_controller = e.id_controller,
                            ds_controller = e.ds_controller,
                            home = e.id_controller == id_ControllerPrincipal,
                            subControllers = _db.webpages_Controllers
                                                                        .Where(sub => sub.id_subController == e.id_controller)
                                                                        .Where(sub => list.Contains(sub.id_controller))
                                                                        .OrderBy(sub => sub.ds_controller)
                                                                        .Select(sub => new
                                                                        {
                                                                            id_controller = sub.id_controller,
                                                                            ds_controller = sub.ds_controller,
                                                                            home = sub.id_controller == id_ControllerPrincipal,
                                                                            subControllers = _db.webpages_Controllers
                                                                                                .OrderBy(sub2 => sub2.ds_controller)
                                                                                                .Where(sub2 => sub2.id_subController == sub.id_controller)
                                                                                                .Where(sub2 => list.Contains(sub2.id_controller))
                                                                                                .Select(sub2 => new
                                                                                                {
                                                                                                    id_controller = sub2.id_controller,
                                                                                                    ds_controller = sub2.ds_controller,
                                                                                                    home = sub2.id_controller == id_ControllerPrincipal,
                                                                                                }).ToList<dynamic>()


                                                                        }).ToList<dynamic>()
                        }).ToList<dynamic>();

                        return new Models.Object.Autenticado { nome = usuario.nm_pessoa, usuario = usuario.ds_login, token = token, controllers = controllers };
                    }

                }
                else
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);

            }
            catch (Exception e)
            {
                if (e.Message.Equals("401")) throw new HttpResponseException(HttpStatusCode.Unauthorized);
                else throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }



        // PUT api/autenticacao/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE api/autenticacao/5
        public void Delete(int id)
        {
        }
    }

}
