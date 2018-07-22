using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioMetrixCore.Model
{
    public class Attn_tblDeviceInfo
    {
        public string DeviceSL { get; set; }

        public int MachineNumber { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public int PeekDataEverySec { get; set; }

        public TimeSpan? OffPeakHourFrom { get; set; }

        public TimeSpan? OffPeakHourTo { get; set; }
        public bool? IsNextDayEndHour { get; set; }

        public bool? IsForStudent { get; set; }
        public bool? IsForTeacher { get; set; }
        //public bool? IsForStuff { get; set; }
        
    }

}
