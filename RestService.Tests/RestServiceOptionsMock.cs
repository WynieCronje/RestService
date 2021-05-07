using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestService.Tests
{
    public class RestServiceOptionsMock : IOptions<RestServiceConfiguration>
    {
        public RestServiceConfiguration Value => new RestServiceConfiguration()
        {
            CorrelationIdPrefix = "UnitTest"
        };
    }
}
