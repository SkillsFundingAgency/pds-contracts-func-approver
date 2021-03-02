using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Contracts.Approver.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Func
{
    /// <summary>
    /// An Azure function to handle the Contracts Approval notification.
    /// </summary>
    public class ContractsApproverFunction
    {
        private readonly IContractsApproverService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsApproverFunction"/> class.
        /// </summary>
        /// <param name="service">Service to be used as the message processor.</param>
        public ContractsApproverFunction(IContractsApproverService service)
        {
            _service = service;
        }

        /// <summary>
        /// Invokes the associated <see cref="IContractsApproverService"/> to process the message.
        /// </summary>
        /// <remarks>
        /// The <see cref="ExponentialBackoffRetryAttribute"/> allows the function to be invoked if the message fails to process.
        /// The number of retries specified here is multiplative with the message bus max delivery count.
        /// When a message fails to process, the function will attempt to retry the number of attempts specified here.
        /// The message will then be returned to the queue and it's delivery count incremented by 1.
        /// As such, setting the retry value to 5, and a max delivery count of 10 on the service bus will result in a total of 50 attempts to process the message.
        /// This retry count applies to all messages, including malformed or bad messages.
        /// see - https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-error-pages?tabs=csharp#using-retry-support-on-top-of-trigger-resilience.
        /// </remarks>
        /// <param name="message">The message to be processed.</param>
        /// <param name="log">An <see cref="ILogger"/> for log output.</param>
        /// <returns>An awaitable task.</returns>
        [FunctionName("ContractsApproverFunction")]
        [ExponentialBackoffRetry(5, "00:00:05", "00:15:00")]
        public async Task Run([ServiceBusTrigger(topicName: "%Pds.Contracts.Notifications.Topic%", subscriptionName: "%Pds.Contracts.Approval.Subscription%", Connection = "sb-connection-string")] ContractApprovedMessage message, ILogger log)
        {
            log?.LogInformation($"Servicebus received message with contract no {message.ContractNumber}: Servicebus topic trigger received message.");

            try
            {
                await _service.ProcessMessage(message);
            }
            catch (Exception ex)
            {
                // Message serialisation or processing error
                // Log the error and exit with error code
                log?.LogError(ex, $"Failed to process contract approval for contract {message.ContractNumber}: {ex.Message}");
                throw;
            }
        }
    }
}
