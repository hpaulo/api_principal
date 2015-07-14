using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class Lixo
    {
        public byte Loja { get; set; }
        public Nullable<byte> PDV { get; set; }
        public Nullable<int> Transacao { get; set; }
        public decimal ValorComJuros { get; set; }
        public Nullable<decimal> ValorSemJuros { get; set; }
    }
}
