using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.Approver.Services.Configuration;
using Pds.Contracts.Approver.Services.Implementations;
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
    public class AuditServiceTests
    {
        private const string TestBaseAddress = "http://test-api-endpoint";

        private const string TestFakeAccessToken = "AccessToken";

        [TestMethod]
        public void CreateAuditAsync_InvokesAuditService_MockHttp()
        {
            Mock.Get(_auditLogger)
                .Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));

            _mockHttpMessageHandler
                .Expect(TestBaseAddress + "/api/auditasync")
                .Respond(HttpStatusCode.OK);

            AuditService auditService = CreateAuditService();
            Audit audit = CreateAudit();

            // Act
            Func<Task> act = async () => await auditService.CreateAuditAsync(audit);

            // Assert
            act.Should().NotThrow();
            _mockHttpMessageHandler.VerifyNoOutstandingExpectation();
            VerifyAllMocks();
        }

        [TestMethod]
        public void CreateAuditAsync_WhenHttpRequestFails_ErrorIsRaised()
        {
            // Arrange;
            _mockHttpMessageHandler
                .Expect(TestBaseAddress + "/api/auditasync")
                .Respond(HttpStatusCode.BadRequest, "application/text", "Bad Request");

            Mock.Get(_auditLogger)
                .Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));
            Mock.Get(_auditLogger)
                .Setup(p => p.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));

            AuditService auditService = CreateAuditService();

            Audit audit = CreateAudit();

            // Act
            Func<Task> act = async () =>
            {
                await auditService.CreateAuditAsync(audit);
            };

            // Assert
            act.Should()
                .Throw<ApiGeneralException>()
                .Where(e => e.ResponseStatusCode == HttpStatusCode.BadRequest);

            VerifyAllMocks();
        }

        #region Setup Helpers

        private readonly MockHttpMessageHandler _mockHttpMessageHandler
            = new MockHttpMessageHandler();

        private readonly ILoggerAdapter<AuditService> _auditLogger
            = Mock.Of<ILoggerAdapter<AuditService>>(MockBehavior.Strict);

        private static Audit CreateAudit()
        {
            return new Models.Audit()
            {
                Action = AuditService.AuditActionType_ContractApprovedMessageSentToFCS,
                Message = "Unit Test Method",
                Severity = AuditService.AuditSeverityLevel_Information,
                Ukprn = null,
                User = "UnitTester"
            };
        }

        private AuditService CreateAuditService()
        {
            var httpClient = _mockHttpMessageHandler.ToHttpClient();
            var authenticationService = GetAuthenticationService();
            var auditConfiguration = Options.Create(GetServicesConfiguration());

            return new AuditService(authenticationService, httpClient, auditConfiguration, _auditLogger);
        }

        private IAuthenticationService<AuditApiConfiguration> GetAuthenticationService()
        {
            var mockAuthenticationService = new Mock<IAuthenticationService<AuditApiConfiguration>>(MockBehavior.Strict);
            mockAuthenticationService
                .Setup(x => x.GetAccessTokenForAAD())
                .Returns(Task.FromResult(TestFakeAccessToken));
            return mockAuthenticationService.Object;
        }

        private AuditApiConfiguration GetServicesConfiguration()
            => new AuditApiConfiguration()
            {
                ApiBaseAddress = TestBaseAddress
            };
        #endregion

        #region Verify Helpers

        private void VerifyAllMocks()
        {
            Mock.Get(_auditLogger).VerifyAll();
        }
        #endregion
    }
}
