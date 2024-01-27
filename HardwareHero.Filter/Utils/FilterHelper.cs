using System.Linq.Expressions;

namespace HardwareHero.Filter.Utils
{
    internal class FilterHelper
    {
        public static Expression<Func<T, object>>? GetExpressionFromString<T>(string expressionString) where T : class
        {
            var type = typeof(T);
            var property = type.GetProperty(expressionString) ??
                type.GetProperties().SelectMany(x => x.PropertyType.GetProperties())
                .FirstOrDefault(x => x.Name == expressionString);

            if (property == null)
            {
                throw new ArgumentException($"Undefined property: {expressionString}");
            }

            var param = Expression.Parameter(type, "x");
            var body = Expression.Property(param, property);
            var expression = Expression.Lambda<Func<T, object>>(body, param);

            return expression;
        }
    }
}
