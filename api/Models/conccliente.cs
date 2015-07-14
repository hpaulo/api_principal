using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class conccliente
    {
        public decimal cod_sit { get; set; }
        public string data_trn { get; set; }
        public string nsu_sitef { get; set; }
        public string codlojasitef { get; set; }
        public string codcliente { get; set; }
        public string cuponfiscal { get; set; }
        public Nullable<decimal> se_cliente { get; set; }
    }
}
