using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
