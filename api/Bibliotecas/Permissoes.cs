using api.Models;
using api.Models.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace api.Bibliotecas
{
    public class Permissoes
    {
        public static bool Autenticado(string token)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                var verify = (from v in _db.LoginAutenticacaos
                              where v.token.Equals(token)
                              select v
                             ).FirstOrDefault();

                if (verify != null)
                    return true;
            }
            return false;
        }


        public static Int32 GetIdUser(string token)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                var verify = (from v in _db.LoginAutenticacaos
                              where v.token.Equals(token)
                              select v
                             ).FirstOrDefault();

                if (verify != null)
                    return verify.idUsers;
            }
            return 0;
        }

        public static Boolean GetPermissionMethod(string token, string ds_method)
        {
            //using (var _db = new painel_taxservices_dbContext())
            //{
            //    Int32 IdUser = Permissoes.GetIdUser(token);


            //    List<dynamic> List = _db.webpages_UsersInRoles.Where(r => r.UserId == IdUser)
            //                                        .Where(r => r.RoleId > 50)
            //                                        .Select(r => new
            //                                        {
            //                                            methods = _db.webpages_Permissions
            //                                                             .Where(p => p.id_roles == r.RoleId)
            //                                                             .Where(p => p.webpages_Methods.webpages_Controllers.id_controller == IdController)
            //                                                             .Select(p => new { ds_method = p.webpages_Methods.ds_method, id_method = p.webpages_Methods.id_method })
            //                                                             .ToList<dynamic>()
            //                                        }
            //                                                ).ToList<dynamic>();


                return false;
            //}
        }




       /* public static bool Autenticad(string token)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                _db.Configuration.ProxyCreationEnabled = false;

                var verify = (from v in _db.LoginAutenticacaos
                              where v.token.Equals(token)
                              select v
                             ).FirstOrDefault();

                            if (verify != null)

                            #region Log de Acesso ao Sistema
                            Log log = new api.Models.Object.Log();
                            log.IdUser = verify.idUsers;
                            log.IdController = 45;
                            log.IdMethod = 51;
                            log.DtAcesso = DateTime.Now;

                            LogAcesso.New(log);
                            #endregion

                                return true;
            }
            return false;
        }*/


    }
}