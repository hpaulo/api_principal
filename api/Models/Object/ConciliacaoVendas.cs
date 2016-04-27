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
        public string Sacado;
        public decimal Valor;
        public Int32 Parcelas;

        public ConciliacaoVendas() { }

        public ConciliacaoVendas(string Tipo, Int32 Id, string Nsu, string CodResumoVendas, DateTime Data,
                                 string Filial, string Adquirente, string Bandeira, string Sacado, decimal Valor, Int32 Parcelas)
        {
            this.Tipo = Tipo;
            this.Id = Id;
            this.Nsu = Nsu;
            this.CodResumoVendas = CodResumoVendas;
            this.Data = Data;
            this.Filial = Filial;
            this.Adquirente = Adquirente;
            this.Bandeira = Bandeira;
            this.Sacado = Sacado;
            this.Valor = Valor;
            this.Parcelas = Parcelas;
        }


        public static bool possuiDivergenciasNaVenda(ConciliacaoVendas recebimento, ConciliacaoVendas venda)
        {
            if (recebimento == null || venda == null)
                return true;

            // Se um for null e o outro não for null, tem divergência
            if (recebimento.Sacado == null ^ venda.Sacado == null)
                return true;

            // Sacado diferente
            if (!recebimento.Sacado.Equals(venda.Sacado))
                return true;

            // Número de parcelas diferente
            if (recebimento.Parcelas != venda.Parcelas)
                return true;

            // Data da venda diferente
            if (!recebimento.Data.ToShortDateString().Equals(venda.Data.ToShortDateString()))
                return true;

            // Valor da venda diferente
            if (recebimento.Valor != venda.Valor)
                return true;

            // NSU diferente
            string nsuR = recebimento.Nsu;
            string nsuV = venda.Nsu;
            while (nsuR.Length < nsuV.Length) nsuR = "0" + nsuR;
            while (nsuV.Length < nsuR.Length) nsuV = "0" + nsuV;
            if (!nsuR.Equals(nsuV))
                return true;

            // Adquirente diferente
            if (!recebimento.Adquirente.Equals(venda.Adquirente))
                return true;

            // Não há divergências
            return false;
        }
    }
}