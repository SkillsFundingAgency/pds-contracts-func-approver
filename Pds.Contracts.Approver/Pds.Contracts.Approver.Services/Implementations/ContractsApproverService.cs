using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pds.Audit.Api.Client.Enumerations;
using Pds.Audit.Api.Client.Interfaces;
using Pds.Audit.Api.Client.Models;
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
        private const string ContractApproverUser = "System-ContractApprover";

        private readonly ILoggerAdapter<ContractsApproverService> _logger;

        private readonly IAuditService _audit;

        private readonly FcsApiClientConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsApproverService"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="audit">The audit service to log to.</param>
        /// <param name="logger">ILogger reference to log output.</param>
        public ContractsApproverService(
            IAuthenticationService<FcsApiClientConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<FcsApiClientConfiguration> configurationOptions,
            IAuditService audit,
            ILoggerAdapter<ContractsApproverService> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value))
        {
            _logger = logger;
            _audit = audit;
            _configuration = configurationOptions.Value;
        }

        /// <inheritdoc/>
        public async Task ProcessMessage(ContractApprovedMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _logger.LogInformation($"Sending approved contract notification for {message.ContractNumber}.");

            await Post(_configuration.ApiContractApproverEndpoint, message);

            try
            {
                await _audit.AuditAsync(new Audit.Api.Client.Models.Audit()
                {
                    Action = ActionType.ContractApprovedMessageSentToFCS,
                    Severity = 0,
                    User = ContractApproverUser,
                    Ukprn = message.Ukprn,
                    Message = $"Notified FCS of approved contract [{message.ContractNumber}] Version [{message.ContractVersionNumber}]."
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when attempting to create an audit entry.");
            }
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