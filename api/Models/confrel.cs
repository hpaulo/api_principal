using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class confrel
    {
        public string cod_usuario { get; set; }
        public string logrede { get; set; }
        public string colunalog { get; set; }
        public string descricao { get; set; }
        public decimal ordem { get; set; }
        public decimal formato { get; set; }
        public decimal tamcoluna { get; set; }
    }
}
