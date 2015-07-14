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