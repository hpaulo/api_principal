using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{
    public class RelatorioVendas
    {
        public DateTime dataVenda;
        public DateTime dataRecebimento;
        public bool recebeu;
        public decimal valorBruto;
        public decimal valorDescontado;
        public decimal valorLiquido;
        public string bandeira;
        public string adquirente;
        public string diaVenda;
    }
}
