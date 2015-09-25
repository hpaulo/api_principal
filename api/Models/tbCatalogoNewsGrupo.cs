using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models
{
    public partial class tbCatalogoNewsGrupo
    {
        public short cdCatalogo { get; set; }
        public int cdNewsGrupo { get; set; }
        public virtual tbCatalogo tbCatalogo { get; set; }
        public virtual tbNewsGrupo tbNewsGrupo { get; set; }
    }
}
