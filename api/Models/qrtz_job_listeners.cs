using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class qrtz_job_listeners
    {
        public string job_name { get; set; }
        public string job_group { get; set; }
        public string job_listener { get; set; }
        public virtual qrtz_job_details qrtz_job_details { get; set; }
    }
}
