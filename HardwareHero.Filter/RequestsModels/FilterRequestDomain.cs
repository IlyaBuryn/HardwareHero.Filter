using System.Linq.Expressions;

namespace HardwareHero.Filter.RequestsModels
{
    public abstract class FilterRequestDomain<T> where T : class
    {
        protected Dictionary<string, Expression<Func<T, object>>?> SortByExpressions { get; set; }
        protected Dictionary<string, Expression<Func<T, object>>?> GroupByExpressions { get; set; }
        protected Dictionary<string, Expression<Func<T, bool>>?> FilterExpressions { get; set; }

        protected FilterRequestDomain()
        {
            SortByExpressions = new Dictionary<string, Expression<Func<T, object>>?>();
            GroupByExpressions = new Dictionary<string, Expression<Func<T, object>>?>();
            FilterExpressions = new Dictionary<string, Expression<Func<T, bool>>?>();
        }

        public virtual Expression<Func<T, object>>? GetSortExpression(string sortByProperty)
        {
            return SortByExpressions.ContainsKey(sortByProperty) ? SortByExpressions[sortByProperty] : null;
        }

        public virtual Expression<Func<T, bool>>? GetFilterExpression(string filterByProperty)
        {
            return FilterExpressions.ContainsKey(filterByProperty) ? FilterExpressions[filterByProperty] : null;
        }

        public virtual Expression<Func<T, object>>? GetGroupExpression(string groupByProperty)
        {
            return GroupByExpressions.ContainsKey(groupByProperty) ? GroupByExpressions[groupByProperty] : null;
        }
    }
}
