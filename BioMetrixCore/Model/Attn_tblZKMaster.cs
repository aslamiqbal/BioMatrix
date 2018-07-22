using IqbalCoreLibrary.DB.Persistent.PersistentsAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioMetrixCore.Model
{
  public   class Attn_tblZKMaster
    {
       
        [Identity]
        public long Id { get; set; }
        public string DeviceID { get; set; }
        public string SerialNumber { get; set; }
        public string UserID { get; set; }
        public DateTime LogTime { get; set; }
        public bool? IsSent { get; set; }

    }
}
