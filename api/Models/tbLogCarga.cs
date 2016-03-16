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
        public bool flStatusVendasCredito { get; set; }
        public bool flStatusVendasDebito { get; set; }
        public bool flStatusPagosCredito { get; set; }
        public bool flStatusPagosDebito { get; set; }
        public bool flStatusPagosAntecipacao { get; set; }
        public bool flStatusReceber { get; set; }
        public decimal vlVendaCredito { get; set; }
        public decimal vlVendaDebito { get; set; }
        public decimal vlPagosCredito { get; set; }
        public decimal vlPagosDebito { get; set; }
        public decimal vlPagosAntecipacao { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual ICollection<tbLogCargaDetalhe> tbLogCargaDetalhes { get; set; }
    }
}