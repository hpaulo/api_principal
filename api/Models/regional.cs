using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class regional
    {
        public regional()
        {
            this.usuarios = new List<usuario>();
        }

        public string cod_regional { get; set; }
        public string descr_regional { get; set; }
        public virtual ICollection<usuario> usuarios { get; set; }
    }
}
