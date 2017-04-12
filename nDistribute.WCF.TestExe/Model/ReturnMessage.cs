namespace nDistribute.WCF.TestExe.Model
{
    using System;

    /// <summary>
    /// A return message.
    /// </summary>
    [Serializable]
    public class ReturnMessage
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// Gets or sets the address of the node.
        /// </summary>
        public string NodeAddress { get; set; }
    }
}
