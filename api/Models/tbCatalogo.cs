using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbCatalogo
    {
        public tbCatalogo()
        {
            this.tbNews = new List<tbNew>();
            this.tbNewsGrupoes = new List<tbNewsGrupo>();
        }

        public short cdCatalogo { get; set; }
        public string dsCatalogo { get; set; }
        public virtual ICollection<tbNew> tbNews { get; set; }
        public virtual ICollection<tbNewsGrupo> tbNewsGrupoes { get; set; }
    }
}