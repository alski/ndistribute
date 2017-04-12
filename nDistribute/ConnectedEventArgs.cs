namespace nDistribute
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information supplied when the connection status changes.
    /// </summary>
    public class ConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the set of connection names;
        /// </summary>
        public IEnumerable<string> Connected { get; set; }
    }
}