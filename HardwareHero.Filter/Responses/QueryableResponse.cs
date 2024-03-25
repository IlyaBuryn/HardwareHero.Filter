namespace HardwareHero.Filter.Responses
{
    public class QueryableResponse<T>
    {
        public IQueryable<T?>? Query { get; set; }
        public string? WarningMessage { get; set; }
        public Exception? Error { get; set; }
        public PageResponseInfo? PageResponseInfo { get; set; }

        public QueryableResponse(IQueryable<T?>? query, string? message = null, PageResponseInfo? pageResponseInfo = null)
        {
            Query = query;
            WarningMessage = message;
            PageResponseInfo = pageResponseInfo;
        }

        public QueryableResponse(IQueryable<T?>? query, Exception exception)
        {
            Query = query;
            Error = exception;
        }

        public IQueryable<T?>? CatchQuery()
        {
            if (Error != null)
            {
                throw Error;
            }

            return Query;
        }
    }
}
