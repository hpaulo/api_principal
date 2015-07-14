using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class MSpeer_request
    {
        public int id { get; set; }
        public string publication { get; set; }
        public Nullable<System.DateTime> sent_date { get; set; }
        public string description { get; set; }
    }
}
