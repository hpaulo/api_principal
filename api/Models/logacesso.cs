using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class logacesso
    {
        public string cod_usuario { get; set; }
        public string datahora { get; set; }
        public string statusacesso { get; set; }
    }
}
