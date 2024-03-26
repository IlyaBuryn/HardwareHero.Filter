using HardwareHero.Filter.Utils;
using System.Linq.Expressions;

namespace HardwareHero.Filter.RequestsModels
{
    public class GroupByRequestInfo<T> where T : class
    {
        public GroupByRequestInfo()
        {
            InitGroupByExpression();
        }

        public Expression<Func<T, object>>? GroupByExpression { get; private set; }

        public string? PropertyName { get; set; }

        private void InitGroupByExpression()
        {
            if (PropertyName != null && PropertyName != string.Empty)
            {
                GroupByExpression = FilterHelper.GetExpressionFromString<T>(PropertyName);
            }
        }
    }
}
