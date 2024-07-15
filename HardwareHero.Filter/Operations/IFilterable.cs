using System.Linq.Expressions;

namespace HardwareHero.Filter.Operations
{
    public interface IFilterable<T> : IFilterOperation where T : class
    {
        void SetupFilterExpressions();
        Expression<Func<T, bool>>? OnGetFilterExpression();
    }
}
