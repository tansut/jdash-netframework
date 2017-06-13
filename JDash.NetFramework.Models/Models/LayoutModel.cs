using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
    public class LayoutModel
    {
        public string module { get; set; }

        [CanBeNull]
        public Dictionary<string, object> config { get; set; }

        public Dictionary<string, DashletOfLayoutModel> dashlets { get; set; }
        
    }
}
