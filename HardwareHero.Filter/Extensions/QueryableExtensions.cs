using HardwareHero.Filter.Exceptions;
using HardwareHero.Filter.RequestsModels;
using HardwareHero.Filter.Responses;
using HardwareHero.Filter.Utils;
using System.Linq.Expressions;

namespace HardwareHero.Filter.Extensions
{
    public static class QueryableExtensions
    {
        public static QueryableResponse<T> ApplyFilter<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            if (source!.Any() == false) 
            {
                return new(source, "Collection was empty!");
            }

            var expressions = filter.GetExpressions();

            if (expressions == null || expressions.Count() == 0)
            {
                return new(source, new FilterPropertyException(nameof(ApplyFilter), nameof(expressions)).Message);
            }

            try
            {
                var combined = expressions[0];
                var param = combined?.Parameters[0];

                if (param == null)
                {
                    return new(source, new Exception("Unable to get the parameter of the expression!"));
                }

                for (int i = 1; i < expressions.Count; i++)
                {
                    var expression = expressions[i];
                    if (expression != null)
                    {
                        var visitor = new ReplaceExpressionVisitor(expression.Parameters[0], param);
                        var newExp = visitor.Visit(expression) as Expression<Func<T, bool>>;
                        var body = Expression.AndAlso(combined!.Body, newExp.Body);
                        combined = Expression.Lambda<Func<T, bool>>(body, param);
                    }
                }

                source = source!.Where(combined!);

                return new(source);
            }
            catch (Exception ex)
            {
                return new(source, ex);
            }
        }

        public static QueryableResponse<T> ApplyOrderBy<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            if (source!.Any() == false)
            {
                return new(source, "Collection was empty!");
            }

            var orderByInfo = filter.SortByRequestInfo;

            if (orderByInfo == null || orderByInfo.PropertyName == null)
            {
                return new(source, new FilterPropertyException(nameof(ApplyOrderBy), nameof(orderByInfo)).Message);
            }

            try
            {
                var expression = FilterHelper.GetExpressionFromString<T>(orderByInfo.PropertyName);

                if (expression != null)
                {
                    var orderType = orderByInfo.CastToEnumSortOrderType();

                    source = orderType == SortOrderType.Asc
                        ? source!.OrderBy(expression!)
                        : source!.OrderByDescending(expression!);

                    return new(source);
                }
                else
                {
                    return new(source, "OrderBy expression was null");
                }
            }
            catch (Exception ex)
            {
                return new(source, ex);
            }
        }

        public static QueryableResponse<T> ApplyGroupBy<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            if (source!.Any() == false)
            {
                return new(source, "Collection was empty!");
            }

            var groupByInfo = filter.GroupByRequestInfo;

            if (groupByInfo == null || groupByInfo.PropertyName == null)
            {
                return new(source, new FilterPropertyException(nameof(ApplyGroupBy), nameof(groupByInfo)).Message);
            }

            try
            {
                var expression = FilterHelper.GetExpressionFromString<T>(groupByInfo.PropertyName); ;

                if (expression != null)
                {
                    var groupedQuery = source!.GroupBy(expression!);
                    source = filter.GroupedPattern(groupedQuery);
                    
                    return new(source);
                }
                else
                {
                    return new(source, "GroupBy expression was null");
                }
            }
            catch (Exception ex)
            {
                return new(source, ex);
            }
        }

        public static QueryableResponse<T> ApplySelection<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            if (source!.Any() == false)
            {
                return new(source, "Collection was empty!");
            }

            source = source!.Select(item => item != null ? filter!.SelectionPattern(item) : item);

            return new(source);
        }

        public static QueryableResponse<T> ApplyPagination<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            if (source!.Any() == false)
            {
                return new(source, "Collection was empty!");
            }

            if (filter!.PageRequestInfo == null)
            {
                return new (source, new FilterPropertyException(nameof(ApplyPagination), nameof(filter.PageRequestInfo)));
            }

            int skip = (filter.PageRequestInfo.PageNumber - 1) * filter.PageRequestInfo.PageSize;
            IQueryable<T?> result = source!.Skip(skip).Take(filter.PageRequestInfo.PageSize);
            var totalPageCount = (int)Math.Ceiling((double)source!.Count() / filter.PageRequestInfo.PageSize);

            return new (result, null, new PageResponseInfo(filter.PageRequestInfo, totalPageCount));
        }

        private static void CheckFilterAndSource<T>(IQueryable<T?>? source, FilterRequestDomain<T>? filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (source == null)
            {
                throw new NullOrEmptyCollectionException(nameof(source));
            }
        }

        private static bool IsSourceEmpty<T>(IQueryable<T?> source) => source.Count() == 0;
    }
}
