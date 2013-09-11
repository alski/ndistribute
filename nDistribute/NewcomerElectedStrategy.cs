namespace nDistribute
{
    /// <summary>The newcomer elected strategy.</summary>
    /// <remarks>A placeholder strategy put in simply to keep parent and child from being refactored out of the interface.
    /// I have no idea how you would possibly make use of this.</remarks>
    public class NewcomerElectedStrategy : IElectionStrategy
    {
        /// <summary>Reverses the existing parent and child.</summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        public void DetermineParent(ref INode parent, ref INode child)
        {
            var temp = child;
            child = parent;
            parent = temp;
        }
    }
}