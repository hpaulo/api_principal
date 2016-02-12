using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class AntecipacaoBancaria
    {
        public int idAntecipacaoBancaria { get; set; }
        public DateTime dtAntecipacaoBancaria { get; set; }
        public int cdAdquirente { get; set; }
        public int cdContaCorrente { get; set; }
        public decimal vlOperacao { get; set; }
        public decimal vlLiquido { get; set; }
        public List<AntecipacaoBancariaVencimentos> antecipacoes { get; set; }
        public List<int> deletar { get; set; }
    }

    public class AntecipacaoBancariaVencimentos
    {
        public int idAntecipacaoBancariaDetalhe { get; set; }
        public int? cdBandeira { get; set; }
        public DateTime dtVencimento { get; set; }
        public decimal vlAntecipacao { get; set; }
        //public decimal vlAntecipacaoLiquida { get; set; }
    }
}