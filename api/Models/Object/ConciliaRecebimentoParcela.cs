using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models.Object
{
    public class ConciliaRecebimentoParcela
    {
        public Int32 idExtrato;

        /*private DateTime data;
        public DateTime Data
        {
            get { return data; }
            set { data = value; }
        }*/

        public List<RecebParcela> recebimentosParcela;

        public class RecebParcela
        {
            public Int32 idRecebimento;
            public Int32 numParcela;

            public override string ToString()
            {
                return "ID: " + idRecebimento + " Parcela: " + numParcela;
            }
        }

    }

}