using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class ContaCorrenteLoginAdquirenteEmpresa
    {
        private Int32 idContaCorrente;
        public Int32 IdContaCorrente
        {
            get { return idContaCorrente; }
            set { idContaCorrente = value; }
        }

        private List<Int32> associar;
        public List<Int32> Associar
        {
            get { return associar; }
            set { associar = value; }
        }

        private List<Int32> desassociar;
        public List<Int32> Desassociar
        {
            get { return desassociar; }
            set { desassociar = value; }
        }

    }

}