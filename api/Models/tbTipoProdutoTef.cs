using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbTipoProdutoTef
    {
        public tbTipoProdutoTef()
        {
            this.tbProdutoTefs = new List<tbProdutoTef>();
        }

        public int cdTipoProdutoTef { get; set; }
        public string dsTipoProdutoTef { get; set; }
        public virtual ICollection<tbProdutoTef> tbProdutoTefs { get; set; }
    }
}
