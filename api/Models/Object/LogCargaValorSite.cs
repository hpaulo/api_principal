using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class LogCargaValorSite
    {
        public string nrCNPJ { get; set; }
        public string adquirente { get; set; }
        public DateTime dtCompetencia { get; set; }
        public bool processouModalidade { get; set; }
        public decimal valorSite { get; set; }
    }
}