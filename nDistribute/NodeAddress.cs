namespace nDistribute
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;

    /// <summary>The node address.</summary>
    [DebuggerDisplay("{Address}")]
    [DataContract]
    public class NodeAddress : IEquatable<NodeAddress>
    {
        /// <summary>Initialises a new instance of the <see cref="NodeAddress"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="address">The address </param>
        public NodeAddress(string address)
        {
            this.Address = address;
        }

        /// <summary>Gets or sets the address.</summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>Gets or sets the parent.</summary>
        [DataMember]
        public NodeAddress Parent { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        [Obsolete]
        public bool Equals(NodeAddress other)
        {
            throw new NotImplementedException("Making you explicitly use .Matches(...)");
        }

        /// <summary>Returns a string representation of the <see cref="NodeAddress"/>.</summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return this.Address;
        }

        /// <summary>Checks if one matches another</summary>
        /// <param name="other">The other.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        internal bool Matches(NodeAddress other)
        {
            return other != null
             && this.Address == other.Address;
        }
    }
}
