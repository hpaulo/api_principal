using System;
using System.Collections.Generic;

namespace api.Models
{
    public partial class syspublication
    {
        public string description { get; set; }
        public string name { get; set; }
        public int pubid { get; set; }
        public byte repl_freq { get; set; }
        public byte status { get; set; }
        public byte sync_method { get; set; }
        public byte[] snapshot_jobid { get; set; }
        public bool independent_agent { get; set; }
        public bool immediate_sync { get; set; }
        public bool enabled_for_internet { get; set; }
        public bool allow_push { get; set; }
        public bool allow_pull { get; set; }
        public bool allow_anonymous { get; set; }
        public bool immediate_sync_ready { get; set; }
        public bool allow_sync_tran { get; set; }
        public bool autogen_sync_procs { get; set; }
        public Nullable<int> retention { get; set; }
        public bool allow_queued_tran { get; set; }
        public bool snapshot_in_defaultfolder { get; set; }
        public string alt_snapshot_folder { get; set; }
        public string pre_snapshot_script { get; set; }
        public string post_snapshot_script { get; set; }
        public bool compress_snapshot { get; set; }
        public string ftp_address { get; set; }
        public int ftp_port { get; set; }
        public string ftp_subdirectory { get; set; }
        public string ftp_login { get; set; }
        public string ftp_password { get; set; }
        public bool allow_dts { get; set; }
        public bool allow_subscription_copy { get; set; }
        public Nullable<bool> centralized_conflicts { get; set; }
        public Nullable<int> conflict_retention { get; set; }
        public Nullable<int> conflict_policy { get; set; }
        public Nullable<int> queue_type { get; set; }
        public string ad_guidname { get; set; }
        public int backward_comp_level { get; set; }
        public bool allow_initialize_from_backup { get; set; }
        public byte[] min_autonosync_lsn { get; set; }
        public Nullable<int> replicate_ddl { get; set; }
        public int options { get; set; }
    }
}
