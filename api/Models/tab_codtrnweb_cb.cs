using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tab_codtrnweb_cb
    {
        public decimal cod_trnweb { get; set; }
        public virtual transaco transaco { get; set; }
    }
}
