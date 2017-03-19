using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mm
{
    interface IAppSettings
    {
        Dictionary<string, string> ConnectionStrings{ get; set; }
    }

    public class AppSettings
    {
        public Dictionary<string,string> ConnectionStrings { get; set; }
    }
}
