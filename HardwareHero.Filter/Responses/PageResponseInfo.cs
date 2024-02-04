using HardwareHero.Filter.RequestsModels;

namespace HardwareHero.Filter.Responses
{
    public class PageResponseInfo
    {
        public PageResponseInfo(PageRequestInfo pageRequestInfo, int totalAvailablePagesCount)
        {
            PageRequestInfo = pageRequestInfo;
            TotalAvailablePagesCount = totalAvailablePagesCount;
        }

        public PageRequestInfo PageRequestInfo { get; set; }
        public int TotalAvailablePagesCount { get; set; }
    }
}
