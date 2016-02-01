using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{
    public partial class tbBancoParametro
    { 
        public string cdBanco { get; set; }
        public string dsMemo { get; set; }
        public Nullable<int> cdAdquirente { get; set; }
        public string dsTipo { get; set; }
        public bool flVisivel { get; set; }
        public string nrCnpj { get; set; }
        public string dsTipoCartao { get; set; }
        public Nullable<int> cdBandeira { get; set; }
        public bool flAntecipacao { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual tbBandeira tbBandeira { get; set; }
    }
}
