using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbCanal
    {
        public tbCanal()
        {
            this.tbNews = new List<tbNews>();
        }

        public short cdCanal { get; set; }
        public string dsCanal { get; set; }
        public virtual ICollection<tbNews> tbNews { get; set; }
    }
}