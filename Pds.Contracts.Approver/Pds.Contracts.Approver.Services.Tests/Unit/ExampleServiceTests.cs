using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pds.Contracts.Approver.Services.Implementations;
using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Tests.Unit
{
    [TestClass]
    public class ExampleServiceTests
    {
        [TestMethod, TestCategory("Unit")]
        public async Task Hello_ReturnsExpectedResult()
        {
            // Arrange
            var expected = "Hello, world!";

            var exampleService = new ContractsApproverService();

            // Act
            var actual = await exampleService.ProcessMessage();

            // Assert
            actual.Should().Be(expected);
        }
    }
}