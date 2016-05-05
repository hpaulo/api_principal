using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class tbBandeiraSacado
    {
        public int cdGrupo {get; set; }
        public string cdSacado { get; set; }
        public int cdBandeira { get; set; }
        public byte qtParcelas { get; set; }
        public virtual grupo_empresa grupo_empresa { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }

    }
}