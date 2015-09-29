using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbCatalogo
    {
        public tbCatalogo()
        {
            this.tbNews = new List<tbNews>();            
            this.tbAssinantes = new List<tbAssinante>();
        }

        public short cdCatalogo { get; set; }
        public string dsCatalogo { get; set; }
        public virtual ICollection<tbNews> tbNews { get; set; }        
        public virtual ICollection<tbAssinante> tbAssinantes { get; set; }
    }
}