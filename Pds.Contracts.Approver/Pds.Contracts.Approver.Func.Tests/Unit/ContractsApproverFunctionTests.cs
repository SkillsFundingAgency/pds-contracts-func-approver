using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Contracts.Approver.Services.Models;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Func.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ContractsApproverFunctionTests
    {
        [TestMethod]
        public void RunMethod_OnValidMessage_ExecutesWithoutErrors()
        {
            // Arrange
            var mockService = new Mock<IContractsApproverService>(MockBehavior.Strict);

            var message = CreateTestContractApprovedMessage();

            mockService.Setup(p => p.ProcessMessage(It.IsAny<ContractApprovedMessage>()))
                .Returns(Task.CompletedTask);

            var function = new ContractsApproverFunction(mockService.Object);

            // Act
            Func<Task> act = async () =>
            {
                await function.Run(message, null);
            };

            // Assert
            act.Should().NotThrow();
            mockService.Verify();
        }

        private ContractApprovedMessage CreateTestContractApprovedMessage()
        {
            ContractApprovedMessage data = new ContractApprovedMessage()
            {
                ContractNumber = "123",
                MasterContractNumber = "123",
                ContractVersionNumber = 12
            };

            return data;
        }
    }
}