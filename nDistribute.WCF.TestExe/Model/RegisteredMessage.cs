namespace nDistribute.WCF.TestExe.Model
{
    using System;

    /// <summary>
    /// A different type of message.
    /// </summary>
    [Serializable]
    public class RegisteredMessage
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the addresss.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets the address as a <see cref="NodeAddress"/>
        /// </summary>
        /// <returns>A <see cref="NodeAddress"/> built from the <see cref="Address"/>.</returns>
        public NodeAddress ToAddress()
        {
            return new NodeAddress(Address);
        }
    }
}
