using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class tbLogCarga
    {
        public tbLogCarga()
        {
            this.tbLogCargaDetalhes = new List<tbLogCargaDetalhe>();
        }

        public int idLogCarga { get; set; }
        public System.DateTime dtCompetencia { get; set; }
        public string nrCNPJ { get; set; }
        public int cdAdquirente { get; set; }
        public bool flStatusVenda { get; set; }
        public bool flStatusPagos { get; set; }
        public bool flStatusReceber { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual ICollection<tbLogCargaDetalhe> tbLogCargaDetalhes { get; set; }
    }
}