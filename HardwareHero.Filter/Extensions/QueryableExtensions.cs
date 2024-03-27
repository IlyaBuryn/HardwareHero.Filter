using HardwareHero.Filter.Exceptions;
using HardwareHero.Filter.RequestsModels;
using HardwareHero.Filter.Responses;

namespace HardwareHero.Filter.Extensions
{
    public static class QueryableExtensions
    {
        public static QueryableResponse<T> ApplyFilter<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            filter.SetFilterExpression();

            var expression = filter.FilterExpression;

            if (expression == null)
            {
                return new(source, new FilterException("No expressions for filtering!"));
            }

            try
            {
                source = source!.Where(expression!);

                return new(source);
            }
            catch (Exception ex)
            {
                return new(source, new FilterException(ex.Message));
            }
        }

        public static QueryableResponse<T> ApplyOrderBy<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            var orderByInfo = filter.SortByRequestInfo;

            if (orderByInfo == null)
            {
                return new(source, new FilterException("No expressions for sorting!"));
            }

            orderByInfo.InitOrderByExpression();

            try
            {
                var expression = orderByInfo.OrderByExpression;

                if (expression != null)
                {
                    source = orderByInfo.OrderByDescending
                        ? source!.OrderByDescending(expression!)
                        : source!.OrderBy(expression!);

                    return new(source);
                }
                else
                {
                    return new(source, new FilterException("An error occurred while creating the expression!"));
                }
            }
            catch (Exception ex)
            {
                return new(source, new FilterException(ex.Message));
            }
        }

        public static QueryableResponse<T> ApplyGroupBy<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            var groupByInfo = filter.GroupByRequestInfo;

            if (groupByInfo == null)
            {
                return new(source, new FilterException("No expressions for grouping!"));
            }

            groupByInfo.InitGroupByExpression();

            try
            {
                var expression = groupByInfo.GroupByExpression;

                if (expression != null)
                {
                    var groupedQuery = source!.GroupBy(expression!);
                    if (filter!.GroupByTransformation == null)
                    {
                        source = groupedQuery.SelectMany(g => g);
                    }
                    else
                    {
                        source = filter!.GroupByTransformation.Invoke(groupedQuery);
                    }
                                        
                    return new(source);
                }
                else
                {
                    return new(source, new FilterException("An error occurred while creating the expression!"));
                }
            }
            catch (Exception ex)
            {
                return new(source, new FilterException(ex.Message));
            }
        }

        public static QueryableResponse<T> ApplySelection<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            if (filter.TransformationPattern == null)
            {
                return new(source, new FilterException($"No delegate in `{nameof(filter.TransformationPattern)}`!"));
            }

            source = source!.Select(item => filter!.TransformationPattern.Invoke(item));

            return new(source);
        }

        public static QueryableResponse<T> ApplyPagination<T>
            (this IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            if (source!.Any() == false)
            {
                return new(source, new FilterException("Collection was empty!"));
            }

            var page = filter!.PageRequestInfo;

            if (page == null)
            {
                return new (source, new FilterException("No page info!"));
            }

            int skip = (page.PageNumber - 1) * page.PageSize;
            IQueryable<T?> result = source!.Skip(skip).Take(page.PageSize);
            var totalPageCount = (int)Math.Ceiling((double)source!.Count() / page.PageSize);

            return new(null, new PageResponseInfo(page, totalPageCount));
        }

        private static void CheckFilterAndSource<T>(IQueryable<T?>? source, FilterRequestDomain<T>? filter) where T : class
        {
            if (filter == null)
            {
                throw new FilterException(typeof(FilterRequestDomain<T>));
            }

            if (source == null)
            {
                throw new FilterException(typeof(IQueryable<T?>));
            }

            if (source!.Any() == false)
            {
                throw new FilterException("Collection was empty!");
            }
        }
    }
}
