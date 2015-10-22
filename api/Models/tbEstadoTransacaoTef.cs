using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbEstadoTransacaoTef
    {
        public tbEstadoTransacaoTef()
        {
            this.tbRecebimentoTEFs = new List<tbRecebimentoTEF>();
        }

        public Int32 cdEstadoTransacaoTef { get; set; }
        public string dsEstadoTransacaoTef { get; set; }
        public virtual ICollection<tbRecebimentoTEF> tbRecebimentoTEFs { get; set; }
    }
}
