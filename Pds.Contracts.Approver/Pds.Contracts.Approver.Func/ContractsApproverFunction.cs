using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ContractsApproverFunction> _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractsApproverFunction"/> class.
        /// </summary>
        /// <param name="log"> Log is used to log all the information to the log provider.</param>
        public ContractsApproverFunction(ILogger<ContractsApproverFunction> log)
        {
            _log = log;
        }

        /// <summary>
        /// This executed by the service bus subscription trigger.
        /// </summary>
        /// <param name="mySbMsg"> The message to be processed.</param>
        [FunctionName("ContractsApproverFunction")]
        public void Run([ServiceBusTrigger(topicName: "%Pds.Contracts.Notifications.Topic%", subscriptionName: "%Pds.Contracts.Approval.Subscription%", Connection = "sb-connection-string")] string mySbMsg)
        {
            _log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
