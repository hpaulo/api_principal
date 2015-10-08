using api.Models.Object;
using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbBandeira
    {
        public tbBandeira()
        {
            this.Recebimentos = new List<Recebimento>();
            this.tbBancoParametros = new List<tbBancoParametro>();
            this.tbRecebimentoAjustes = new List<tbRecebimentoAjuste>();
            this.tbRecebimentoResumoManuals = new List<tbRecebimentoResumoManual>();
        }

        public int cdBandeira { get; set; }
        public string dsBandeira { get; set; }
        public int cdAdquirente { get; set; }
        public string dsTipo { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual ICollection<Recebimento> Recebimentos { get; set; }
        public virtual ICollection<tbBancoParametro> tbBancoParametros { get; set; }
        public virtual ICollection<tbRecebimentoAjuste> tbRecebimentoAjustes { get; set; }
        public virtual ICollection<tbRecebimentoResumoManual> tbRecebimentoResumoManuals { get; set; }
    }
}
