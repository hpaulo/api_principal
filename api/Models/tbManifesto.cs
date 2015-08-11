using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbManifesto
    {
        public int idManifesto { get; set; }
        public string nrChave { get; set; }
        public int cdGrupo { get; set; }
        public string nrCNPJ { get; set; }
        public string nrEmitenteCNPJCPF { get; set; }
        public string nmEmitente { get; set; }
        public string nrEmitenteIE { get; set; }
        public System.DateTime dtEmissao { get; set; }
        public string tpOperacao { get; set; }
        public decimal vlNFe { get; set; }
        public System.DateTime dtRecebimento { get; set; }
        public string nrNSU { get; set; }
        public string cdSituacaoNFe { get; set; }
        public string cdSituacaoManifesto { get; set; }
        public string dsSituacaoManifesto { get; set; }
        public string nrProtocoloManifesto { get; set; }
        public string xmlNFe { get; set; }
        public string nrProtocoloDownload { get; set; }
    }
}