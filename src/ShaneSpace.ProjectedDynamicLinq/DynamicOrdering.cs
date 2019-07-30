using System.Linq.Expressions;

namespace ShaneSpace.ProjectedDynamicLinq
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}