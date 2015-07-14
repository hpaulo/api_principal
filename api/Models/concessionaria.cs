using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class concessionaria
    {
        public concessionaria()
        {
            this.grp_concessionaria = new List<grp_concessionaria>();
        }

        public string cod_concessionaria { get; set; }
        public string descr_concessionaria { get; set; }
        public virtual ICollection<grp_concessionaria> grp_concessionaria { get; set; }
    }
}
