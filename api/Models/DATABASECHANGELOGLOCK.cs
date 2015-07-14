using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class DATABASECHANGELOGLOCK
    {
        public int ID { get; set; }
        public bool LOCKED { get; set; }
        public Nullable<System.DateTime> LOCKGRANTED { get; set; }
        public string LOCKEDBY { get; set; }
    }
}
