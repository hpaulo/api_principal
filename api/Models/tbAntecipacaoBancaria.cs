using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class tbAntecipacaoBancaria
    {
        public int idAntecipacaoBancaria { get; set; }
        public DateTime dtAntecipacaoBancaria { get; set; }
        public DateTime dtVencimento { get; set; }
        public decimal vlAntecipacao { get; set; }
        public decimal vlAntecipacaoLiquida { get; set; }
        public int cdAdquirente { get; set; }
        public int? cdBandeira { get; set; }
        public int cdContaCorrente { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
        public virtual tbContaCorrente tbContaCorrente { get; set; }
    }
}