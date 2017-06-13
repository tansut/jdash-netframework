using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{

    public class SearchDashboardModel
    {
        /// <summary>
        /// Can be ArrayOfString (JSON) or just string , check if this is an array via the string can be converted to a string[] via a JSON converter.
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// Can be ArrayOfString  (JSON) or just string, check if this is an array via the string can be converted to a string[] via a JSON converter.
        /// </summary>
        public string user { get; set; }

        /// <summary>
        /// Can be ArrayOfString  (JSON) or just string, check if this is an array via the string can be converted to a string[] via a JSON converter.
        /// </summary>
        public string shareWith { get; set; }
    }
}
