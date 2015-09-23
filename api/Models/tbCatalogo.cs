using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbCatalogo
    {
        public tbCatalogo()
        {
            this.tbNews = new List<tbNews>();
            //this.tbNewsGrupoes = new List<tbNewsGrupos>();
            this.tbCatalogoNewsGrupos = new List<tbCatalogoNewsGrupo>();
        }

        public short cdCatalogo { get; set; }
        public string dsCatalogo { get; set; }
        public virtual ICollection<tbNews> tbNews { get; set; }
        //public virtual ICollection<tbNewsGrupos> tbNewsGrupoes { get; set; }
        public virtual ICollection<tbCatalogoNewsGrupo> tbCatalogoNewsGrupos { get; set; }
    }
}