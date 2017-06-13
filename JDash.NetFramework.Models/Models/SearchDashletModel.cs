using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
    public class SearchDashletModel
    {
        /// <summary>
        /// Can be ArrayOfString  (JSON) or just string
        /// </summary>
        public string user { get; set; }
        public string dashboardId { get; set; }
    }
}
