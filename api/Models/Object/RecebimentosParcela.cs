using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{
    public class RecebimentosParcela
    {
        public DateTime dtaRecebimentoEfetivo;
        public List<RecebParcela> recebimentosParcela;

        public class RecebParcela
        {
            public Int32 idRecebimento;
            public Int32 numParcela;
        }
    }
}
