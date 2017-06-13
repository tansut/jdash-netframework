using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JDash.NetFramework.Models
{

    /* 
     * 
     * 
        getDashboard?(appid: string, id: string): Promise<GetDashboardResult>;
        searchDashboards(search: ISearchDashboard, query?: Query): Promise<QueryResult<DashboardModel>>;
        createDashboard(model: DashboardModel): Promise<CreateResult>;
        deleteDashboard(appid: string, id: string): Promise<any>;
        updateDashboard(appid: string, id: string, updateValues: DashboardUpdateModel): Promise<any>;
        createDashlet(model: DashletModel): Promise<CreateResult>;
        searchDashlets(search: ISearchDashlet): Promise<Array<DashletModel>>;
        deleteDashlet(id: string | Array<string>): Promise<any>;
        updateDashlet(id: string, updateValues: DashletUpdateModel): Promise<any>;
     * 
     * */


    /// <summary>
    /// This is the persistance layer of JDash, with this Interface you can create your own database layer.
    /// For implementation details you can ask our team.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IJDashProvider : IDisposable
    {
        bool EnsureTablesCreated();
        GetDashboardResult GetDashboard(string appid, string id);

        /// <summary>
        /// Searches the dashboards.
        /// </summary>
        /// <param name="searchDashboardModel">The search dashboard model. This parameter can have ArrayOfString values as JSON Please implement it via testing if parameter starts with [ and ends with ] and contains "," keys </param>
        /// <param name="query">The paging query.</param>
        /// <returns></returns>
        QueryResult<DashboardModel> SearchDashboards(SearchDashboardModel searchDashboardModel, Query query);
        CreateResult CreateDashboard(DashboardModel model);
        void DeleteDashboard(string appid, string id);
        void UpdateDashboard(string appid, string id, DashboardUpdateModel updateModel);
        CreateResult CreateDashlet(DashletModel model);
        IEnumerable<DashletModel> SearchDashlets(SearchDashletModel model);
        void DeleteDashlet(string id);
        void DeleteDashlet(IEnumerable<string> ids);
        void UpdateDashlet(string id, DashletUpdateModel model);
        DashletModel GetDashlet(string id);

        DashboardModel GetDashboardById(string appid, string id);

    }
}
