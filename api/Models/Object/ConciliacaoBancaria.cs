using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{

    public class ConciliacaoBancaria
    {
        private char tipo; // 'E' : extrato, 'R' : RecebimentoParcela
        public char Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private List<Int32> ids; // id do extrato ou do recebimento
        public List<Int32> Ids
        {
            get { return ids; }
            set { ids = value; }
        }

        private DateTime data; // data do extrato ou do recebimento
        public DateTime Data
        {
            get { return data; }
            set { data = value; }
        }

        private List<decimal> valores; // valor do Extrato ou valor liquido do recebimento parcela
        public List<decimal> Valores
        {
            get { return valores; }
            set { valores = value; }
        }

        private string adquirente; 
        public string Adquirente
        {
            get { return adquirente; }
            set { adquirente = value; }
        }

        private tbContaCorrente conta; // conta bancária associada
        public tbContaCorrente Conta
        {
            get { return conta; }
            set { conta = value; }
        }


        /*public class Comparer : IEqualityComparer<ConciliacaoBancaria>
        {
            public bool Equals(ConciliacaoBancaria c1, ConciliacaoBancaria c2)
            {
                if (c1 == c2) return true; // é a mesma instância ou ambos são nulos
                if (c1 == null || c2 == null) return false;
                return c1.Id == c2.Id;
            }

            public int GetHashCode(ConciliacaoBancaria c)
            {
                return c != null ? c.Id : 0;
            }
        }*/

    }
}
