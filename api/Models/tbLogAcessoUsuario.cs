using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbLogAcessoUsuario
    {
        public int idLogAcessoUsuario { get; set; }
        public int idUser { get; set; }
        public string dsUrl { get; set; }
        public Nullable<int> idController { get; set; }
        public Nullable<int> idMethod { get; set; }
        public string dsParametros { get; set; }
        public string dsFiltros { get; set; }
        public System.DateTime dtAcesso { get; set; }
        public string dsAplicacao { get; set; }
        public int codResposta { get; set; }
        public string msgErro { get; set; }
        public string dsJson { get; set; }
    }
}