using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDash.NetFramework.Api.Core
{
    class JRouteAttribute : Attribute
    {
        public JRouteAttribute(string template)
        {
            this.Template = template;
        }

        public string Template { get; private set; }
    }
}
