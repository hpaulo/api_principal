using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class ConciliacaoPagamento
    {
        public int IdConciliacaoPagamento { get; set; }
        public string CNPJFilial { get; set; }
        public int IdPdv { get; set; }
        public int IdBandeira { get; set; }
        public System.DateTime DtMovimentoPagto { get; set; }
        public decimal VlVenda { get; set; }
        public string NsuHostTef { get; set; }
        public int NumParcelas { get; set; }
        public string NsuSitef { get; set; }
        public virtual Bandeira Bandeira { get; set; }
        public virtual PDV PDV { get; set; }
    }
}
