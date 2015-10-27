using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbProdutoTef
    {
        public tbProdutoTef()
        {
            this.tbRecebimentoTEFs = new List<tbRecebimentoTEF>();
        }

        public Int32 cdProdutoTef { get; set; }
        public Nullable<int> cdTipoProdutoTef { get; set; }
        public string dsProdutoTef { get; set; }
        public virtual tbTipoProdutoTef tbTipoProdutoTef { get; set; }
        public virtual ICollection<tbRecebimentoTEF> tbRecebimentoTEFs { get; set; }
    }
}
