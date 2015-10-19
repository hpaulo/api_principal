using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{
    public class RecebiveisFuturos
    {
        public string nome;
        public Nullable<DateTime> data;
        public decimal valorBruto;
        public decimal valorDescontado;
        public decimal valorLiquido;
        public string bandeira;
        public string adquirente;
        public string competencia;

        public RecebiveisFuturos()
        {
            nome = String.Empty;
            data = null;
            valorBruto = new decimal(0.0);
            valorDescontado = new decimal(0.0);
            valorLiquido = new decimal(0.0);
            bandeira = String.Empty;
            adquirente = String.Empty;
            competencia = String.Empty;
        }
    }
}
