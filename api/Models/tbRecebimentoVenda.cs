using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class tbRecebimentoVenda
    {
        public tbRecebimentoVenda()
        {
            this.Recebimentos = new List<Recebimento>();
        }

        public int idRecebimentoVenda{ get; set; } 
        public string nrCNPJ { get; set; } 
        public string nrNSU { get; set; } 
        public DateTime dtVenda { get; set; }
        //public int cdAdquirente { get; set; } 
        public string dsBandeira { get; set; }
        public decimal vlVenda { get; set; }
        public byte qtParcelas { get; set; }
        public string cdERP { get; set; }
        public string cdSacado { get; set; }
        public DateTime? dtAjuste { get; set; }
        public bool flInsert { get; set; }
        //public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual ICollection<Recebimento> Recebimentos { get; set; }
    }
}