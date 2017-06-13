using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace JDash.NetFramework.Api
{
    public class JRequestWrapper
    {
        public JRequestWrapper(HttpRequestMessage request, IPrincipal user)
        {
            this.Request = request;
            this.User = user;
        }
        public HttpRequestMessage Request { get; set; }
        public IPrincipal User { get; set; }
    }
}
