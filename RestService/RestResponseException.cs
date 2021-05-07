using System;
using System.Net;
using System.Net.Http;

namespace RestService
{
	public class RestResponseException : Exception
    {
        public RestResponseException(HttpResponseMessage response) :
            base($"API Error: {response.RequestMessage.RequestUri} {response.RequestMessage.Method} {(int)response.StatusCode} ({response.StatusCode})")
        {
            Response = response;

            StatusCode = response.StatusCode;
            RequestUri = response.RequestMessage.RequestUri;
            RequestMethod = response.RequestMessage.Method.ToString();
        }

        public string RequestMethod { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public HttpResponseMessage Response { get; private set; }
        public Uri RequestUri { get; private set; }
    }
}
