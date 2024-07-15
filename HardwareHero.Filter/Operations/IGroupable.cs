using System.Linq.Expressions;

namespace HardwareHero.Filter.Operations
{
    public interface IGroupable<T> : IFilterOperation where T : class
    {
        string? GroupByProperty { get; init; }
        void SetupGroupByExpressions();
        Expression<Func<T, object>>? OnGetGroupExpression(string groupByProperty);
    }
}
