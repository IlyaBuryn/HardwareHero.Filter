using System.Linq.Expressions;

namespace HardwareHero.Filter.Models
{
    public abstract class FilterRequestDomain<T>
    {
        public FilterRequestDomain()
        {
            SelectionExpressions = new();
        }

        public List<Expression<Func<T, bool>>?> SelectionExpressions { get; init; }
        public PageRequestInfo? PageRequestInfo { get; init; }
        public SortByRequestInfo? SortByRequestInfo { get; init; }
        public GroupByRequestInfo? GroupByRequestInfo { get; init; }


        public virtual T SelectionPattern(T refItem)
        {
            return refItem;
        }

        public virtual IQueryable<T?>? GroupedPattern(IQueryable<IGrouping<object, T?>> groups)
        {
            return new List<T?>().AsQueryable();
        }

        protected void AddExpression(Expression<Func<T, bool>> expression)
        {
            if (SelectionExpressions != null && SelectionExpressions.Count != 0)
            {
                SelectionExpressions.Add(expression);
            }
        }
    }
}
