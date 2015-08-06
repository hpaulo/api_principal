using api.Bibliotecas;
using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbContaCorrente
    {
        public tbContaCorrente()
        {
            this.tbContaCorrente_tbLoginAdquirenteEmpresa = new List<tbContaCorrente_tbLoginAdquirenteEmpresa>();
        }

        public int idContaCorrente { get; set; }
        public int cdGrupo { get; set; }
        public string nrCnpj { get; set; }
        public string cdBanco { get; set; }
        public string nrAgencia { get; set; }
        public string nrConta { get; set; }
        public virtual ICollection<tbContaCorrente_tbLoginAdquirenteEmpresa> tbContaCorrente_tbLoginAdquirenteEmpresa { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual grupo_empresa grupo_empresa { get; set; }
    }
}