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
        public virtual tbAdquirente tbAdquirente { get; set; }
    }
}
