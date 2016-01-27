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
            this.tbRecebimentoAjustes = new List<tbRecebimentoAjuste>();
        }

        public int idResumoVenda { get; set; }
        public string cdResumoVenda { get; set; }
        public DateTime dtVenda { get; set; }
        public DateTime dtRecebimentoPrevisto { get; set; }
        public decimal vlTotal { get; set; }
        public Int16 qtCVs { get; set; }
        public int cdBandeira { get; set; }
        public string nrCNPJ { get; set; }
        public byte nrLinha { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual ICollection<Recebimento> Recebimentos { get; set; }
        public virtual ICollection<tbRecebimentoAjuste> tbRecebimentoAjustes { get; set; }
    }
}