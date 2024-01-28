using System.Linq.Expressions;

namespace HardwareHero.Filter.Utils
{
    internal class FilterHelper
    {
        public static Expression<Func<T, object>>? GetExpressionFromString<T>(string expressionString) 
            where T : class
        {
            var type = typeof(T);

            var param = Expression.Parameter(type, "x");

            var parts = expressionString.Split('.');
            Expression propertyExpression = param;

            foreach (var part in parts)
            {
                propertyExpression = Expression.PropertyOrField(propertyExpression, part);
            }

            var expression = Expression.Lambda<Func<T, object>>(
                Expression.Convert(propertyExpression, typeof(object)), param);

            return expression;
        }
    }
}
