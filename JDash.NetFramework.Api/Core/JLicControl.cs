using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Net.Http;

namespace JDash.NetFramework.Api.Core
{
    class JLicControl : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override bool AllowMultiple => false;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var isNotLocal = !(actionContext.Request.RequestUri.Host == "localhost" || actionContext.Request.RequestUri.Host == "127.0.0.1");
            if (isNotLocal)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "Trial Version of JDash Can Only Be Run At \"localhost\" or \"127.0.0.1\" adresses",
                    Content = new StringContent("Trial Version of JDash Can Only Be Run At \"localhost\" or \"127.0.0.1\" adresses", Encoding.UTF8)
                };
            }
        }

    }
}
