using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvizhSeller_Console.services.actions
{
    public class Setting
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public SettingParams _params { get; set; }
    }

    public class SettingParams
    {
        public int com_port { get; set; }
        public int com_speed { get; set; }
        public bool use_remote { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public int model { get; set; }
        public Purchase_settings purchase_Settings { get; set; }
    }

    public class Purchase_settings
    {
        public bool cashierSign { get; set; }
        public bool buyerSign { get; set; }
        public int indent { get; set; }
        public string comment { get; set; }
    }
}
