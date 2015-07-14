using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class MSpub_identity_range
    {
        public int objid { get; set; }
        public long range { get; set; }
        public long pub_range { get; set; }
        public long current_pub_range { get; set; }
        public int threshold { get; set; }
        public Nullable<long> last_seed { get; set; }
    }
}
