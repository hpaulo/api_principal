using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class MSpeer_response
    {
        public Nullable<int> request_id { get; set; }
        public string peer { get; set; }
        public string peer_db { get; set; }
        public Nullable<System.DateTime> received_date { get; set; }
    }
}
