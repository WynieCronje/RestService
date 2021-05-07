using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RestService.Tests
{
	public class RestServiceStatusCodesTests
    {
        private const string dummyUrl = "http://dummy.com";

        private IOptions<RestServiceConfiguration> restServiceOptions = new RestServiceOptionsMock();

        private HttpClientFactoryMock Client404Factory = new HttpClientFactoryMock(HttpStatusCode.NotFound);
        private HttpClientFactoryMock Client200Factory = new HttpClientFactoryMock(HttpStatusCode.OK);
        private HttpClientFactoryMock Client400Factory = new HttpClientFactoryMock(HttpStatusCode.BadRequest);

        [Test]
        public void TestValid404MethodStatusCodeThrowsNoException()
        {
            //arrange
            IRestService restService = new RestService(null, Client404Factory, restServiceOptions);

            //act
            var result = restService
                .GetAsync<string>(new Uri(dummyUrl), new RestActionOptions().SetValidStatusCodes(HttpStatusCode.NotFound))
                .Result;

            //assert
            Assert.IsNull(default(string));
        }

        [Test]
        public void TestValid404ArrayStatusCodeThrowsNoException()
        {
            //arrange
            IRestService restService = new RestService(null, Client404Factory, restServiceOptions);

            //act
            var result = restService.GetAsync<string>(new Uri(dummyUrl), new RestActionOptions()
            {
                ValidStatusCodes = new HttpStatusCode[] { HttpStatusCode.NotFound }
            }).Result;

            //assert
            Assert.IsNull(default(string));
        }

        [Test]
        public void TestInvalid404StatusCodeThrowsException()
        {
            //arrange
            IRestService restService = new RestService(null, Client404Factory, restServiceOptions);

            //assert
            Assert.Throws<RestResponseException>(() =>
            {
                //act
                restService.GetAsync<string>(new Uri(dummyUrl)).GetAwaiter().GetResult();
            });
        }

        [Test]
        public void Test200StatusCodeThrowsNoException()
        {
            //arrange
            IRestService restService = new RestService(null, Client200Factory, restServiceOptions);

            //assert
            var result = Task.Run(() =>
            {
                return restService.GetAsync<string>(new Uri(dummyUrl), new RestActionOptions().SetValidStatusCodes(HttpStatusCode.NotFound));
            }).Result;

            //assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void NoDefaultValidResponseCodesAdded()
        {
            //arrange and act
            var options = new RestActionOptions();

            //assert
            Assert.AreEqual(0, options.ValidStatusCodes.Length);
        }
    }
}
