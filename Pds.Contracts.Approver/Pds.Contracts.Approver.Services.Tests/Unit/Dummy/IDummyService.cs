using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Tests.Unit.Dummy
{
    internal interface IDummyService
    {
        Task<HttpResponseMessage> MakeHttpCallAsync();
    }
}
