using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class tbAntecipacaoBancaria
    {
        public tbAntecipacaoBancaria()
        {
            this.tbAntecipacaoBancariaDetalhes = new List<tbAntecipacaoBancariaDetalhe>();
        }

        public int idAntecipacaoBancaria { get; set; }
        public DateTime dtAntecipacaoBancaria { get; set; }
        public decimal? vlOperacao { get; set; }
        public decimal? vlLiquido { get; set; }
        public int cdAdquirente { get; set; }
        public int cdContaCorrente { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual tbContaCorrente tbContaCorrente { get; set; }
        public ICollection<tbAntecipacaoBancariaDetalhe> tbAntecipacaoBancariaDetalhes { get; set; }
    }
}