using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.Approver.Services.Implementations;
using Pds.Contracts.Approver.Services.Interfaces;
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
            var mockService = new Mock<IContractsApproverService>();
            var function = new ContractsApproverFunction(mockService.Object);

            // Act
            Func<Task> act = async () => { await function.Run(null, null); };

            // Assert
            act.Should().NotThrow();
        }
    }
}