using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbTerminalLogico
    {
        public tbTerminalLogico()
        {
            this.tbRecebimentoResumoManuals = new List<tbRecebimentoResumoManual>();
        }

        public string cdTerminalLogico { get; set; }
        public int cdAdquirente { get; set; }
        public string nrCNPJ { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual ICollection<tbRecebimentoResumoManual> tbRecebimentoResumoManuals { get; set; }
    }
}
