using HardwareHero.Filter.Extensions;
using System.Linq.Expressions;

namespace HardwareHero.Filter.RequestsModels
{
    public abstract class FilterRequestDomain<T> where T : class
    {
        private Expression<Func<T, bool>>? _filterExpression;
        private Expression<Func<IGrouping<object, T?>, IQueryable<T>>>? _groupByTransformation;

        public PageRequestInfo? PageRequestInfo { get; set; }
        public SortByRequestInfo<T>? SortByRequestInfo { get; set; }
        public GroupByRequestInfo<T>? GroupByRequestInfo { get; set; }

        public Func<T?, T?> TransformationPattern { get; set; }

        public FilterRequestDomain() { }

        public virtual void SetFilterExpression() { }

        protected void AddFilterCondition(Expression<Func<T, bool>> condition)
        {
            if (_filterExpression == null)
            {
                _filterExpression = condition;
            }
            else
            {
                _filterExpression = _filterExpression.And(condition);
            }
        }

        protected void AddGroupByTransformation(Expression<Func<IGrouping<object, T?>, IQueryable<T>>> condition)
        {
            _groupByTransformation = condition;
        }
        
        internal protected Expression<Func<T, bool>>? GetFilterExpression() => 
            _filterExpression;

        internal protected Expression<Func<IGrouping<object, T?>, IQueryable<T>>>? GetGroupByTransformation() =>
            _groupByTransformation;
    }
}
