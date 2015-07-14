using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class modoentrada
    {
        public modoentrada()
        {
            this.convmodoentradas = new List<convmodoentrada>();
            this.convtransacoes = new List<convtransaco>();
            this.logtefs = new List<logtef>();
        }

        public decimal cdmodoentrada { get; set; }
        public string descr_modoent { get; set; }
        public virtual ICollection<convmodoentrada> convmodoentradas { get; set; }
        public virtual ICollection<convtransaco> convtransacoes { get; set; }
        public virtual ICollection<logtef> logtefs { get; set; }
    }
}
