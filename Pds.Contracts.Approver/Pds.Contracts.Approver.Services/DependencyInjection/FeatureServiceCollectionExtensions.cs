using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.Audit.Api.Client.Registrations;
using Pds.Contracts.Approver.Services.Configuration;
using Pds.Contracts.Approver.Services.HttpPolicyConfiguration;
using Pds.Contracts.Approver.Services.Implementations;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.ApiClient.Services;
using System.Collections.Generic;

namespace Pds.Contracts.Approver.Services.DependencyInjection
{
    /// <summary>
    /// Extensions class for <see cref="IServiceCollection"/> for registering the feature's services.
    /// </summary>
    public static class FeatureServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for the current feature to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the feature's services to.</param>
        /// <param name="config">The <see cref="IConfiguration"/> elements for the current service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFeatureServices(this IServiceCollection services, IConfiguration config)
        {
            var policyRegistry = services.AddPolicyRegistry();
            var policies = new List<PolicyType>() { PolicyType.Retry, PolicyType.CircuitBreaker };

            // Configure Polly Policies for IContractsApproverService HttpClient
            services
                .AddPollyPolicies<IContractsApproverService>(config, policyRegistry)
                .AddHttpClient<IContractsApproverService, ContractsApproverService, FcsApiClientConfiguration>(config, policies);

            services.AddTransient(typeof(IAuthenticationService<>), typeof(AuthenticationService<>));

            services.AddAuditApiClient(config, policyRegistry);

            return services;
        }
    }
}