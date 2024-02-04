using HardwareHero.Filter.RequestsModels;
using HardwareHero.Filter.Options;

namespace HardwareHero.Filter.Extensions
{
    public static class FilterExtensions
    {
        public static void ApplyPageSizeOptions<T>(this FilterRequestDomain<T> filter, IPageSizeOptions options)
        {
            if (filter.PageRequestInfo == null)
            {
                return;
            }

            if (filter.PageRequestInfo.PageSize <= 0)
            {
                filter.PageRequestInfo.PageSize = options.PageSize;
                filter.PageRequestInfo.PageNumber = options.DefaultPageNumber;
            }
        }
    }
}
