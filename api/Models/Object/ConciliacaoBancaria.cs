using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{

    public class ConciliacaoBancaria
    {
        private string tipo; // "E" : extrato, "R" : RecebimentoParcela
        public string Tipo
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

        private string bandeira;
        public string Bandeira
        {
            get { return bandeira; }
            set { bandeira = value; }
        }

        private ConciliacaoConta conta; // conta bancária associada
        public ConciliacaoConta Conta
        {
            get { return conta; }
            set { conta = value; }
        }

        public class ConciliacaoConta
        {
            private Int32 cdContaCorrente;
            public Int32 CdContaCorrente
            {
                get { return cdContaCorrente; }
                set { cdContaCorrente = value; }
            }

            private string cdBanco;
            public string CdBanco
            {
                get { return cdBanco; }
                set { cdBanco = value; }
            }

            private string nrAgencia;
            public string NrAgencia
            {
                get { return nrAgencia; }
                set { nrAgencia = value; }
            }

            private string nrConta; 
            public string NrConta
            {
                get { return nrConta; }
                set { nrConta = value; }
            }

            
        }


        public class Comparer : IEqualityComparer<ConciliacaoBancaria>
        {
            public bool Equals(ConciliacaoBancaria c1, ConciliacaoBancaria c2)
            {
                if (c1 == c2) return true; // é a mesma instância ou ambos são nulos
                if (c1 == null || c2 == null) return false;
                return c1.Data.Year == c2.Data.Year && c1.Data.Month == c2.Data.Month && c1.Data.Day == c2.Data.Day &&
                       c1.Valores.Sum() == c2.Valores.Sum() && 
                       c1.Adquirente.Equals(c2.Adquirente);
            }

            public int GetHashCode(ConciliacaoBancaria c)
            {
                if (c == null) return 0;
                DateTime d = new DateTime(c.Data.Year, c.Data.Month, c.Data.Day);
                return ((int)d.Ticks) + c.Adquirente.GetHashCode() + ((int)c.Valores.Sum());
            }
        }

    }
}
