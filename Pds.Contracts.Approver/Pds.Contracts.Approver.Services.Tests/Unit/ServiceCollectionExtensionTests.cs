using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pds.Contracts.Approver.Services.HttpPolicyConfiguration;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Pds.Contracts.Approver.Services.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class ServiceCollectionExtensionTests
    {
        [TestMethod]
        public void AddPolicies_VerifyPoliciesAreAddedCorrectly()
        {
            // Arrange
            var dummyServiceCollection = new ServiceCollection();
            var mockConfig = Mock.Of<IConfiguration>();
            var mockRegistry = Mock.Of<IPolicyRegistry<string>>();
            var mockIConfigurationSection = Mock.Of<IConfigurationSection>();

            Mock.Get(mockConfig)
                .Setup(c => c.GetSection(nameof(HttpPolicyOptions)))
                .Returns(mockIConfigurationSection)
                .Verifiable();

            Mock.Get(mockRegistry)
                .Setup(r => r.Add($"{nameof(ServiceCollectionExtensionTests)}_{PolicyType.Retry}", It.IsAny<AsyncRetryPolicy<HttpResponseMessage>>()))
                .Verifiable();

            Mock.Get(mockRegistry)
                .Setup(r => r.Add($"{nameof(ServiceCollectionExtensionTests)}_{PolicyType.CircuitBreaker}", It.IsAny<AsyncCircuitBreakerPolicy<HttpResponseMessage>>()))
                .Verifiable();


            // Act
            dummyServiceCollection.AddPollyPolicies<ServiceCollectionExtensionTests>(mockConfig, mockRegistry);

            // Assert
            Mock.Verify(Mock.Get(mockConfig), Mock.Get(mockRegistry));
        }
    }
}
