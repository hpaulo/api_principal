using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class tbResumoVenda
    {
        public tbResumoVenda()
        {
            this.Recebimentos = new List<Recebimento>();
        }

        public int idResumoVenda { get; set; }
        public string cdResumoVenda { get; set; }
        public DateTime dtVenda { get; set; }
        public DateTime dtRecebimentoPrevisto { get; set; }
        public decimal vlTotal { get; set; }
        public Int16 qtVCs { get; set; }
        public int cdBandeira { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
        public virtual ICollection<Recebimento> Recebimentos { get; set; }
    }
}