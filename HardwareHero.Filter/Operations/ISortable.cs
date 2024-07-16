using System.Linq.Expressions;

namespace HardwareHero.Filter.Operations
{
    public interface ISortable<T> : IFilterOperation where T : class
    {
        string? SortByProperty { get; init; }
        bool SortByDescending { get; init; }
        void SetupSortByExpressions();
        Expression<Func<T, object>>? OnGetSortExpression(string? sortByProperty);
    }
}
