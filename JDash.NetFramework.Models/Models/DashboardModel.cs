using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
    public class DashboardModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string appid { get; set; }
        public string user { get; set; }
        public string description { get; set; }
        public string shareWith { get; set; }
        public LayoutModel layout { get; set; }
        public DateTime? createdAt { get; set; }
        public Dictionary<string, object> config { get; set; }
    }
}
