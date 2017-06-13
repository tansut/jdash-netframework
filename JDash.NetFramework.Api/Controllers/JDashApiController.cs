using JDash.NetFramework.Api.Core;
using JDash.NetFramework.Api.Models;
using JDash.NetFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;

namespace JDash.NetFramework.Api
{

#if !(REGISTERED_VERSION)
    [JLicControl]
#endif
    public class JDashApiController : ApiController, IJDashController
    {
        private BaseJDashConfigurator _configurator;
        private JDashPrincipal _principal;
        private BaseJDashConfigurator Configurator
        {
            get
            {
                if (_configurator == null)
                {
                    _configurator = (BaseJDashConfigurator)Activator.CreateInstance(NetFramework.Api.Configuration.ConfiguratorType, new object[] { new JRequestWrapper(this.Request, this.User) });
                }
                return _configurator;
            }
        }


        private JDashPrincipal Principal
        {
            get
            {
                if (_principal == null)
                {
                    _principal = Configurator.GetDecryptedPrincipal();
                }
                return _principal;
            }
        }

        
        [HttpGet]
        public IHttpActionResult status()
        {

            return Ok("api works");
        }

        
        [HttpPost]
        public IHttpActionResult DeleteDashlet(string id)
        {

            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {
                var dashlet = persistance.GetDashlet(id);
                if (dashlet == null)
                    return NotFound();

                var dashboard = persistance.GetDashboardById(principal.appid, dashlet.dashboardId);
                if (dashboard == null)
                    return NotFound();

                if (dashboard.user != principal.user && dashboard.appid != principal.appid)
                    return Unauthorized();

                persistance.DeleteDashlet(id);
                return Ok();
            }
        }

        
        [HttpPost]
        public IHttpActionResult CreateDashlet([FromBody] DashletCreateModel model)
        {
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {

                var dashboard = persistance.GetDashboardById(principal.appid, model.dashboardId);
                if (dashboard == null)
                    return NotFound();

                if (dashboard.user != principal.user)
                    return Unauthorized();

                var dashletModel = new DashletModel
                {
                    title = model.title,
                    configuration = model.configuration,
                    description = model.description,
                    createdAt = DateTime.UtcNow,
                    dashboardId = model.dashboardId,
                    moduleId = model.moduleId
                };

                var createResult = persistance.CreateDashlet(dashletModel);
                return Ok(createResult);
            }
        }

        
        [HttpGet]
        public IHttpActionResult MyDashboards()
        {
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {
                var dashboards = persistance.SearchDashboards(new SearchDashboardModel
                {
                    appid = principal.appid,
                    user = principal.user
                }, null);

                return Ok(dashboards);
            }
        }

        
        [HttpPost]
        public IHttpActionResult DeleteDashboard(string id)
        {
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {
                var dashboard = persistance.GetDashboardById(principal.appid, id);
                if (dashboard == null)
                    return NotFound();

                if (dashboard.user != principal.user)
                    return Unauthorized();

                persistance.DeleteDashboard(principal.appid, id);
                return Ok();

            }
        }

        
        [HttpGet]
        public IHttpActionResult GetDashboard(string id)
        {
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {
                var dashboard = persistance.GetDashboard(principal.appid, id);
                if (dashboard == null || dashboard.dashboard == null)
                    return NotFound();

                if (dashboard.dashboard.user != principal.user)
                    return Unauthorized();

                return Ok(dashboard);
            }
        }

        
        [HttpPost]
        public IHttpActionResult CreateDashboard([FromBody]DashboardCreateModel model)
        {
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {
                var newModel = new DashboardModel
                {
                    title = model.title,
                    appid = principal.appid,
                    config = model.config,
                    createdAt = DateTime.UtcNow,
                    description = model.description,
                    layout = model.layout,
                    shareWith = model.shareWith,
                    user = principal.user,
                    id = null
                };

                var result = persistance.CreateDashboard(newModel);
                return Ok(result);
            }
        }


        
        [HttpPost]
        public IHttpActionResult SearchDashboards([FromBody] SearchDashboardsModelWithQuery search)
        {

            // FIXME : Search can be array of strings
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {

                var searchResult = persistance.SearchDashboards(new SearchDashboardModel
                {
                    appid = principal.appid,
                    user = search.search.user,
                    shareWith = search.search.shareWith
                }, search.query);

                return Ok(searchResult);
            }
        }

        
        [HttpPost]
        public IHttpActionResult SaveDashboard([FromUri]string id, [FromBody] DashboardUpdateModel updateValues)
        {
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {

                var dashboardResult = persistance.GetDashboard(principal.appid, id);
                if (dashboardResult == null || dashboardResult.dashboard == null)
                    return NotFound();

                if (dashboardResult.dashboard.user != principal.user)
                    return Unauthorized();

                // we need to remove old dashlets if removed on client when saving dashboard.
                if (updateValues.layout != null && updateValues.layout.dashlets != null)
                {
                    if (dashboardResult.dashlets != null)
                    {
                        var oldDashletIds = dashboardResult.dashlets.Select(d => d.id);
                        var newDashletIds = updateValues.layout.dashlets.Keys.Select(d => d.ToString());
                        var removedDashletIds = oldDashletIds.Except(newDashletIds);
                        persistance.DeleteDashlet(removedDashletIds);
                    }
                }

                persistance.UpdateDashboard(principal.appid, id, updateValues);
                return Ok();

            }
        }

        
        [HttpPost]
        public IHttpActionResult SaveDashlet([FromUri]string id, [FromBody]DashletUpdateModel updateValues)
        {
            var principal = this.Principal;
            using (var persistance = this.Configurator._GetProvider())
            {
                var dashlet = persistance.GetDashlet(id);
                if (dashlet == null)
                    return NotFound();

                var dashboard = persistance.GetDashboardById(principal.appid, dashlet.dashboardId);
                if (dashboard == null)
                    return NotFound();

                if (dashboard.user != principal.user && dashboard.appid != principal.appid)
                    return Unauthorized();

                persistance.UpdateDashlet(id, updateValues);
                return Ok();
            }
        }


    }
}
