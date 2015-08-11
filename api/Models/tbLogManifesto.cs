using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbLogManifesto
    {
        public int idLog { get; set; }
        public System.DateTime dtLog { get; set; }
        public string dsComando { get; set; }
        public string cdRetorno { get; set; }
        public string dsRetorno { get; set; }
        public string dsMetodo { get; set; }
        public string tpLog { get; set; }
    }
}