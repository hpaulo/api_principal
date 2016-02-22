using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class tbAntecipacaoBancariaDetalhe
    {
        public int idAntecipacaoBancariaDetalhe { get; set; }
        public int idAntecipacaoBancaria { get; set; }
        public DateTime dtVencimento { get; set; }
        public decimal vlAntecipacao { get; set; }
        public decimal vlAntecipacaoLiquida { get; set; }
        public int? cdBandeira { get; set; }
        public decimal vlIOF { get; set; }
        public decimal vlIOFAdicional { get; set; }
        public decimal vlJuros { get; set; }
        public virtual tbAntecipacaoBancaria tbAntecipacaoBancaria { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
    }
}