using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbRecebimentoTEF
    {
        public int idRecebimentoTEF { get; set; }
        public byte cdGrupo { get; set; }
        public string nrCNPJ { get; set; }
        public string cdEstabelecimentoTEF { get; set; }
        public string nrPDVTEF { get; set; }
        public string nrNSU { get; set; }
        public string cdAutorizacao { get; set; }
        public int cdSitef { get; set; }
        public System.DateTime dtVenda { get; set; }
        public System.TimeSpan hrVenda { get; set; }
        public decimal vlVenda { get; set; }
        public string nrCartao { get; set; }
        public short cdBandeira { get; set; }
        public string nmOperadora { get; set; }
        public System.DateTime dthrVenda { get; set; }
    }
}
