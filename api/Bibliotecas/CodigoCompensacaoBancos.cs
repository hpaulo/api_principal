using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Bibliotecas
{
    public class CodigoCompensacaoBancos
    {
        private string codigo;

        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }
        private string nomeReduzido;

        public string NomeReduzido
        {
            get { return nomeReduzido; }
            set { nomeReduzido = value; }
        }
        private string nomeExtenso;

        public string NomeExtenso
        {
            get { return nomeExtenso; }
            set { nomeExtenso = value; }
        }

    }
}