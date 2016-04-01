using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class ConciliacaoVendas
    {
        public string Tipo; // "V" : tbRecebimentoVenda, "R" : Recebimento
        public Int32 Id;
        public string Nsu;
        public string CodResumoVendas;
        public DateTime Data; // Data da venda
        public string Filial;
        public string Adquirente;
        public string Bandeira;
        public decimal Valor;
        public Int32 Parcelas;
    }
}