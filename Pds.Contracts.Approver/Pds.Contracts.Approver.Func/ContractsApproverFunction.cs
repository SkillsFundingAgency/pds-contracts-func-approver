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
        private readonly IContractsApproverService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsApproverFunction"/> class.
        /// </summary>
        /// <param name="service">Service to be used as the message processor.</param>
        public ContractsApproverFunction(IContractsApproverService service)
        {
            this.service = service;
        }

        /// <summary>
        /// This executed by the service bus subscription trigger.
        /// </summary>
        /// <param name="mySbMsg"> The message to be processed.</param>
        /// <param name="log">An <see cref="ILogger"/> for log output.</param>
        [FunctionName("ContractsApproverFunction")]
        public async Task Run([ServiceBusTrigger(topicName: "%Pds.Contracts.Notifications.Topic%", subscriptionName: "%Pds.Contracts.Approval.Subscription%", Connection = "sb-connection-string")] string mySbMsg, ILogger log)
        {
            log?.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            await service.ProcessMessage(mySbMsg);
        }
    }
}
