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

            public RecebParcela()
            {
                this.idRecebimento = 0;
                this.numParcela = 0;
            }

            public RecebParcela(Int32 idRecebimento, Int32 numParcela)
            {
                this.idRecebimento = idRecebimento;
                this.numParcela = numParcela;
            }
        }
    }
}
