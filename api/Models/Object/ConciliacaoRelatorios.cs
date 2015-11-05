using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class ConciliacaoRelatorios
    {
        public string tipo { get; set; }  // Diferenciar registros de cada Gateway
        public string adquirente { get; set; }
        public string bandeira { get; set; }
        public string tipocartao { get; set; }
        public DateTime competencia { get; set; }   // Data do filtro
        
        // variáveis específicas de RecebimentoParcela
        public int? idExtrato { get; set; }
        public decimal valorDescontadoAntecipacao { get; set; }
        public decimal valorDescontado { get; set; }
        public decimal valorBruto { get; set; }
        public decimal valorLiquido { get; set; }

        public decimal taxaCashFlow { get; set; }
        //public decimal vendas { get; set; }
        //public decimal taxaAdm { get; set; }
        //public decimal vlAjuste { get; set; }   // tbRecebimentoAjustes
        //public decimal ajustesCredito { get; set; }
        //public decimal ajustesDebito { get; set; }
        //public decimal valorLiquido { get; set; }
        //public decimal extratoBancario { get; set; }
        //public decimal diferença { get; set; }
    }
}