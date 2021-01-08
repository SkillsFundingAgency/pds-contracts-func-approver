using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pds.Contracts.Approver.Func;
using Pds.Contracts.Approver.Services.DependencyInjection;
using System.IO;

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
            FunctionsHostBuilderContext context = builder.GetContext();
            var config = new ConfigurationBuilder()
           .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"local.settings.json"), optional: true, reloadOnChange: false)
           .AddEnvironmentVariables()
           .Build();
        }
    }
}