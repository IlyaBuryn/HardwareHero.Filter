using HardwareHero.Filter.Utils;
using System.Linq.Expressions;

namespace HardwareHero.Filter.RequestsModels
{
    public class SortByRequestInfo<T> where T : class
    {
        public SortByRequestInfo() { }

        internal Expression<Func<T, object>>? OrderByExpression { get; private set; }

        public string? PropertyName { get; init; }
        public bool OrderByDescending { get; init; } = true;

        internal void InitOrderByExpression()
        {
            if (PropertyName != null && PropertyName != string.Empty)
            {
                OrderByExpression = FilterHelper.GetExpressionFromString<T>(PropertyName);
            }
        }
    }
}
