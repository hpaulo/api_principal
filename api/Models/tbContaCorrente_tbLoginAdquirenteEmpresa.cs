using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbContaCorrente_tbLoginAdquirenteEmpresa
    {
        public int cdContaCorrente { get; set; }
        public int cdLoginAdquirenteEmpresa { get; set; }
        public Nullable<System.DateTime> dtInicio { get; set; }
        public Nullable<System.DateTime> dtFim { get; set; }
        public virtual tbContaCorrente tbContaCorrente { get; set; }
        public virtual tbLoginAdquirenteEmpresa tbLoginAdquirenteEmpresa { get; set; }
    }
}