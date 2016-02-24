using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class EmpresaAtualizar
    {
        public string novo_cnpj { get; set; }
        public string nu_cnpj { get; set; }
        public string ds_fantasia { get; set; }
        public string ds_razaoSocial { get; set; }
        public string ds_endereco { get; set; }
        public string ds_cidade { get; set; }
        public string sg_uf { get; set; }
        public string nu_cep { get; set; }
        public string nu_telefone { get; set; }
        public string ds_bairro { get; set; }
        public string ds_email { get; set; }
        public long fl_ativo { get; set; }
        public string token { get; set; }
        public int id_grupo { get; set; }
        public string filial { get; set; }
        public Nullable<long> nu_inscEstadual { get; set; }
    }
}