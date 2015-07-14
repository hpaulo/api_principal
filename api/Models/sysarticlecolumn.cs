using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class sysarticlecolumn
    {
        public int artid { get; set; }
        public short colid { get; set; }
        public Nullable<bool> is_udt { get; set; }
        public Nullable<bool> is_xml { get; set; }
        public Nullable<bool> is_max { get; set; }
    }
}
