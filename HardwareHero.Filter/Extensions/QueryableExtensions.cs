using HardwareHero.Filter.Exceptions;
using HardwareHero.Filter.RequestsModels;
using HardwareHero.Filter.Responses;
using HardwareHero.Filter.Utils;

namespace HardwareHero.Filter.Extensions
{
    public static class QueryableExtensions
    {
        public static QueryableResponse<T> ApplyFilter<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            if (filter.GetExpressions() == null || filter.GetExpressions().Count == 0)
            {
                return new(source, new FilterPropertyException(nameof(ApplyFilter), nameof(filter.GetExpressions)));
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
            
            if (filter.SortByRequestInfo == null || filter.SortByRequestInfo.Property == null)
            {
                return new(source, new FilterPropertyException(nameof(ApplyOrderBy), nameof(filter.SortByRequestInfo)));
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

                    source = SortByRequestInfo.SortOrderMatches[order] == SortOrderType.Asc
                        ? source!.OrderBy(expression)
                        : source!.OrderByDescending(expression);

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

            if (filter.GroupByRequestInfo == null || filter.GroupByRequestInfo.PropertyName == null)
            {
                return new(source, new FilterPropertyException(nameof(ApplyGroupBy), nameof(filter.GroupByRequestInfo)));
            }

            try
            {
                var expression = FilterHelper.GetExpressionFromString<T>(filter.GroupByRequestInfo.PropertyName); ;

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

            source = source!.Select(item => item != null ? filter!.SelectionPattern(item) : item);

            return new(source);
        }

        public static QueryableResponse<T> ApplyPagination<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            CheckFilterAndSource(source, filter);

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

            if (source == null || source.Count() == 0)
            {
                throw new NullOrEmptyCollectionException(nameof(source));
            }
        }
    }
}
