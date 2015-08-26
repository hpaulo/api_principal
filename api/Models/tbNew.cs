using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbNew
    {
        public tbNew()
        {
            this.tbNewsStatus = new List<tbNewsStatu>();
        }

        public int idNews { get; set; }
        public string dsNews { get; set; }
        public System.DateTime dtNews { get; set; }
        public Nullable<int> cdEmpresaGrupo { get; set; }
        public short cdCatalogo { get; set; }
        public short cdCanal { get; set; }
        public string cdReporter { get; set; }
        public Nullable<System.DateTime> dtEnvio { get; set; }
        public virtual tbCanal tbCanal { get; set; }
        public virtual tbCatalogo tbCatalogo { get; set; }
        public virtual ICollection<tbNewsStatu> tbNewsStatus { get; set; }
    }
}