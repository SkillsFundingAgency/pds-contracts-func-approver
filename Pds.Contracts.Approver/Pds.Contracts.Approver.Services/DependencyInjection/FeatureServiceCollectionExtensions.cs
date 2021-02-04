using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pds.Contracts.Approver.Services.Configuration;
using Pds.Contracts.Approver.Services.HttpPolicyConfiguration;
using Pds.Contracts.Approver.Services.Implementations;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.ApiClient.Services;

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
            var policies = new PolicyType[] { PolicyType.Retry, PolicyType.CircuitBreaker };

            // Configure Polly Policies for IAuditService HttpClient
            services
                .AddPolicies<IAuditService>(config, policyRegistry)
                .AddHttpClient<IAuditService, AuditService, AuditApiConfiguration>(config, policies);

            // Configure Polly Policies for IContractsApproverService HttpClient
            services
                .AddPolicies<IContractsApproverService>(config, policyRegistry)
                .AddHttpClient<IContractsApproverService, ContractsApproverService, FcsApiClientConfiguration>(config, policies);

            services.AddTransient(typeof(IAuthenticationService<>), typeof(AuthenticationService<>));

            return services;
        }
    }
}