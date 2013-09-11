namespace nDistribute
{
    /// <summary>The first come first elected strategy.</summary>
    public class FirstComeFirstElectedStrategy : IElectionStrategy
    {
        /// <summary>Keeps the existing parent and child.</summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        public void DetermineParent(ref INode parent, ref INode child)
        {
            // Do nothing, parent and child are in the right order
        }
    }
}