using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
    public class DashletModel
    {
        public string moduleId { get; set; }
        public string dashboardId { get; set; }

        [CanBeNull]
        public string id { get; set; }
        [CanBeNull]
        public string title { get; set; }
        [CanBeNull]
        public string description { get; set; }
        public Dictionary<string, object> configuration { get; set; }
        public DateTime? createdAt { get; set; }
    }
}
