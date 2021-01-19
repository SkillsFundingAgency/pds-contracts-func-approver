using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Pds.Contracts.Approver.Services.Configuration;
using Pds.Contracts.Approver.Services.Implementations;
using Pds.Contracts.Approver.Services.Models;
using Pds.Core.ApiClient;
using Pds.Core.ApiClient.Exceptions;
using Pds.Core.ApiClient.Interfaces;
using Pds.Core.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ContractApproverServiceTests
    {
        private const string TestBaseAddress = "http://test-api-endpoint";

        private const string TestFakeAccessToken = "AccessToken";
        private const string TestFakeExceptionMessage = "Test Error";

        private readonly HttpMessageHandler _httpMessageHandler = Mock.Of<HttpMessageHandler>(MockBehavior.Strict);

        [TestMethod]
        public void ContractPost_ReturnsExpectedResult()
        {
            // Arrange
            var message = GetContractApprovedMessage();
            var httpClient = GetHttpClient();

            SetupMessageHandler(HttpStatusCode.OK);

            var fcsConfiguration = Options.Create(GetServicesConfiguration());

            var mockAuthentication = GetMockAuthenticationService<FcsApiClientConfiguration>();
            var mockLogger = new Mock<ILoggerAdapter<ContractsApproverService>>(MockBehavior.Strict);
            mockLogger.Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));

            var contractService = new ContractsApproverService(mockAuthentication.Object, httpClient, fcsConfiguration, mockLogger.Object);

            // Act
            Func<Task> act = async () => await contractService.ProcessMessage(message);

            // Assert
            act.Should().NotThrowAsync();
            mockLogger.Verify(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce());
            VerifyMessageHandlerWhenUsingAADAuth(HttpMethod.Post, TestBaseAddress + "/api/contract/approve");
        }

        [TestMethod]
        public void ContractPost_OnError_LogsError()
        {
            // Arrange
            var message = GetContractApprovedMessage();
            var httpClient = GetHttpClient();

            SetupMessageHandlerException(TestFakeExceptionMessage);

            var fcsConfiguration = Options.Create(GetServicesConfiguration());

            var mockAuthentication = GetMockAuthenticationService<FcsApiClientConfiguration>();
            var mockLogger = new Mock<ILoggerAdapter<ContractsApproverService>>(MockBehavior.Strict);
            mockLogger.Setup(p => p.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));
            mockLogger.Setup(p => p.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));

            var contractService = new ContractsApproverService(mockAuthentication.Object, httpClient, fcsConfiguration, mockLogger.Object);

            // Act
            Func<Task> act = async () => await contractService.ProcessMessage(message);

            // Assert
            var exceptionAssertions = act.Should()
                .Throw<ApiGeneralException>()
                .WithMessage("Request failed with unknown status code*");

            exceptionAssertions
                .WithInnerException<Exception>()
                    .WithMessage(TestFakeExceptionMessage);

            exceptionAssertions
                .Which.ResponseStatusCode.Should().BeNull();
            mockLogger.Verify(p => p.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));
            VerifyMessageHandlerWhenUsingAADAuth(HttpMethod.Post, TestBaseAddress + "/api/contract/approve");
        }

        #region Setup Helpers

        private static ContractApprovedMessage GetContractApprovedMessage()
        {
            // Arrange
            return new ContractApprovedMessage() { ContractNumber = "123", MasterContractNumber = "12", ContractVersionNumber = 1 };
        }

        private HttpClient GetHttpClient()
            => new HttpClient(_httpMessageHandler);

        private FcsApiClientConfiguration GetServicesConfiguration()
            => new FcsApiClientConfiguration
            {
                ApiBaseAddress = TestBaseAddress
            };

        private Mock<IAuthenticationService<T>> GetMockAuthenticationService<T>()
            where T : BaseApiClientConfiguration
        {
            var mockAuthenticationService = new Mock<IAuthenticationService<T>>(MockBehavior.Strict);
            mockAuthenticationService.Setup(x => x.GetAccessTokenForAAD()).Returns(
                Task.FromResult(TestFakeAccessToken));
            return mockAuthenticationService;
        }

        private void SetupMessageHandler(HttpStatusCode statusCode, TestResponseClass responseObject = null)
        {
            var expectedResponse = new HttpResponseMessage
            {
                StatusCode = statusCode
            };

            if (responseObject != null)
            {
                var responseContent = JsonConvert.SerializeObject(responseObject);
                expectedResponse.Content = new StringContent(responseContent);
            }

            Mock.Get(_httpMessageHandler)
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(expectedResponse);
        }

        private void SetupMessageHandlerException(string message)
        {
            Mock.Get(_httpMessageHandler)
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception(message));
        }

        #endregion


        #region Verify Helpers

        private void VerifyMessageHandlerWhenUsingAADAuth(HttpMethod httpMethod, string expectedUri)
        {
            Mock.Get(_httpMessageHandler)
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(), // we expected a single external request
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.Method.Equals(httpMethod)
                               && req.RequestUri.Equals(new Uri(expectedUri))
                               && req.Headers.Authorization.Parameter.Equals(TestFakeAccessToken)
                               && req.Headers.Authorization.Scheme.Equals("Bearer")),
                    ItExpr.IsAny<CancellationToken>());
        }

        #endregion


        #region Test Response

        private class TestResponseClass : IEquatable<TestResponseClass>
        {
            /// <summary>
            /// Gets or sets int representing the response code.
            /// </summary>
            public int TestInt { get; set; }

            /// <summary>
            /// Gets or sets string representing the response message.
            /// </summary>
            public string TestString { get; set; }

            /// <inheritdoc/>
            public bool Equals([AllowNull] TestResponseClass other)
            {
                return other != null &&
                       TestInt == other.TestInt &&
                       TestString == other.TestString;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                return HashCode.Combine(TestInt, TestString);
            }
        }

        #endregion
    }
}