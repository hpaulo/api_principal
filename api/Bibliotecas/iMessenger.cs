using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using api.Models;

namespace api.Bibliotecas
{
    public class iMessenger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idNews"></param>
        /// <returns></returns>
        public static List<int> getIdUsersFromNews(tbNews noticias)
        {
            List<int> ids = new List<int>();

            using (var _db = new painel_taxservices_dbContext())
            {
                ids = _db.webpages_Users
                    .Where(u => u.tbAssinantes
                        .Where(c => c.tbCatalogo.tbNews
                            .Where(n => n.idNews == noticias.idNews).Count() > 0)
                        .Count() > 0)
                    .Where(u => noticias.cdEmpresaGrupo != null ? u.id_grupo == noticias.cdEmpresaGrupo.Value : true)
                    .Select(u => u.id_users)
                    .ToList<int>();
            }
            return ids;
        }
    }
}