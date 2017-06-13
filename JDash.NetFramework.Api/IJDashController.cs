using JDash.NetFramework.Api.Models;
using JDash.NetFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace JDash.NetFramework.Api
{
    public interface IJDashController
    {
        IHttpActionResult MyDashboards();
        IHttpActionResult GetDashboard(string id);
        IHttpActionResult CreateDashboard(DashboardCreateModel model);
        IHttpActionResult SearchDashboards(SearchDashboardsModelWithQuery search);
        IHttpActionResult DeleteDashboard(string id);
        IHttpActionResult SaveDashboard(string id, DashboardUpdateModel updateValues);
        IHttpActionResult CreateDashlet(DashletCreateModel model);
        IHttpActionResult DeleteDashlet(string id);
        IHttpActionResult SaveDashlet(string id, DashletUpdateModel updateValues);
    }
}
