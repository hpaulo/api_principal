using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class atualizacao_log
    {
        public System.DateTime data_atualizacao { get; set; }
        public string fonte_atualizacao { get; set; }
        public string msg_atualizacao { get; set; }
    }
}
