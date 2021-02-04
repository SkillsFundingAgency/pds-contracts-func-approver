using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Tests.Unit.Dummy
{
    internal class DummyService : IDummyService
    {
        private readonly HttpClient _client;

        public DummyService(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> MakeHttpCallAsync()
        {
            return await _client.GetAsync("https://testhost/test/Get");
        }
    }
}
