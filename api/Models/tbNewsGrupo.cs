using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbNewsGrupo
    {
        public tbNewsGrupo()
        {
            this.webpages_Users = new List<webpages_Users>();
            this.tbCatalogoes = new List<tbCatalogo>();
            this.tbAssinantes = new List<tbAssinante>();
        }

        public int cdNewsGrupo { get; set; }
        public int cdEmpresaGrupo { get; set; }
        public string dsNewsGrupo { get; set; }
        public virtual ICollection<webpages_Users> webpages_Users { get; set; }
        public virtual ICollection<tbCatalogo> tbCatalogoes { get; set; }
        public virtual ICollection<tbAssinante> tbAssinantes { get; set; }
        public virtual ICollection<tbCatalogoNewsGrupo> tbCatalogoNewsGrupos { get; set; }
    }
}