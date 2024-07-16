using HardwareHero.Filter.Exceptions;
using HardwareHero.Filter.Operations;
using HardwareHero.Filter.Responses;

namespace HardwareHero.Filter.Extensions
{
    public static class PartialQueryableExtensions
    {
        public static QueryableResponse<T> ApplyFilter<T>
            (this QueryableResponse<T> next, IFilterable<T> filter) where T : class
        {
            if (!next.IsSuccessful)
            {
                next.Errors.Add(new FilterException(nameof(IFilterable<T>), next.Errors.Count - 1));

                return next;
            }

            return next.Query.ApplyFilter(filter);
        }
    }
}
