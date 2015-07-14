using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tab_tipodoc_cb
    {
        public tab_tipodoc_cb()
        {
            this.logcbs = new List<logcb>();
        }

        public decimal tipodoc { get; set; }
        public string descr_tipodoc { get; set; }
        public virtual ICollection<logcb> logcbs { get; set; }
    }
}
