using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Contracts.Approver.Services.Implementations;
using System;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Func.Tests.Integration
{
    [TestClass]
    public class ExampleServiceBusFunctionTests
    {
        [TestMethod, TestCategory("Integration")]
        public void Run_DoesNotThrowException()
        {
            // Arrange
            var exampleService = new ExampleService();

            var function = new ExampleServiceBusFunction(exampleService);

            // Act
            Func<Task> act = async () => { await function.Run(null, null); };

            // Assert
            act.Should().NotThrow();
        }
    }
}