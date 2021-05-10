using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Reflection;

namespace RestService
{
	public class RestServiceConfiguration
    {
        /// <summary>
        /// Prefix when generating a new CorrelationId (When it does not exist as a header on the <see cref="IHttpContextAccessor"/>) Typically the Apps name
        /// Defaults to Assembly.GetCallingAssembly().GetName().Name
        /// </summary>
        public string CorrelationIdPrefix { get; set; } = Assembly.GetCallingAssembly().GetName().Name;

        /// <summary>
        /// Name used to register global HttpClient from <see cref="IHttpClientFactory.CreateClient(string)"/>.
        /// </summary>
        public string DefaultHttpClientName { get; set; }

        /// <summary>
        /// If forwarding and or adding of Correlation headers should be enabled (X-Correlation-Id) and (X-Correlation-Index)
        /// These values are automatically ingested from <see cref="IHttpContextAccessor"/>
        /// services.AddHttpContextAccessor(); should in Startup.cs
        /// </summary>
        public bool EnableCorrelationId { get; set; } = false;
    }
}
