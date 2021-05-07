using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestService.Tests
{
    public class HttpClientFactoryMock : IHttpClientFactory
    {
        private HttpStatusCode forceHttpStatusCode;

        public HttpClientFactoryMock(HttpStatusCode forceHttpStatusCode)
        {
            this.forceHttpStatusCode = forceHttpStatusCode;
        }

        public HttpClient CreateClient(string name)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = forceHttpStatusCode,
                   Content = new StringContent(""),
                   RequestMessage = new HttpRequestMessage()
                   {
                   }
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            return httpClient;
        }

    }
}
