using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.Approver.Services.Implementations;
using Pds.Core.Logging;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Tests.Unit
{
    [TestClass]
    public class ContractApproverServiceTests
    {
        [TestMethod, TestCategory("Unit")]
        public async Task Hello_ReturnsExpectedResult()
        {
            // Arrange
            var message = "Hello, world!";
            var expected = $">> Processing message {message}";

            var mockLogger = new Mock<ILoggerAdapter<ContractsApproverService>>();
            mockLogger.Setup(p => p.LogInformation(It.IsAny<string>()));

            var exampleService = new ContractsApproverService(mockLogger.Object);

            // Act
            await exampleService.ProcessMessage(message);

            // Assert
            mockLogger.VerifyAll();
        }
    }
}