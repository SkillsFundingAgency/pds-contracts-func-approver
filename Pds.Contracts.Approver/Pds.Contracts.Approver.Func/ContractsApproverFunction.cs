using System;
using System.Collections.Generic;
using System.Text;

namespace Pds.Contracts.Approver.Func
{
    public class ContractsApproverFunction
    {
        public ContractsApproverFunction()
        {
            // [ServiceBusTrigger("mytopichere", "mysubscriptionhere", Connection = "connectionstringnamehere")] string mySbMsg
        }


        [FunctionName("Function1")]
        public void Run(, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
