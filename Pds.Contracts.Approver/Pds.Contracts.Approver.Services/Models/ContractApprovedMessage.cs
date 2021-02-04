namespace Pds.Contracts.Approver.Services.Models
{
    /// <summary>
    /// Details of the contract that was approved.
    /// </summary>
    public class ContractApprovedMessage
    {
        /// <summary>
        /// Gets or sets the number identifying the contract.
        /// </summary>
        public string ContractNumber { get; set; }

        /// <summary>
        /// Gets or sets the version number of the contract.
        /// </summary>
        public int ContractVersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the number identifying the parent of this contract.
        /// </summary>
        public string MasterContractNumber { get; set; }
    }
}
