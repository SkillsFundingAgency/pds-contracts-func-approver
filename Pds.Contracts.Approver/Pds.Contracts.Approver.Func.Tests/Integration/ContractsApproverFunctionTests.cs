using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Contracts.Approver.Services.Implementations;
using System;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Func.Tests.Integration
{
    [TestClass]
    public class ContractsApproverFunctionTests
    {
        [TestMethod, TestCategory("Integration")]
        public void Run_DoesNotThrowException()
        {
            // Arrange
            var loggerFactory = new LoggerFactory();

            var function = new ContractsApproverFunction(loggerFactory.CreateLogger<ContractsApproverFunction>());

            // Act
            function.Run("This is from SB");
        }
    }
}