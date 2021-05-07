using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestService
{
	public interface IRestService
    {
        /// <summary>
        /// Adds CorrelationId. Throws <see cref="RestResponseException"/> 
        /// </summary>
        Task<TResponse> DeleteAsync<TResponse>(Uri apiAction, RestActionOptions options = null);

        /// <summary>
        /// Adds CorrelationId. Throws <see cref="RestResponseException"/> 
        /// </summary>
        Task<TResponse> GetAsync<TResponse>(Uri apiAction, RestActionOptions options = null);

        /// <summary>
        /// Adds CorrelationId. Throws <see cref="RestResponseException"/> 
        /// </summary>
        Task<TResponse> PostAsync<TResponse, TRequest>(Uri apiAction, TRequest requestBody, RestActionOptions options = null);

        /// <summary>
        /// Adds CorrelationId. Throws <see cref="RestResponseException"/> 
        /// </summary>
        Task<TResponse> PostAsync<TResponse>(Uri apiAction, object requestBody, RestActionOptions options = null);

        /// <summary>
        /// Adds CorrelationId. Throws <see cref="RestResponseException"/> 
        /// </summary>
        Task<TResponse> PutAsync<TResponse, TRequest>(Uri apiAction, TRequest requestBody, RestActionOptions options = null);

        /// <summary>
        /// Adds CorrelationId. Throws <see cref="RestResponseException"/> 
        /// </summary>
        Task<TResponse> PutAsync<TResponse>(Uri apiAction, object requestBody, RestActionOptions options = null);

        /// <summary>
        /// Only adds CorrelationId, No Exceptions are thrown
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest, RestActionOptions options = null);
    }
}
