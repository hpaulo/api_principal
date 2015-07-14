using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class historico_senhas
    {
        public string cod_usuario { get; set; }
        public string senha { get; set; }
        public Nullable<System.DateTime> cadastro { get; set; }
        public decimal contador { get; set; }
        public virtual usuario usuario { get; set; }
    }
}
