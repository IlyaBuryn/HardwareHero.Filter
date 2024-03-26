using HardwareHero.Filter.Exceptions;

namespace HardwareHero.Filter.Responses
{
    public class QueryableResponse<T>
    {
        public IQueryable<T?>? Query { get; set; }
        public FilterException? Error { get; set; }
        public PageResponseInfo? PageResponseInfo { get; set; }

        public QueryableResponse(IQueryable<T?>? query, PageResponseInfo? pageResponseInfo = null)
        {
            Query = query;
            PageResponseInfo = pageResponseInfo;
        }

        public QueryableResponse(IQueryable<T?>? query, FilterException? exception)
        {
            Query = query;
            Error = exception;
        }
    }
}
