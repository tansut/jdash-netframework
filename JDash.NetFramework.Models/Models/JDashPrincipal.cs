using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
    public class JDashPrincipal
    {

        public JDashPrincipal(string user)
        {
            this.user = user;
            this.appid = "defaultapp";
        }

        public JDashPrincipal(string user, string appid)
        {
            this.appid = appid;
            this.user = user;
        }

        public string appid { get; set; }
        public string user { get; set; }
    }
}
