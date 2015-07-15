using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using api.Models;

namespace api.Negocios.Administracao
{
    public class GatewayModulo
    {
        private Dictionary<string, Int32> modulos = new Dictionary<string, Int32>();

        public Dictionary<string, Int32> Modulos
        {
            get { return modulos; }
            set { modulos = value; }
        }


        public GatewayModulo()
        {
            Modulos.Add( "ADMINISTRACAO/WEBPAGESCONTROLLERS", GetIdController("") );
        }

        public static Int32 GetIdController( string ds_controller)
        {
            using (var _db = new painel_taxservices_dbContext())
            {
                webpages_Controllers controller = _db.webpages_Controllers
                                                     .Where( c => c.ds_controller.ToUpper().Equals(ds_controller.ToUpper()) )
                                                     .First<webpages_Controllers>();

                return controller != null ? controller.id_controller : 0;
            }
        }
    }
}