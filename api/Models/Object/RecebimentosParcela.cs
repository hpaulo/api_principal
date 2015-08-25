using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Object
{
    public class RecebimentosParcela
    {
        private DateTime dtaRecebimento;
        public DateTime DtaRecebimento
        {
            get { return dtaRecebimento; }
            set { dtaRecebimento = value; }
        }

        private List<Int32> idsRecebimento;
        public List<Int32> IdsRecebimento
        {
            get { return idsRecebimento; }
            set { idsRecebimento = value; }
        }
    }
}
