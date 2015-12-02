using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbLoginAdquirenteEmpresa
    {

        public tbLoginAdquirenteEmpresa()
        {
            this.tbContaCorrente_tbLoginAdquirenteEmpresas = new List<tbContaCorrente_tbLoginAdquirenteEmpresa>();
            //this.tbExecucaoLog = new List<tbExecucaoLog>();
        }

        public int cdLoginAdquirenteEmpresa { get; set; }
        public int cdAdquirente { get; set; }
        public int cdGrupo { get; set; }
        public string nrCnpj { get; set; }
        public string dsLogin { get; set; }
        public string dsSenha { get; set; }
        public string cdEstabelecimento { get; set; }
        public Nullable<System.DateTime> dtAlteracao { get; set; }
        public byte stLoginAdquirente { get; set; }
        public byte stLoginAdquirenteEmpresa { get; set; }
        public string nrCNPJCentralizadora { get; set; }
        public string cdEstabelecimentoConsulta { get; set; }
        public Nullable<DateTime> dtBloqueio { get; set; }
        public byte qtTentativas { get; set; }
        public virtual ICollection<tbContaCorrente_tbLoginAdquirenteEmpresa> tbContaCorrente_tbLoginAdquirenteEmpresas { get; set; }
        //public virtual ICollection<tbExecucaoLog> tbExecucaoLog { get; set; }
        public virtual grupo_empresa grupo_empresa { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual empresa empresaCentralizadora { get; set; }
    }
}
