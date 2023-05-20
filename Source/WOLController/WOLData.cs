using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bass.Util.WOL
{
    public class WOLData
    {
        public string MachineName { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;


        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(MachineName))
                return false;

            if(!WOLSender.IsValidMacAddress(MacAddress)) 
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"[{MachineName}] {MacAddress} ";
        }

    }
}
