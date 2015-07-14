using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class TipoPagamento
    {
        public TipoPagamento()
        {
            this.Bandeiras = new List<Bandeira>();
            this.Bandeiras1 = new List<Bandeira1>();
        }

        public int IdTipoPagamento { get; set; }
        public string Descricao { get; set; }
        public virtual ICollection<Bandeira> Bandeiras { get; set; }
        public virtual ICollection<Bandeira1> Bandeiras1 { get; set; }
    }
}
