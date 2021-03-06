using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
using JDash.NetFramework.Models;
using JDash.NetFramework.Provider.MsSQL.Scripts;

namespace JDash.NetFramework.Provider.MsSQL
{
    public class JSQLProvider : IJDashProvider
    {
        private string connStr;
        private string defaultScheme;

        public JSQLProvider(string connectionString)
        {
            this.defaultScheme = "dbo";
            this.connStr = connectionString;
        }

        public JSQLProvider(string connectionString, string defaultScheme)
        {
            this.connStr = connectionString;
            this.defaultScheme = defaultScheme;
        }

        public virtual bool EnsureTablesCreated()
        {
            using (var connection = CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT count(*) FROM INFORMATION_SCHEMA.TABLES where (TABLE_NAME = 'dashboard' or TABLE_NAME = 'dashlet' ) and TABLE_SCHEMA = @scheme";
                command.Parameters.AddWithValue("@scheme", this.defaultScheme);
                try
                {
                    connection.Open();
                    if (Convert.ToInt32(command.ExecuteScalar().ToString()) != 2)
                    {
                        using (var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                        {
                            try
                            {
                                var dashboardCreateCommand = connection.CreateCommand();
                                dashboardCreateCommand.CommandText = string.Format(SQLScripts.DashboardTableCreationScript, this.defaultScheme);
                                dashboardCreateCommand.CommandType = System.Data.CommandType.Text;
                                dashboardCreateCommand.Transaction = transaction;
                                dashboardCreateCommand.ExecuteNonQuery();

                                var dashletCreateCommand = connection.CreateCommand();
                                dashletCreateCommand.CommandText = string.Format(SQLScripts.DashletTableCreationScript, this.defaultScheme);
                                dashletCreateCommand.CommandType = System.Data.CommandType.Text;
                                dashletCreateCommand.Transaction = transaction;
                                dashletCreateCommand.ExecuteNonQuery();

                                transaction.Commit();
                                return true;
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw new Exception("Dashlet/Dashboard Table Creation Failed on SQL Server Please Check If Database Created and Dashboard and Dashlet tables do not exists.", ex);
                            }

                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                finally
                {
                    connection.Close();
                }

            }

        }

        public virtual DashletModel GetDashlet(string id)
        {
            using (var connection = CreateConnection())
            {
                var query = getSelectDashletQueryBase();
                StringBuilder sb = new StringBuilder(query);
                sb.Append(" where id = @id");

                var command = connection.CreateCommand();
                command.CommandText = sb.ToString();
                command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(id);
                command.CommandType = System.Data.CommandType.Text;

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    return CastToDashletModel(reader).FirstOrDefault();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual DashboardModel GetDashboardById(string appid, string id)
        {
            using (var connection = CreateConnection())
            {
                var query = getSelectDashboardQueryBase();
                StringBuilder sb = new StringBuilder(query);
                sb.Append(" where id = @id and appId = @appid ");

                var command = connection.CreateCommand();
                command.CommandText = sb.ToString();
                command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(id);
                command.Parameters.Add("@appid", System.Data.SqlDbType.NVarChar).Value = appid;
                command.CommandType = System.Data.CommandType.Text;
                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    return CastToDashboardModel(reader).FirstOrDefault();
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public virtual CreateResult CreateDashboard(DashboardModel model)
        {
            string statement = @"INSERT INTO [" + this.defaultScheme + @"].[dashboard]
                                       ([appId]
                                       ,[title]
                                       ,[shareWith]
                                       ,[description]
                                       ,[user]
                                       ,[createdAt]
                                       ,[config]
                                       ,[layout])
                                 OUTPUT Inserted.id
                                 VALUES
                                (@appid , @title , @shareWith , @description , @user , @createdAt , @config , @layout)";

            using (var connection = CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = statement;
                command.Parameters.Add("@appid", System.Data.SqlDbType.VarChar).Value = model.appid ?? (object)DBNull.Value;
                command.Parameters.Add("@title", System.Data.SqlDbType.NVarChar).Value = model.title ?? "";
                command.Parameters.Add("@shareWith", System.Data.SqlDbType.VarChar).Value = model.shareWith ?? "";
                command.Parameters.Add("@description", System.Data.SqlDbType.VarChar).Value = model.description ?? "";
                command.Parameters.Add("@user", System.Data.SqlDbType.VarChar).Value = model.user ?? (object)DBNull.Value;
                command.Parameters.Add("@createdAt", System.Data.SqlDbType.DateTime).Value = model.createdAt;

                if (model.config != null)
                {
                    command.Parameters.Add("@config", System.Data.SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(model.config);
                }
                else
                {
                    command.Parameters.Add("@config", System.Data.SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(new Dictionary<string, object>());
                }

                if (model.layout != null)
                {
                    command.Parameters.Add("@layout", System.Data.SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(model.layout);
                }
                else
                {
                    command.Parameters.Add("@layout", System.Data.SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(new LayoutModel() { });
                }

                try
                {
                    connection.Open();
                    var id = command.ExecuteScalar().ToString();
                    return new CreateResult() { id = id };
                }
                finally
                {
                    connection.Close();
                }


            }
        }

        public virtual CreateResult CreateDashlet(DashletModel model)
        {
            string statement = @"INSERT INTO [" + this.defaultScheme + @"].[dashlet]
           ([moduleId]
           ,[dashboardId]
           ,[configuration]
           ,[title]
           ,[description]
           ,[createdAt])
     OUTPUT Inserted.id
     VALUES
           (@moduleId, @dashboardId , @configuration, @title, @description, @createdAt)";

            using (var connection = CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = statement;
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.Add("@moduleId", System.Data.SqlDbType.NVarChar).Value = model.moduleId ?? (object)DBNull.Value;
                command.Parameters.Add("@dashboardId", System.Data.SqlDbType.Int).Value = model.dashboardId ?? (object)DBNull.Value;
                command.Parameters.Add("@title", System.Data.SqlDbType.NVarChar).Value = model.title ?? "";
                command.Parameters.Add("@description", System.Data.SqlDbType.NVarChar).Value = model.description ?? "";
                command.Parameters.Add("@createdAt", System.Data.SqlDbType.DateTime).Value = model.createdAt;

                if (model.configuration != null)
                {
                    command.Parameters.Add("@configuration", System.Data.SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(model.configuration);
                }
                else
                {
                    command.Parameters.Add("@configuration", System.Data.SqlDbType.NVarChar).Value = JsonConvert.SerializeObject(new Dictionary<string, object>());
                }

                try
                {
                    connection.Open();
                    var id = command.ExecuteScalar().ToString();
                    return new CreateResult() { id = id };
                }
                finally
                {
                    connection.Close();
                }

            }

        }

        public virtual void DeleteDashboard(string appid, string id)
        {
            using (var connection = CreateConnection())
            {
                string statement = "delete from [" + this.defaultScheme + "].dashboard where appId = @appid and id = @id";
                var command = connection.CreateCommand();
                command.CommandText = statement;
                command.CommandType = System.Data.CommandType.Text;

                command.Parameters.Add("@appid", System.Data.SqlDbType.VarChar).Value = appid;
                command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(id);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual void DeleteDashlet(IEnumerable<string> ids)
        {
            if (!ids.Any())
                return;

            using (var connection = CreateConnection())
            {
                var idParameterNames = ids.Select((item, index) => { return "@id" + index; });
                var namedParameters = string.Join(",", idParameterNames);

                string statement = "delete from [" + this.defaultScheme + "].dashlet where id in (" + namedParameters + ")";
                var command = connection.CreateCommand();
                command.CommandText = statement;
                command.CommandType = System.Data.CommandType.Text;

                for (int i = 0; i < idParameterNames.Count(); i++)
                {
                    var idParameter = idParameterNames.ElementAt(i);
                    var parameterValue = ids.ElementAt(i);
                    command.Parameters.Add(idParameter, System.Data.SqlDbType.Int).Value = int.Parse(parameterValue);
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual void DeleteDashlet(string id)
        {
            using (var connection = CreateConnection())
            {
                string statement = "delete from [" + this.defaultScheme + "].dashlet where id = @id";
                var command = connection.CreateCommand();
                command.CommandText = statement;
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(id);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual GetDashboardResult GetDashboard(string appid, string id)
        {
            var dashboard = GetDashboardById(appid, id);
            var dashletsOfDashboard = GetDashletsOfDashboard(id);
            return new GetDashboardResult() { dashboard = dashboard, dashlets = dashletsOfDashboard };
        }

        public virtual IEnumerable<DashletModel> SearchDashlets(SearchDashletModel model)
        {
            using (var connection = CreateConnection())
            {
                var query = getSelectDashletQueryBase();
                StringBuilder sb = new StringBuilder(query);
                sb.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(model.dashboardId))
                    sb.Append(" and dashboardId = @id");
                if (!string.IsNullOrEmpty(model.user))
                    sb.Append(" and [user] = @user");

                var command = connection.CreateCommand();
                command.CommandText = sb.ToString();
                command.CommandType = System.Data.CommandType.Text;

                if (!string.IsNullOrEmpty(model.dashboardId))
                    command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(model.dashboardId);
                if (!string.IsNullOrEmpty(model.user))
                    command.Parameters.Add("@user", System.Data.SqlDbType.NVarChar).Value = model.user;


                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    return CastToDashletModel(reader);
                }
                finally
                {
                    connection.Close();
                }

            }

        }

        public virtual void UpdateDashlet(string id, DashletUpdateModel model)
        {
            string statement = "update [" + this.defaultScheme + "].dashlet set {0} where id = @id";
            using (var connection = CreateConnection())
            {
                Dictionary<string, KeyValuePair<string, object>> keyValues = new Dictionary<string, KeyValuePair<string, object>>();
                if (model.configuration != null)
                    keyValues.Add("[configuration]", new KeyValuePair<string, object>("@configuration", JsonConvert.SerializeObject(model.configuration)));

                if (model.description != null)
                    keyValues.Add("[description]", new KeyValuePair<string, object>("@description", model.description));

                if (model.title != null)
                    keyValues.Add("[title]", new KeyValuePair<string, object>("@title", model.title));


                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(id);

                string setValues = "";

                foreach (var key in keyValues)
                {
                    setValues += " " + key.Key + " = " + key.Value.Key + ",";
                    if (key.Value.Value == null)
                        command.Parameters.Add(key.Value.Key, System.Data.SqlDbType.NVarChar).Value = DBNull.Value;
                    else
                        command.Parameters.Add(key.Value.Key, System.Data.SqlDbType.NVarChar).Value = key.Value.Value;
                }
                setValues = setValues.TrimEnd(",".ToCharArray());

                command.CommandText = string.Format(statement, setValues);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public virtual void UpdateDashboard(string appid, string id, DashboardUpdateModel updateModel)
        {
            string statement = "update [" + this.defaultScheme + "].dashboard set {0} where id = @id and appId = @appid ";
            using (var connection = CreateConnection())
            {
                Dictionary<string, KeyValuePair<string, object>> keyValues = new Dictionary<string, KeyValuePair<string, object>>();

                if (updateModel.config != null)
                    keyValues.Add("[config]", new KeyValuePair<string, object>("@config", JsonConvert.SerializeObject(updateModel.config)));

                if (updateModel.layout != null)
                    keyValues.Add("[layout]", new KeyValuePair<string, object>("@layout", JsonConvert.SerializeObject(updateModel.layout)));

                if (updateModel.description != null)
                    keyValues.Add("[description]", new KeyValuePair<string, object>("@description", updateModel.description));

                if (updateModel.title != null)
                    keyValues.Add("[title]", new KeyValuePair<string, object>("@title", updateModel.title));

                if (updateModel.shareWith != null)
                    keyValues.Add("[shareWith]", new KeyValuePair<string, object>("@shareWith", updateModel.shareWith));


                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(id);
                command.Parameters.Add("@appid", System.Data.SqlDbType.VarChar).Value = appid;

                string setValues = "";

                foreach (var key in keyValues)
                {
                    setValues += " " + key.Key + " = " + key.Value.Key + ",";
                    if (key.Value.Value == null)
                        command.Parameters.Add(key.Value.Key, System.Data.SqlDbType.NVarChar).Value = DBNull.Value;
                    else
                        command.Parameters.Add(key.Value.Key, System.Data.SqlDbType.NVarChar).Value = key.Value.Value;
                }
                setValues = setValues.TrimEnd(",".ToCharArray());

                command.CommandText = string.Format(statement, setValues);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }
        }



        public virtual QueryResult<DashboardModel> SearchDashboards(SearchDashboardModel searchDashboardModel, Query query)
        {
            if (query == null)
            {
                query = new Query()
                {
                    limit = 100,
                    startFrom = 0
                };
            }

            if (query.limit == 0)
                query.limit = 100;

            using (var connection = CreateConnection())
            {

                bool isUserArray = false;
                bool isAppIdArray = false;
                bool isShareWithArray = false;

                if (!string.IsNullOrEmpty(searchDashboardModel.user))
                    isUserArray = searchDashboardModel.user.StartsWith("[") && searchDashboardModel.user.EndsWith("]");

                if (!string.IsNullOrEmpty(searchDashboardModel.appid))
                    isAppIdArray = searchDashboardModel.appid.StartsWith("[") && searchDashboardModel.appid.EndsWith("]");

                if (!string.IsNullOrEmpty(searchDashboardModel.shareWith))
                    isShareWithArray = searchDashboardModel.shareWith.StartsWith("[") && searchDashboardModel.shareWith.EndsWith("]");


                string statement = getSelectDashboardQueryBase();
                StringBuilder queryBuilder = new StringBuilder(statement);
                queryBuilder.Append(" where 1 = 1 ");

                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;

                if (!string.IsNullOrEmpty(searchDashboardModel.user))
                    AddMultiParameter(queryBuilder, command, "user", searchDashboardModel.user, isUserArray, System.Data.SqlDbType.NVarChar);

                if (!string.IsNullOrEmpty(searchDashboardModel.appid))
                    AddMultiParameter(queryBuilder, command, "appId", searchDashboardModel.appid, isAppIdArray, System.Data.SqlDbType.VarChar);

                if (!string.IsNullOrEmpty(searchDashboardModel.shareWith))
                    AddMultiParameter(queryBuilder, command, "shareWith", searchDashboardModel.shareWith, isShareWithArray, System.Data.SqlDbType.NVarChar);

                // +1 is for "hasMore" result.
                queryBuilder.Append(" order by [createdAt] offset " + (query.startFrom) + " rows fetch next " + (query.limit + 1) + " rows only");

                command.CommandText = queryBuilder.ToString();

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    var dashboardModels = CastToDashboardModel(reader);
                    var result = new QueryResult<DashboardModel>();
                    result.data = dashboardModels.Take(query.limit);
                    result.hasMore = dashboardModels.Count() > query.limit;
                    return result;
                }
                finally
                {
                    connection.Close();
                }


            }
        }

        private IEnumerable<DashletModel> GetDashletsOfDashboard(string id, string userId = null)
        {
            using (var connection = CreateConnection())
            {
                var query = getSelectDashletQueryBase();
                StringBuilder sb = new StringBuilder(query);
                sb.Append(" where dashboardId = @id");

                var command = connection.CreateCommand();
                command.CommandText = sb.ToString();
                command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = int.Parse(id);
                command.CommandType = System.Data.CommandType.Text;

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    return CastToDashletModel(reader);
                }
                finally
                {
                    connection.Close();
                }


            }
        }

        private void AddMultiParameter(StringBuilder queryBuilder, SqlCommand command, string key, string value, bool isMultiple, System.Data.SqlDbType type)
        {
            // this method should add multiple keys for "in" clause ,if not defined multiple it will just add key,value as parameters.
            if (isMultiple)
            {

                queryBuilder.Append(" and [" + key + "] in (");
                var valueCollection = JsonConvert.DeserializeObject<string[]>(value);
                for (int i = 0; i < valueCollection.Length; i++)
                {
                    if (i == valueCollection.Length - 1)
                    {
                        queryBuilder.Append("@" + key + i);
                        command.Parameters.Add("@" + key + i, type).Value = valueCollection[i];
                    }
                    else
                    {
                        queryBuilder.Append("@" + key + i + " , ");
                        command.Parameters.Add("@" + key + i, type).Value = valueCollection[i];
                    }
                }
                queryBuilder.Append(" )");
            }
            else
            {

                queryBuilder.Append(" and [" + key + "] = @" + key + " ");
                command.Parameters.Add("@" + key, type).Value = value;
            }
        }

        private SqlConnection CreateConnection()
        {
            SqlConnection connection = new SqlConnection(this.connStr);
            return connection;
        }


        private string getSelectDashletQueryBase()
        {
            string query = "SELECT [id], [moduleId], [dashboardId], [configuration], [title], [description], [createdAt] FROM  [" + this.defaultScheme + "].[dashlet] ";
            return query;
        }

        private string getSelectDashboardQueryBase(int top = 0)
        {
            StringBuilder query = new StringBuilder("SELECT [id], [appId], [title], [shareWith], [description], [user], [createdAt], [config] , [layout]  FROM  [");
            query.Append(this.defaultScheme);
            query.Append("].[dashboard] ");
            return query.ToString();
        }

        private IEnumerable<DashboardModel> CastToDashboardModel(SqlDataReader reader)
        {
            List<DashboardModel> dashboards = new List<DashboardModel>();
            while (reader.Read())
            {
                var dashboardModel = new DashboardModel
                {
                    id = reader.GetInt32(0).ToString(),
                    appid = reader.GetString(1),
                    title = reader.GetString(2),
                    shareWith = reader.GetString(3),
                    description = reader.GetString(4),
                    user = reader.GetString(5),
                    createdAt = reader.GetDateTime(6),

                    // 7th and 8th index are json objects will handle after. 
                };

                var configurationJson = reader.GetString(7);
                if (!string.IsNullOrEmpty(configurationJson))
                {
                    dashboardModel.config = JsonConvert.DeserializeObject<Dictionary<string, object>>(configurationJson);
                }

                var layoutJson = reader.GetString(8);
                if (!string.IsNullOrEmpty(layoutJson))
                {
                    dashboardModel.layout = JsonConvert.DeserializeObject<LayoutModel>(layoutJson);
                }

                dashboards.Add(dashboardModel);
            }

            return dashboards;
        }

        private IEnumerable<DashletModel> CastToDashletModel(SqlDataReader reader)
        {
            List<DashletModel> dashlets = new List<DashletModel>();
            while (reader.Read())
            {
                //  [id], [moduleId], [dashboardId], [configuration], [title], [description], [createdAt] 
                var dashletModel = new DashletModel
                {
                    id = reader.GetInt32(0).ToString(),
                    moduleId = reader.GetString(1),
                    dashboardId = reader.GetInt32(2).ToString(),
                    // 3rd index is a json object will handle after.
                    title = reader.GetString(4),
                    description = reader.GetString(5),
                    createdAt = reader.GetDateTime(6)
                };

                var configurationJson = reader.GetString(3);
                if (!string.IsNullOrEmpty(configurationJson))
                {
                    dashletModel.configuration = JsonConvert.DeserializeObject<Dictionary<string, object>>(configurationJson);
                }

                dashlets.Add(dashletModel);
            }

            return dashlets;
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JSQLProvider() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
