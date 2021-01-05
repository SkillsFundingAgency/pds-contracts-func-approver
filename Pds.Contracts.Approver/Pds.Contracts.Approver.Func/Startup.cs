using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Pds.Contracts.Approver.Func;
using Pds.Contracts.Approver.Services.DependencyInjection;

// See: https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
[assembly: FunctionsStartup(typeof(Startup))]

namespace Pds.Contracts.Approver.Func
{
    /// <summary>
    /// The startup class.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <inheritdoc/>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddFeatureServices();
        }
    }
}