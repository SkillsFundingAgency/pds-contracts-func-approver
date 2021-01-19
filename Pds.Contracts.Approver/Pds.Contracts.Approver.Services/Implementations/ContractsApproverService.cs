using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pds.Contracts.Approver.Services.Configuration;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Contracts.Approver.Services.Models;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Implementations
{
    /// <summary>
    /// Service to process contract approval messages.
    /// </summary>
    public class ContractsApproverService : BaseApiClient<FcsApiClientConfiguration>, IContractsApproverService
    {
        /// <summary>
        /// Endpoint for contracts approval.
        /// </summary>
        public const string ContractApproveEndpoint = "/api/contract/approve";

        private readonly ILoggerAdapter<ContractsApproverService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsApproverService"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">ILogger reference to log output.</param>
        public ContractsApproverService(
            IAuthenticationService<FcsApiClientConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<FcsApiClientConfiguration> configurationOptions,
            ILoggerAdapter<ContractsApproverService> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value))
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task ProcessMessage(ContractApprovedMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _logger.LogInformation($"Sending approved contract notification for {message.ContractNumber}.");

            await PostWithAADAuth(ContractApproveEndpoint, message);
        }

        /// <inheritdoc/>
        protected override Action<ApiGeneralException> FailureAction
            => exception =>
            {
                _logger.LogError(exception, exception.Message);
                throw exception;
            };
    }
}