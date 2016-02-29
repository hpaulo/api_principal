using System;

namespace api.Models
{
    public partial class tbRecebimentoAjuste
    {
        public int idRecebimentoAjuste { get; set; }
        public System.DateTime dtAjuste { get; set; }
        public string nrCNPJ { get; set; }
        public int cdBandeira { get; set; }
        public string dsMotivo { get; set; }
        public decimal vlAjuste { get; set; }
        public Nullable<int> idExtrato { get; set; }
        public Nullable<int> idResumoVenda { get; set; }
        public bool flAntecipacao { get; set; }
        public Nullable<int> idAntecipacaoBancariaDetalhe { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual tbExtrato tbExtrato { get; set; }
        public virtual tbResumoVenda tbResumoVenda { get; set; }
        public virtual tbAntecipacaoBancariaDetalhe tbAntecipacaoBancariaDetalhe { get; set; }
    }
}
