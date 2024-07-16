using HardwareHero.Filter.Exceptions;
using HardwareHero.Filter.Operations;
using HardwareHero.Filter.RequestsModels;
using HardwareHero.Filter.Responses;

namespace HardwareHero.Filter.Extensions
{
    public static class QueryableExtensions
    {
        public static QueryableResponse<T> ApplyFilter<T>
            (this IQueryable<T?>? source, IFilterable<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            var predicate = filter.OnGetFilterExpression();
            if (predicate == null)
            {
                return new(source, new FilterException("No expressions for filtering!"));
            }

            var query = source!.Where(predicate);

            return new(query);

        }

        public static QueryableResponse<T> ApplyOrderBy<T>
            (this IQueryable<T?>? source, ISortable<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            var sortBy = filter.OnGetSortExpression(filter.SortByProperty);
            if (sortBy == null)
            {
                return new(source, new FilterException("No expressions for sorting!"));
            }

            var query = filter.SortByDescending ? 
                source!.OrderByDescending(sortBy!) :
                source!.OrderBy(sortBy!);

            return new(query);
        }

        public static PageResponse<T> ApplyPagination<T>
            (this IQueryable<T?>? source, IPaginable filter) where T : class
        {
            CheckFilterAndSource(source, filter);
            IQueryable<T?>? pageQuery;
            uint totalPages = 0;

            try
            {
                pageQuery = source!.Skip((int)((filter.PageNumber - 1) * filter.PageSize)).Take((int)filter.PageSize);
                totalPages = (uint)Math.Ceiling((double)source!.Count() / filter.PageSize);
            }
            catch (Exception ex)
            {
                return new(new(source, new FilterException(ex.Message)), 0, filter.PageNumber, filter.PageSize);
            }

            return new(new(pageQuery), totalPages, filter.PageNumber, filter.PageSize);
        }

        public static GroupResponse<T> ApplyGroupBy<T>
            (this IQueryable<T?>? source, IGroupable<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            var groupBy = filter.OnGetGroupExpression(filter.GroupByProperty);
            if (groupBy == null)
            {
                return new(new FilterException("No expressions for grouping!"));
            }

            var query = source!.GroupBy(groupBy);
            try
            {
                var groupQuery = query.ToGroupQueryable<T>();
                return new(groupQuery);
            }
            catch (Exception ex)
            {
                return new(new FilterException(ex.Message));
            }
        }

        public static QueryableResponse<object> ApplySelection<T>
            (this IQueryable<T?>? source, ISelectable<T> filter) where T : class
        {
            CheckFilterAndSource(source, filter);

            var query = source!.Select(item => filter.SetupSelectFields(item));

            return new(query);
        }

        private static List<GroupItem<T>> ToGroupQueryable<T>(this IQueryable<IGrouping<object, T?>>? query) where T : class
        {
            return query.Select(g => new GroupItem<T>(g.Key.ToString(), g.AsQueryable())).ToList();
        }

        private static void CheckFilterAndSource<T>(IQueryable<T?>? source, IFilterOperation? filter) where T : class
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
