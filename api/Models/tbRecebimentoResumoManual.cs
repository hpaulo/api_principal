using System;

namespace api.Models
{
    public class tbRecebimentoResumoManual
    {
        public int idRecebimentoResumoManual { get; set; }
        public string cdTerminalLogico { get; set; }
        public Nullable<int> cdAdquirente { get; set; }
        public DateTime dtVenda { get; set; }
        public decimal vlVenda { get; set; }
        public Nullable<byte> qtTracacao { get; set; }        
        public Nullable<int> cdBandeira { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
        public virtual tbTerminalLogico tbTerminalLogico { get; set; }
    }
}
