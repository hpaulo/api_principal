using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models
{
    public partial class tbAssinante
    {
        public int cdNewsGrupo { get; set; }
        public int cdUser { get; set; }
        public virtual webpages_Users webpages_Users { get; set; }
        public virtual tbNewsGrupo tbNewsGrupos { get; set; }
    }
}
