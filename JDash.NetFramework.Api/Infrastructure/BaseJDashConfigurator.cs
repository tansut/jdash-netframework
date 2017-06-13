using JDash.NetFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace JDash.NetFramework.Api
{
    public abstract class BaseJDashConfigurator
    {
        protected readonly JRequestWrapper Request;
        private bool ensureTablesCreated;
        private static bool _dbCreated = false;

        public BaseJDashConfigurator(JRequestWrapper request)
        {
            this.Request = request;
            this.EnsureTablesCreated = true;
           
        }

        public string AuthorizationHeader { get; private set; }

        protected bool EnsureTablesCreated
        {
            get { return ensureTablesCreated; }
            set { ensureTablesCreated = value; }
        }

        internal JDashPrincipal GetDecryptedPrincipal()
        {
            return this.GetPrincipal();
        }

        /// <summary>
        /// Gets the jdash principal from Authorization Header .
        /// </summary>
        /// <param name="authorizationHeader">The authorization header of http request, this header will be encrypted as JWT that you have encrypted before.</param>
        /// <returns>UnEncrypted User and Application Information</returns>
        public abstract JDashPrincipal GetPrincipal();

        internal IJDashProvider _GetProvider()
        {
            var provider = GetProvider();
            if (EnsureTablesCreated && !_dbCreated)
            {
                provider.EnsureTablesCreated();
                _dbCreated = true;
            }
            return provider;
        }
        public abstract IJDashProvider GetProvider();
    }
}
