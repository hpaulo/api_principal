using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbEmpresa
    {
        public string nrCNPJBase { get; set; }
        public byte[] dsCertificadoDigital { get; set; }
        public string dsCertificadoDigitalSenha { get; set; }
        public int cdEmpresaGrupo { get; set; }
        public virtual tbEmpresaGrupo tbEmpresaGrupo { get; set; }
    }
}
