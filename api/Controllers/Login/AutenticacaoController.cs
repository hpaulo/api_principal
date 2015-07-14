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

namespace api.Controllers.Login
{
    public class AutenticacaoController : ApiController
    {
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
               //var ip = System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Request.UserHostAddress : "";

               //NetworkInterface[] nif = NetworkInterface.GetAllNetworkInterfaces();
               //String MACAddress = string.Empty;
               //foreach (NetworkInterface adapter in nif)
               //{
               //    if (MACAddress == String.Empty)
               //    {
               //        IPInterfaceProperties ipproperties = adapter.GetIPProperties();
               //        MACAddress = adapter.GetPhysicalAddress().ToString();
               //    }
               //}


                if(Permissoes.Autenticado(token))
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
                                                            .Select(u => new { id_users = u.id_users, nm_pessoa = u.pessoa.nm_pessoa, ds_login = u.ds_login })
                                                            .FirstOrDefault();

                            List<dynamic> permissoes = _db.webpages_UsersInRoles.Where(r => r.UserId == usuario.id_users)
                                                                                .Where(r => r.RoleId > 50)
                                                                                .Select(
                                                                                            r => new
                                                                                            {
                                                                                                RoleId = r.RoleId,
                                                                                                RolePrincipal = r.RolePrincipal,
                                                                                                ControllerPrincipal = r.webpages_Roles.webpages_Permissions.Where(p => p.fl_principal == true).Select( p => p.webpages_Methods.webpages_Controllers.id_controller ).FirstOrDefault(), // .FirstOrDefault().webpages_Methods.webpages_Controllers.id_controller,
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
                            .Where(e => list.Contains( e.id_controller ) )
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

                            return new Models.Object.Autenticado { nome = usuario.nm_pessoa, usuario = usuario.ds_login, token = token, controllers = controllers  };
                        }
                        else
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }
                }
                else
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                

            }
            catch(Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
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
                                                            .Select(u => new { id_users = u.id_users, nm_pessoa = u.pessoa.nm_pessoa, ds_login = u.ds_login })
                                                            .FirstOrDefault();

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
            catch(Exception e)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
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
