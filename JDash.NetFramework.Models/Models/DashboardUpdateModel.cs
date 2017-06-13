using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
    public class DashboardUpdateModel
    {
        [CanBeNull]
        public string title { get; set; }

        [CanBeNull]
        public string description { get; set; }

        [CanBeNull]
        public string shareWith { get; set; }

        [CanBeNull]
        public LayoutModel layout { get; set; }

        [CanBeNull]
        public Dictionary<string,object> config { get; set; }

    }
}
