using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Pds.Contracts.Approver.Services.Interfaces;
using System;
using System.Collections.Generic;
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
        /// <param name="message"> The message to be processed.</param>
        /// <param name="log">An <see cref="ILogger"/> for log output.</param>
        /// <returns>An awaitable task.</returns>
        [FunctionName("ContractsApproverFunction")]
        public async Task Run([ServiceBusTrigger(topicName: "%Pds.Contracts.Notifications.Topic%", subscriptionName: "%Pds.Contracts.Approval.Subscription%", Connection = "sb-connection-string")] string message, ILogger log)
        {
            log?.LogInformation($"C# ServiceBus topic trigger function processed message: {message}");
            await _service.ProcessMessage(message);
        }
    }
}
