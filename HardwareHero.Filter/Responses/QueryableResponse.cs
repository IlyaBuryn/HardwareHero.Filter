namespace HardwareHero.Filter.Responses
{
    public class QueryableResponse<T>
    {
        public QueryableResponse(IQueryable<T?>? query, string? message = null, PageResponseInfo? pageResponseInfo = null)
        {
            Query = query;
            Message = message;
            PageResponseInfo = pageResponseInfo;

            IsError = false;
        }

        public QueryableResponse(IQueryable<T?>? query, Exception exception)
        {
            Query = query;
            SetException(exception);
        }

        public void SetException(Exception exception)
        {
            Message = exception.Message;
            IsError = true;
        }

        public IQueryable<T?>? Query { get; set; }
        public string? Message { get; set; }
        public bool IsError { get; set; }
        public PageResponseInfo? PageResponseInfo { get; set; }
    }
}
