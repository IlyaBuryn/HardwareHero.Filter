using HardwareHero.Filter.Extensions;
using System.Linq.Expressions;

namespace HardwareHero.Filter.RequestsModels
{
    public abstract class FilterRequestDomain<T> where T : class
    {
        public PageRequestInfo? PageRequestInfo { get; set; }
        public SortByRequestInfo<T>? SortByRequestInfo { get; set; }
        public GroupByRequestInfo<T>? GroupByRequestInfo { get; set; }

        internal Func<T?, T?>? TransformationPattern { get; set; }
        internal Func<IQueryable<IGrouping<object, T?>>?, IQueryable<T>?>? GroupByTransformation { get; set; }
        internal Expression<Func<T, bool>>? FilterExpression { get; set; }

        public FilterRequestDomain() { }

        public virtual void SetFilterExpression() { }

        protected void AddTransformationPattern(Func<T?, T?> pattern)
        {
            TransformationPattern = pattern;
        }

        protected void AddGroupByTransformationPattern(Func<IQueryable<IGrouping<object, T?>>?, IQueryable<T>?>? pattern)
        {
            GroupByTransformation = pattern;
        }

        protected void AddFilterCondition(Expression<Func<T, bool>> condition)
        {
            if (FilterExpression == null)
            {
                FilterExpression = condition;
            }
            else
            {
                FilterExpression = FilterExpression.And(condition);
            }
        }
    }
}
