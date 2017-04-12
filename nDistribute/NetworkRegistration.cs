namespace nDistribute
{
    using System;

    /// <summary>
    /// Methods to 
    /// </summary>
    public class NetworkRegistration
    {
        /// <summary>
        /// Gets or sets function used to analyse a <see cref="NodeAddress"/> and determine if <see cref="CreateNetwork"/> can create it.
        /// </summary>
        public Func<NodeAddress, bool> CanCreate { get; set; }

        /// <summary>
        /// Gets or sets function used to create a network appropriate for the Node type.
        /// </summary>
        public Func<NetworkBase> CreateNetwork { get; set; }
    }
}
