namespace nDistribute.WCF.TestExe.Model
{
    using System;

    /// <summary>
    /// A simple outgoing message for testing
    /// </summary>
    [Serializable]
    public class OutgoingMessage
    {
        /// <summary>
        /// Gets or sets the actual textual message.
        /// </summary>
        public string Message { get; set; }
    }
}
