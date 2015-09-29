using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models
{
    public partial class tbAssinante
    {
        public short cdCatalogo { get; set; }
        public int id_users { get; set; }
        public virtual webpages_Users webpages_Users { get; set; }
        public virtual tbCatalogo tbCatalogo { get; set; }
    }
}
