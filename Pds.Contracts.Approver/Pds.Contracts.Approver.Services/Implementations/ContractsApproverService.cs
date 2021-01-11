using Microsoft.Extensions.Logging;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Core.Logging;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Implementations
{
    /// <summary>
    /// Service to process contract approval messages.
    /// </summary>
    public class ContractsApproverService : IContractsApproverService
    {
        private readonly ILoggerAdapter<ContractsApproverService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsApproverService"/> class.
        /// </summary>
        /// <param name="logger">ILogger reference to log output.</param>
        public ContractsApproverService(ILoggerAdapter<ContractsApproverService> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Processes a contract approval message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>Returns an async task.</returns>
        public async Task ProcessMessage(string message)
        {
            _logger.LogInformation($">> Processing message {message}");
            await Task.CompletedTask;
        }
    }
}