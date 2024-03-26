using HardwareHero.Filter.Utils;
using System.Linq.Expressions;

namespace HardwareHero.Filter.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
            var leftExpression = leftVisitor.Visit(left.Body);
            var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
            var rightExpression = rightVisitor.Visit(right.Body);
            var andExpression = Expression.AndAlso(leftExpression, rightExpression);
            return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
        }
    }
}
