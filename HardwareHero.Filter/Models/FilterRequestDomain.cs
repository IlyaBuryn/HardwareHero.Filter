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

        public PageRequestInfo? PageRequestInfo { get; set; }
        public SortByRequestInfo? SortByRequestInfo { get; set; }
        public GroupByRequestInfo? GroupByRequestInfo { get; set; }


        public virtual T SelectionPattern(T refItem)
        {
            return refItem;
        }

        public virtual IQueryable<T?>? GroupedPattern(IQueryable<IGrouping<object, T?>> groups)
        {
            return groups.SelectMany(x => x).Distinct();
        }

        protected void AddExpression(Expression<Func<T, bool>> expression)
        {
            if (SelectionExpressions != null)
            {
                SelectionExpressions.Add(expression);
            }
        }
    }
}
