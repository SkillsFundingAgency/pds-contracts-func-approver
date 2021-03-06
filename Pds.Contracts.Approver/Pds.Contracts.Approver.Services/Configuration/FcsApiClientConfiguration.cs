﻿using Pds.Core.ApiClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pds.Contracts.Approver.Services.Configuration
{
    /// <summary>
    /// Class to hold configuration settings for a FCS API Client.
    /// </summary>
    public class FcsApiClientConfiguration : BaseApiClientConfiguration
    {
        /// <summary>
        /// Gets or sets the api contract approver endpoint.
        /// </summary>
        public string ApiContractApproverEndpoint { get; set; }
    }
}
