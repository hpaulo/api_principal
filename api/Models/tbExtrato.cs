using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbExtrato
    {
        public int idExtrato { get; set; }
        public int cdContaCorrente { get; set; }
        public string nrDocumento { get; set; }
        public System.DateTime dtExtrato { get; set; }
        public Nullable<decimal> vlMovimento { get; set; }
    }
}