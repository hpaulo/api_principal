using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class pessoa
    {
        public pessoa()
        {
            this.webpages_Users = new List<webpages_Users>();
        }

        public int id_pesssoa { get; set; }
        public string nm_pessoa { get; set; }
        public Nullable<System.DateTime> dt_nascimento { get; set; }
        public string nu_telefone { get; set; }
        public string nu_ramal { get; set; }
        public virtual ICollection<webpages_Users> webpages_Users { get; set; }
    }
}
