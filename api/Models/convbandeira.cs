using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class convbandeira
    {
        public decimal operacaotef { get; set; }
        public string mascara_bin { get; set; }
        public decimal tam_cartao { get; set; }
        public Nullable<decimal> idt_bandeira { get; set; }
        public virtual bandeiras1 bandeiras1 { get; set; }
    }
}
