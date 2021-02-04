using Microsoft.Extensions.Options;
using Pds.Contracts.Approver.Services.Configuration;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Contracts.Approver.Services.Models;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Implementations
{
    /// <summary>
    /// Service wrapper to allow calls to be made to the Audit API.
    /// </summary>
    public class AuditService : BaseApiClient<AuditApiConfiguration>, IAuditService
    {
        /// <summary>
        /// ActionType enum - Contract Approved Message successfully sent to FCS API.
        /// </summary>
        public const int AuditActionType_ContractApprovedMessageSentToFCS = 37;

        /// <summary>
        /// SeverityLevel enum - Severity for general information audit.
        /// </summary>
        public const int AuditSeverityLevel_Information = 0;

        private readonly ILoggerAdapter<AuditService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditService"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="retryMechanism">IRetryMechanism reference to allow retries of audit operations.</param>
        /// <param name="logger">ILogger reference to log output.</param>
        public AuditService(
            IAuthenticationService<AuditApiConfiguration> authenticationService,
            HttpClient httpClient,
            IOptions<AuditApiConfiguration> configurationOptions,
            ILoggerAdapter<AuditService> logger)
            : base(authenticationService, httpClient, Options.Create(configurationOptions.Value))
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task CreateAuditAsync(Audit audit)
        {
            _logger.LogInformation($"Creating an audit entry for action [{audit.Action}] : {audit.Message}");

            await PostWithAADAuth("/api/auditasync", audit);
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