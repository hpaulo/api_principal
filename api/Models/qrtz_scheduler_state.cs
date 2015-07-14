using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class qrtz_scheduler_state
    {
        public string instance_name { get; set; }
        public decimal last_checkin_time { get; set; }
        public decimal checkin_interval { get; set; }
    }
}
