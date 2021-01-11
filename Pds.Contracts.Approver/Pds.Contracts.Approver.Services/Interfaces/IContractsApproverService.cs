using System.Threading.Tasks;

namespace Pds.Contracts.Approver.Services.Interfaces
{
    /// <summary>
    /// An interface for contract approval service.
    /// </summary>
    public interface IContractsApproverService
    {
        /// <summary>
        /// Processes a contract approval message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <returns>Returns an async task.</returns>
        Task ProcessMessage(string message);
    }
}