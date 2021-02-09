using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.Approver.Services.Configuration;
using Pds.Contracts.Approver.Services.Implementations;
using Pds.Contracts.Approver.Services.Interfaces;
using Pds.Contracts.Approver.Services.Models;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Logging;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ContractApproverServiceTests
    {
        private const string TestBaseAddress = "http://test-api-endpoint";
        private const string TestApiEndpoint = "/testapi/contract/approve";
        private const string TestFakeAccessToken = "AccessToken";

        [TestMethod]
        public void ContractPost_ReturnsExpectedResult()
        {
            // Arrange
            var message = GetContractApprovedMessage();

            _mockHttpMessageHandler
                .Expect(TestBaseAddress + TestApiEndpoint)
                .Respond(HttpStatusCode.OK);

            Mock.Get(_auditApi)
                   .Setup(p => p.CreateAuditAsync(It.IsAny<Audit>()))
                   .Returns(Task.CompletedTask);

            Mock.Get(_contractsApproverLogger)
                .Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();

            var contractService = CreateContractsApproverService();

            // Act
            Func<Task> act = async () => await contractService.ProcessMessage(message);

            // Assert
            act.Should().NotThrow();
            VerifyAllMocks();
        }

        [TestMethod]
        public void ProcessMessage_OnSuccess_LogsToLoggerAndAudit()
        {
            // Arrange
            var message = GetContractApprovedMessage();
            int expectedAuditActionType = AuditService.AuditActionType_ContractApprovedMessageSentToFCS;
            int actualAuditActionType = -1; // Init to invalid value - Will be overriden in mock

            _mockHttpMessageHandler
                .Expect(TestBaseAddress + TestApiEndpoint)
                .Respond(HttpStatusCode.OK);

            Mock.Get(_auditApi)
                .Setup(p => p.CreateAuditAsync(It.IsAny<Audit>()))
                .Returns((Audit audit) =>
                    {
                        actualAuditActionType = audit.Action;
                        return Task.CompletedTask;
                    })
                .Verifiable();

            Mock.Get(_contractsApproverLogger)
                .Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();

            var contractService = CreateContractsApproverService();

            // Act
            Func<Task> act = async () => await contractService.ProcessMessage(message);

            // Assert
            act.Should().NotThrow();
            actualAuditActionType.Should().Be(expectedAuditActionType);
            VerifyAllMocks();
        }

        [TestMethod]
        public void ProcessMessage_OnAuditAPIFailure_LogsErrorAndContinues()
        {
            // Arrange
            var message = GetContractApprovedMessage();
            _mockHttpMessageHandler
                .Expect(TestBaseAddress + TestApiEndpoint)
                .Respond(HttpStatusCode.OK);

            Mock.Get(_auditApi)
                .Setup(p => p.CreateAuditAsync(It.IsAny<Audit>()))
                .Returns((Audit audit) => throw new Exception())
                .Verifiable();

            Mock.Get(_contractsApproverLogger)
                .Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()))
                .Verifiable();
            Mock.Get(_contractsApproverLogger)
                .Setup(p => p.LogError(It.IsAny<Exception>(), It.IsAny<string>()))
                .Verifiable();

            var contractService = CreateContractsApproverService();

            // Act
            Func<Task> act = async () => await contractService.ProcessMessage(message);

            // Assert
            act.Should().NotThrow();
            VerifyAllMocks();
        }

        [TestMethod]
        public void ContractPost_OnErrorWhenSendingToFCS_LogsError()
        {
            // Arrange
            var message = GetContractApprovedMessage();
            _mockHttpMessageHandler
                .Expect(TestBaseAddress + TestApiEndpoint)
                .Respond(HttpStatusCode.ServiceUnavailable, "text/html", "Service Not Available");

            Mock.Get(_contractsApproverLogger)
                .Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));
            Mock.Get(_contractsApproverLogger)
                .Setup(p => p.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));

            var contractService = CreateContractsApproverService();

            // Act
            Func<Task> act = async () => await contractService.ProcessMessage(message);

            // Assert
            var exceptionAssertions = act.Should()
                .Throw<ApiGeneralException>()
                .And.ResponseStatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

            VerifyAllMocks();
        }

        #region Setup Helpers

        private static ContractApprovedMessage GetContractApprovedMessage()
        {
            return new ContractApprovedMessage()
            {
                ContractNumber = "123",
                MasterContractNumber = "12",
                ContractVersionNumber = 1
            };
        }

        private readonly MockHttpMessageHandler _mockHttpMessageHandler
            = new MockHttpMessageHandler();

        private readonly IAuditService _auditApi
            = Mock.Of<IAuditService>(MockBehavior.Strict);

        private readonly ILoggerAdapter<ContractsApproverService> _contractsApproverLogger
            = Mock.Of<ILoggerAdapter<ContractsApproverService>>(MockBehavior.Strict);

        private ContractsApproverService CreateContractsApproverService()
        {
            var httpClient = _mockHttpMessageHandler.ToHttpClient();

            var authenticationService = GetAuthenticationService();
            var fcsConfiguration = Options.Create(GetServicesConfiguration());

            return new ContractsApproverService(authenticationService, httpClient, fcsConfiguration, _auditApi, _contractsApproverLogger);
        }

        private IAuthenticationService<FcsApiClientConfiguration> GetAuthenticationService()
        {
            var mockAuthenticationService = new Mock<IAuthenticationService<FcsApiClientConfiguration>>(MockBehavior.Strict);
            mockAuthenticationService
                .Setup(x => x.GetAccessTokenForAAD())
                .Returns(Task.FromResult(TestFakeAccessToken));
            return mockAuthenticationService.Object;
        }

        private FcsApiClientConfiguration GetServicesConfiguration()
            => new FcsApiClientConfiguration()
            {
                ApiBaseAddress = TestBaseAddress,
                ApiContractApproverEndpoint = TestApiEndpoint
            };
        #endregion

        #region Verify Helpers

        private void VerifyAllMocks()
        {
            _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
            Mock.Get(_auditApi).Verify();
            Mock.Get(_contractsApproverLogger).Verify();
        }
        #endregion
    }
}