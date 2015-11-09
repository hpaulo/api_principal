using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class ConciliacaoTitulos
    {
        private string tipo; // "T" : tbRecebimentoTitulo, "R" : RecebimentoParcela
        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        private Int32 id;
        public Int32 Id
        {
            get { return id; }
            set { id = value; }
        }

        private Int32 numParcela;
        public Int32 NumParcela
        {
            get { return numParcela; }
            set { numParcela = value; }
        }

        private string nsu;
        public string Nsu
        {
            get { return nsu; }
            set { nsu = value; }
        }

        private string codResumoVendas;
        public string CodResumoVendas
        {
            get { return codResumoVendas; }
            set { codResumoVendas = value; }
        }

        private DateTime data; // data do título ou do recebimento
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

        private string filial;
        public string Filial
        {
            get { return filial; }
            set { filial = value; }
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

        private decimal valor;
        public decimal Valor
        {
            get { return valor; }
            set { valor = value; }
        }
    }
}