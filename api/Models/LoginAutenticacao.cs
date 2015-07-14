using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class LoginAutenticacao
    {
        public int idUsers { get; set; }
        public string token { get; set; }
        public Nullable<System.DateTime> dtValidade { get; set; }
        public virtual webpages_Users webpages_Users { get; set; }
    }
}
