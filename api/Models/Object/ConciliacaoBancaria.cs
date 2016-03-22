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

        private DateTime data; // data do extrato ou do recebimento
        public DateTime Data
        {
            get { return data; }
            set { data = value; }
        }

        private Nullable<DateTime> dataVenda; // data do extrato ou do recebimento
        public Nullable<DateTime> DataVenda
        {
            get { return dataVenda; }
            set { dataVenda = value; }
        }

        private List<ConciliacaoGrupo> grupo;
        public List<ConciliacaoGrupo> Grupo
        {
            get { return grupo; }
            set { grupo = value; }
        }

        private string filial;
        public string Filial
        {
            get { return filial; }
            set { filial = value; }
        }

        //public string CNPJ { get; set; }

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

        private int lote;
        public int Lote
        {
            get { return lote; }
            set { lote = value; }
        }

        private decimal antecipacaoBancaria;
        public decimal AntecipacaoBancaria
        {
            get { return antecipacaoBancaria; }
            set { antecipacaoBancaria = value; }
        }

        private string tipoCartao;
        public string TipoCartao
        {
            get { return tipoCartao; }
            set { tipoCartao = value; }
        }

        private string memo;
        public string Memo
        {
            get { return memo; }
            set { memo = value; }
        }

        private decimal valortotal;
        public decimal ValorTotal
        {
            get { return valortotal; }
            set { valortotal = value; }
        }

        private Nullable<decimal> valortotalbruto;
        public Nullable<decimal> ValorTotalBruto
        {
            get { return valortotalbruto; }
            set { valortotalbruto = value; }
        }

        private ConciliacaoConta conta; // conta bancária associada
        public ConciliacaoConta Conta
        {
            get { return conta; }
            set { conta = value; }
        }

        private Nullable<bool> antecipado;
        public Nullable<bool> Antecipado
        {
            get { return antecipado; }
            set { antecipado = value; }
        }

        private Nullable<bool> dataRecebimentoDiferente;
        public Nullable<bool> DataRecebimentoDiferente
        {
            get { return dataRecebimentoDiferente; }
            set { dataRecebimentoDiferente = value; }
        }



        public class ConciliacaoGrupo
        {
            private Int32 id; // id do extrato ou do recebimento
            public Int32 Id
            {
                get { return id; }
                set { id = value; }
            }

            private decimal valor; // valor do Extrato ou valor liquido do recebimento parcela
            public decimal Valor
            {
                get { return valor; }
                set { valor = value; }
            }

            private Nullable<decimal> valorBruto;
            public Nullable<decimal> ValorBruto
            {
                get { return valorBruto; }
                set { valorBruto = value; }
            }

            private string documento;
            public string Documento
            {
                get { return documento; }
                set { documento = value; }
            }

            private string filial;
            public string Filial
            {
                get { return filial; }
                set { filial = value; }
            }

            private string bandeira;
            public string Bandeira
            {
                get { return bandeira; }
                set { bandeira = value; }
            }

            private int lote;
            public int Lote
            {
                get { return lote; }
                set { lote = value; }
            }

            private decimal antecipacaoBancaria;
            public decimal AntecipacaoBancaria
            {
                get { return antecipacaoBancaria; }
                set { antecipacaoBancaria = value; }
            }

            private string tipoCartao;
            public string TipoCartao
            {
                get { return tipoCartao; }
                set { tipoCartao = value; }
            }

            private Nullable<DateTime> dataVenda; // data do extrato ou do recebimento
            public Nullable<DateTime> DataVenda
            {
                get { return dataVenda; }
                set { dataVenda = value; }
            }

            private Nullable<DateTime> dataPrevista; // data de recebimento prevista
            public Nullable<DateTime> DataPrevista
            {
                get { return dataPrevista; }
                set { dataPrevista = value; }
            }

            private Nullable<int> numParcela;
            public Nullable<int> NumParcela
            {
                get { return numParcela; }
                set { numParcela = value; }
            }

            private Nullable<bool> antecipado;
            public Nullable<bool> Antecipado
            {
                get { return antecipado; }
                set { antecipado = value; }
            }

            //private Nullable<DateTime> dataRecebimentoOriginal;
            //public Nullable<DateTime> DataRecebimentoOriginal
            //{
            //    get { return dataRecebimentoOriginal; }
            //    set { dataRecebimentoOriginal = value; }
            //}
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


        public class ConciliacaoLote
        {
            public int Lote { get; set; }
            public string Filial { get; set; }
            public DateTime? DtVenda { get; set; }
            public DateTime DtRecebimento { get; set; }
            public string Bandeira { get; set; }
            public decimal Valor { get; set; }
        }


        public class Comparer : IEqualityComparer<ConciliacaoBancaria>
        {
            public bool Equals(ConciliacaoBancaria c1, ConciliacaoBancaria c2)
            {
                if (c1 == c2) return true; // é a mesma instância ou ambos são nulos
                if (c1 == null || c2 == null) return false;
                return c1.Data.Year == c2.Data.Year && c1.Data.Month == c2.Data.Month && c1.Data.Day == c2.Data.Day &&
                       c1.ValorTotal == c2.ValorTotal &&
                       c1.Adquirente.Equals(c2.Adquirente);
            }

            public int GetHashCode(ConciliacaoBancaria c)
            {
                if (c == null) return 0;
                DateTime d = new DateTime(c.Data.Year, c.Data.Month, c.Data.Day);
                return ((int)d.Ticks) + c.Adquirente.GetHashCode() + ((int)c.ValorTotal);
            }
        }

    }
}
