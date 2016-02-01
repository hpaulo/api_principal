using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{
    public class ParametroBancario
    {
        private string cdBanco;
        public string CdBanco
        {
            get { return cdBanco; }
            set { cdBanco = value; }
        }
        private string dsMemo;
        public string DsMemo
        {
            get { return dsMemo; }
            set { dsMemo = value; }
        }
        private string dsTipo;
        public string DsTipo
        {
            get { return dsTipo; }
            set { dsTipo = value; }
        }
    }

    public class ParametrosBancarios
    {
        private Nullable<int> cdAdquirente;
        public Nullable<int> CdAdquirente
        {
            get { return cdAdquirente; }
            set { cdAdquirente = value; }
        }

        private string nrCnpj;
        public string NrCnpj
        {
            get { return nrCnpj; }
            set { nrCnpj = value; }
        }

        private string dsTipoCartao;
        public string DsTipoCartao
        {
            get { return dsTipoCartao; }
            set { dsTipoCartao = value; }
        }

        private Nullable<int> cdBandeira;
        public Nullable<int> CdBandeira
        {
            get { return cdBandeira; }
            set { cdBandeira = value; }
        }

        private bool flVisivel;
        public bool FlVisivel
        {
            get { return flVisivel; }
            set { flVisivel = value; }
        }

        private Nullable<bool> flAntecipacao;
        public Nullable<bool> FlAntecipacao
        {
            get { return flAntecipacao; }
            set { flAntecipacao = value; }
        }

        private bool deletar;
        public bool Deletar
        {
            get { return deletar; }
            set { deletar = value; }
        }

        private List<ParametroBancario> parametros;
        public List<ParametroBancario> Parametros
        {
            get { return parametros; }
            set { parametros = value; }
        }
    }
}
