using HardwareHero.Filter.RequestsModels;

namespace HardwareHero.Filter.Responses
{
    public class PageResponse<T> where T : class
    {
        public QueryableResponse<T> QueryableResponse { get; set; }
        public uint TotalPages { get; set; }
        public uint CurrentPageNumber { get; set; }
        public uint CurrentPageSize { get; set; }

        public PageResponse(
            QueryableResponse<T> queryableResponse,
            uint totalPages, uint currentPageNumber, uint currentPageSize)
        {
            QueryableResponse = queryableResponse;
            TotalPages = totalPages;
            CurrentPageNumber = currentPageNumber;
            CurrentPageSize = currentPageSize;
        }
    }
}
