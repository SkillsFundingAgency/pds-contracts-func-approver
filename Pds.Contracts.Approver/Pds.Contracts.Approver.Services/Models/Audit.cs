namespace Pds.Contracts.Approver.Services.Models
{
    /// <summary>
    /// A representation of an Audit record.
    /// </summary>
    public class Audit
    {
        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        public int Severity { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the ukprn.
        /// </summary>
        public int? Ukprn { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public int Action { get; set; }
    }
}
