using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RestService
{
	public class RestActionOptions
	{
		/// <summary>
		/// JsonSerializerSettings Overide 
		/// </summary>
		public JsonSerializerSettings SerializerSettings { get; set; }

		/// <summary>
		/// Targets an HttpClient registered with <see cref="IHttpClientFactory"/> it overides the <see cref="RestServiceConfiguration.DefaultHttpClientName"/> 
		/// </summary>
		public string HttpClientName { get; set; }

		/// <summary>
		/// Takes preference as the CorrelationId used
		/// </summary>
		public string CorrelationId { get; set; }

		/// <summary>
		/// Status Codes other than the 200 range that should not throw exceptions
		/// </summary>
		public HttpStatusCode[] ValidStatusCodes { get; set; } = new HttpStatusCode[] { };

		/// <summary>
		/// Additional Headers to Add (If not added by the HTTP Client)
		/// </summary>
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Authorization Header (If not added by the HTTP Client)
		/// </summary>
		public AuthenticationHeaderValue AuthorizationHeader { get; set; }

		/// <summary>
		/// Status Codes other than the 200 range that should not throw exceptions
		/// </summary>
		public RestActionOptions SetValidStatusCodes(params HttpStatusCode[] httpStatusCodes)
		{
			ValidStatusCodes = httpStatusCodes;
			return this;
		}
	}
}
