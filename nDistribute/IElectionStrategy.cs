namespace nDistribute
{
    /// <summary>The ElectionStrategy interface.</summary>
    public interface IElectionStrategy
    {
        /// <summary>Determines the parent and child and will rearrange the values in the ref parameters to reflect this.</summary>
        /// <param name="parent">The candidate parent when passed in, the definitive parent on return.</param>
        /// <param name="child">The candidate child when passed in, the definitive child on return.</param>
        void DetermineParent(ref INode parent, ref INode child);
    }
}