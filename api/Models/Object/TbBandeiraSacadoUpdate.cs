using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class TbBandeiraSacadoUpdate
    {
        public int cdGrupo { get; set; }
        public string oldCdSacado { get; set; }
        public int oldCdBandeira { get; set; }
        public byte oldQtParcelas { get; set; }
        public string newCdSacado { get; set; }
        public int newCdBandeira { get; set; }
        public byte newQtParcelas { get; set; }
    }
}