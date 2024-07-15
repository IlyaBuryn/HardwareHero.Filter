using HardwareHero.Filter.Exceptions;

namespace HardwareHero.Filter.Responses
{
    public class QueryableResponse<T> where T : class
    {
        public IQueryable<T?>? Query { get; set; }
        public List<FilterException?> Errors { get; set; } = new();
        public bool IsSuccessful { get; set; }

        public QueryableResponse(IQueryable<T?>? query)
        {
            Query = query;
            Errors = new();
            IsSuccessful = true;
        }

        public QueryableResponse(IQueryable<T?>? query, FilterException? exception)
        {
            Query = query;
            Errors.Add(exception);
            IsSuccessful = false;
        }
    }
}
