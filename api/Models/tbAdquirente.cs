using api.Models.Object;
using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbAdquirente
    {
        public tbAdquirente()
        {
            //this.tbRecebimento = new List<tbRecebimento>();
            this.tbLoginAdquirenteEmpresas = new List<tbLoginAdquirenteEmpresa>();
            this.tbBancoParametros = new List<tbBancoParametro>();
            this.tbBandeiras = new List<tbBandeira>();
            this.tbTerminalLogicos = new List<tbTerminalLogico>();
            this.tbRecebimentoResumoManuals = new List<tbRecebimentoResumoManual>();
            this.tbLogCargas = new List<tbLogCarga>();
        }

        public int cdAdquirente { get; set; }
        public string nmAdquirente { get; set; }
        public string dsAdquirente { get; set; }
        public byte stAdquirente { get; set; }
        public System.DateTime hrExecucao { get; set; }

        //public virtual ICollection<tbRecebimento> tbRecebimento { get; set; }
        public virtual ICollection<tbLoginAdquirenteEmpresa> tbLoginAdquirenteEmpresas { get; set; }
        public virtual ICollection<tbBancoParametro> tbBancoParametros { get; set; }
        public virtual ICollection<tbLogCarga> tbLogCargas { get; set; }
        public virtual ICollection<tbBandeira> tbBandeiras { get; set; }
        public virtual ICollection<tbTerminalLogico> tbTerminalLogicos { get; set; }
        public virtual ICollection<tbRecebimentoResumoManual> tbRecebimentoResumoManuals { get; set; }
    }
}
