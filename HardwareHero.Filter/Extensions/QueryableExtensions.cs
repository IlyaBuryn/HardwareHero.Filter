using HardwareHero.Filter.Exceptions;
using HardwareHero.Filter.Models;
using HardwareHero.Filter.Utils;

namespace HardwareHero.Filter.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T?>? ApplyFilter<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            if (filter.GetExpressions() == null || filter.GetExpressions().Count == 0)
            {
                return source;
            }

            try
            {
                foreach (var expression in filter.GetExpressions())
                {
                    if (expression != null)
                    {
                        source = source!.Where(expression);
                    }

                }

                return source;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static IQueryable<T?>? ApplyOrderBy<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            
            if (filter.SortByRequestInfo == null || filter.SortByRequestInfo.Property == null)
            {
                return source;
            }

            try
            {
                var expression = FilterHelper.GetExpressionFromString<T>(filter.SortByRequestInfo.Property);

                if (expression != null)
                {
                    var tmp = SortByRequestInfo.SortOrderMatches.First().Key;
                    var order = filter.SortByRequestInfo.SortOrder?.ToLower() ?? tmp;
                    if (!SortByRequestInfo.SortOrderMatches.ContainsKey(order))
                    {
                        order = tmp;
                    }

                    return SortByRequestInfo.SortOrderMatches[order] == SortOrderType.Asc
                        ? source!.OrderBy(expression)
                        : source!.OrderByDescending(expression);
                }

                return source;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static IQueryable<T?>? ApplyGroupBy<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            if (filter.GroupByRequestInfo == null || filter.GroupByRequestInfo.PropertyName == null)
            {
                return source;
            }

            try
            {
                var expression = FilterHelper.GetExpressionFromString<T>(filter.GroupByRequestInfo.PropertyName); ;

                if (expression != null)
                {
                    var groupedQuery = source!.GroupBy(expression!);
                    return filter.GroupedPattern(groupedQuery);
                }

                return source;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static IQueryable<T?>? ApplySelection<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            source = source!.Select(item => item != null ? filter!.SelectionPattern(item) : item);

            return source;
        }

        public static (IQueryable<T?>?, int) ApplyPagination<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            if (filter!.PageRequestInfo == null)
            {
                return (source, -1);
            }

            int skip = (filter.PageRequestInfo.PageNumber - 1) * filter.PageRequestInfo.PageSize;
            IQueryable<T?> result = source!.Skip(skip).Take(filter.PageRequestInfo.PageSize);
            var totalPageCount = (int)Math.Ceiling((double)source!.Count() / filter.PageRequestInfo.PageSize);

            return (result, totalPageCount);
        }

        private static void CheckFilterAndSource<T>(IQueryable<T?>? source, FilterRequestDomain<T>? filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (source == null || source.Count() == 0)
            {
                throw new NullOrEmptyCollectionException(nameof(source));
            }
        }
    }
}
