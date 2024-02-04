using System.Linq.Expressions;

namespace HardwareHero.Filter.RequestsModels
{
    public abstract class FilterRequestDomain<T>
    {
        public PageRequestInfo? PageRequestInfo { get; set; } = new();
        public SortByRequestInfo? SortByRequestInfo { get; set; }
        public GroupByRequestInfo? GroupByRequestInfo { get; set; }



        private readonly List<Expression<Func<T, bool>>?> SelectionExpressions;

        public FilterRequestDomain()
        {
            SelectionExpressions = new();
        }


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

        internal protected List<Expression<Func<T, bool>>?> GetExpressions() => SelectionExpressions;
    }
}
