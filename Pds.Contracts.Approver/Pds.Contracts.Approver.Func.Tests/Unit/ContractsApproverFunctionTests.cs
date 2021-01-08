using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.Approver.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Func.Tests.Unit
{
    [TestClass]
    public class ContractsApproverFunctionTests
    {
        [TestMethod, TestCategory("Integration")]
        public void Run_DoesNotThrowException()
        {
            // Arrange
            var mockLoggerService = new Mock<ILogger<ContractsApproverFunction>>();

            var function = new ContractsApproverFunction(mockLoggerService.Object);

            // Act
            function.Run(null);

            // Assert
            mockLoggerService.Verify();
        }
    }
}