using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbRecebimentoTEF
    {
        public int idRecebimentoTEF { get; set; }
        public Nullable<int> cdGrupo { get; set; }
        public string nrCNPJ { get; set; }
        public string cdEmpresaTEF { get; set; }
        public string nrPDVTEF { get; set; }
        public string nrNSUHost { get; set; }
        public string nrNSUTEF { get; set; }
        public string cdAutorizacao { get; set; }
        public int cdSituacaoRedeTEF { get; set; }
        public System.DateTime dtVenda { get; set; }
        public System.TimeSpan hrVenda { get; set; }
        public decimal vlVenda { get; set; }
        public Nullable<int> qtParcelas { get; set; }
        public string nrCartao { get; set; }
        public Nullable<int> cdBandeira { get; set; }
        public string nmOperadora { get; set; }
        public Nullable<System.DateTime> dthrVenda { get; set; }
        public Nullable<int> cdEstadoTransacaoTEF { get; set; }
        public Nullable<int> cdTrasacaoTEF { get; set; }
        public Nullable<int> cdModoEntradaTEF { get; set; }
        public Nullable<int> cdRedeTEF { get; set; }
        public Nullable<int> cdProdutoTEF { get; set; }
        public Nullable<int> cdBandeiraTEF { get; set; }
        public string cdEstabelecimentoHost { get; set; }
        public virtual tbEstadoTransacaoTef tbEstadoTransacaoTef { get; set; }
        public virtual tbProdutoTef tbProdutoTef { get; set; }
        public virtual grupo_empresa grupo_empresa { get; set; }
    }
}
