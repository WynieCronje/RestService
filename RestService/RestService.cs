using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestService
{
	public class RestService : IRestService
	{
		private static JsonSerializerSettings defaultSettings = new JsonSerializerSettings();

		private readonly Type stringType = typeof(string);
		private readonly Type streamType = typeof(Stream);
		private readonly Type byteArrayType = typeof(byte[]);

		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IHttpClientFactory clientFactory;
		private readonly RestServiceConfiguration config;

		public RestService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IOptions<RestServiceConfiguration> config)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.clientFactory = clientFactory;
			this.config = config.Value;
		}

		public async Task<TResponse> GetAsync<TResponse>(Uri apiAction, RestActionOptions options)
		{
			return await GetApiResponse<TResponse>(apiAction, HttpMethod.Get, options);
		}

		public async Task<TResponse> DeleteAsync<TResponse>(Uri apiAction, RestActionOptions options)
		{
			return await GetApiResponse<TResponse>(apiAction, HttpMethod.Delete, options);
		}

		public async Task<TResponse> PostAsync<TResponse>(Uri apiAction, object requestBody, RestActionOptions options)
		{
			return await GetApiResponse<TResponse, object>(apiAction, HttpMethod.Post, requestBody, options);
		}

		public async Task<TResponse> PostAsync<TResponse, TRequest>(Uri apiAction, TRequest requestBody, RestActionOptions options = null)
		{
			return await GetApiResponse<TResponse, TRequest>(apiAction, HttpMethod.Post, requestBody, options);
		}

		public async Task<TResponse> PutAsync<TResponse>(Uri apiAction, object requestBody, RestActionOptions options)
		{
			return await GetApiResponse<TResponse, object>(apiAction, HttpMethod.Put, requestBody, options);
		}

		public async Task<TResponse> PutAsync<TResponse, TRequest>(Uri apiAction, TRequest requestBody, RestActionOptions options = null)
		{
			return await GetApiResponse<TResponse, TRequest>(apiAction, HttpMethod.Put, requestBody, options);
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, RestActionOptions actionOptions)
		{
			var options = actionOptions ?? new RestActionOptions();

			var httpClient = ResolveHttpClient(options);

			ApplyHeaderSettings(httpRequest, actionOptions);

			return await httpClient.SendAsync(httpRequest);
		}

		#region private methods

		private async Task<TResponse> GetApiResponse<TResponse>(Uri apiAction, HttpMethod method, RestActionOptions actionOptions)
		{
			return await GetApiResponse<TResponse, object>(apiAction, method, null, actionOptions);
		}

		private async Task<TResponse> GetApiResponse<TResponse, TRequest>(Uri apiAction, HttpMethod method, TRequest requestBody, RestActionOptions actionOptions)
		{
			var options = actionOptions ?? new RestActionOptions();

			var httpClient = ResolveHttpClient(options);

			using (var httpRequest = new HttpRequestMessage(method, apiAction))
			{
				ApplyHeaderSettings(httpRequest, actionOptions);

				if (requestBody != null)
				{
					httpRequest.Content = GetStringContent(requestBody, options?.SerializerSettings);
				}

				using (var response = await httpClient.SendAsync(httpRequest))
				{
					return await GetResponse<TResponse>(response, options, requestBody);
				}
			}
		}

		private async Task<TResponse> GetResponse<TResponse>(HttpResponseMessage response, RestActionOptions options, object requestData)
		{
			EnsureResponse(response, options, requestData);

			Type typeParameterType = typeof(TResponse);

			if (typeParameterType == byteArrayType)
			{
				return (TResponse)Convert.ChangeType(await response.Content.ReadAsByteArrayAsync(), typeof(TResponse));
			}

			if (typeParameterType == streamType)
			{
				return (TResponse)Convert.ChangeType(await response.Content.ReadAsStreamAsync(), typeof(TResponse));
			}

			if (typeParameterType == stringType)
			{
				return (TResponse)Convert.ChangeType(await response.Content.ReadAsStringAsync(), typeof(TResponse));
			}

			var responseStream = await response.Content.ReadAsStreamAsync();

			using (var streamReader = new StreamReader(responseStream))
			{
				var serializer = JsonSerializer.CreateDefault(options.SerializerSettings ?? defaultSettings);
				var result = (TResponse)serializer.Deserialize(streamReader, typeof(TResponse));
				return result;
			}
		}

		private HttpResponseMessage EnsureResponse(HttpResponseMessage response, RestActionOptions options, object requestData)
		{
			if (!response.IsSuccessStatusCode)
			{
				if (options.ValidStatusCodes.Any(x => x == response.StatusCode))
				{
					return response;
				}

				throw new RestResponseException(response);
			}

			return response;
		}

		private StringContent GetStringContent<K>(K value, JsonSerializerSettings serializerSettings)
		{
			return new StringContent(JsonConvert.SerializeObject(value, serializerSettings ?? defaultSettings), Encoding.UTF8, "application/json");
		}

		private string GetCorrelationId(RestActionOptions options)
		{
			if (!string.IsNullOrWhiteSpace(options?.CorrelationId))
			{
				return options.CorrelationId;
			}

			if (httpContextAccessor?.HttpContext != null)
			{
				var correlationId = httpContextAccessor.HttpContext.Request.Headers[CorrelationIdDefaults.CorrelationIdHeader].ToString();

				if (!string.IsNullOrEmpty(correlationId))
				{
					return correlationId;
				}

				return $"{config.CorrelationIdPrefix}-{httpContextAccessor.HttpContext.TraceIdentifier}";
			}

			return $"{config.CorrelationIdPrefix}-{Guid.NewGuid().ToString("N")}";
		}

		private string GetCorrelationIndex()
		{
			var index = 1;

			if (httpContextAccessor?.HttpContext != null)
			{
				var correlationIndexStr = httpContextAccessor.HttpContext.Request.Headers[CorrelationIdDefaults.CorrelationIndexHeader].ToString();

				if (!string.IsNullOrEmpty(correlationIndexStr))
				{
					if (int.TryParse(correlationIndexStr, out var correlationIndex))
					{
						index = ++correlationIndex;
					}
				}
			}

			return index.ToString();
		}

		private HttpClient ResolveHttpClient(RestActionOptions options)
		{
			var clientName = options.HttpClientName ?? config.DefaultHttpClientName;

			return string.IsNullOrWhiteSpace(clientName) ? clientFactory.CreateClient() : clientFactory.CreateClient(clientName);
		}

		private void ApplyHeaderSettings(HttpRequestMessage httpRequest, RestActionOptions options)
		{
			if (config.EnableCorrelationId)
			{
				httpRequest.Headers.Add(CorrelationIdDefaults.CorrelationIdHeader, GetCorrelationId(options));
				httpRequest.Headers.Add(CorrelationIdDefaults.CorrelationIndexHeader, GetCorrelationIndex());
			}

			if (options?.AuthorizationHeader != null)
			{
				httpRequest.Headers.Authorization = options.AuthorizationHeader;
			}

			if (options?.Headers.Any() == true)
			{
				foreach (var header in options.Headers.AsEnumerable())
				{
					httpRequest.Headers.Add(header.Key, header.Value);
				}
			}
		}

		#endregion
	}
}
