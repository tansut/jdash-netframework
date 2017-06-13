using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{
   
    public interface IPrincipalResolver
    {
        void SetPrincipal(JDashPrincipal principal);
        JDashPrincipal GetPrincipal();
    }
}
