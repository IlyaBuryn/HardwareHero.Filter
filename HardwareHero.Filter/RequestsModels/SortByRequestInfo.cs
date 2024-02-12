namespace HardwareHero.Filter.RequestsModels
{
    public enum SortOrderType
    {
        Asc = 0, Desc = 1
    }

    public class SortByRequestInfo
    {
        public SortByRequestInfo() { }

        public string? PropertyName { get; init; }
        public string? SortOrder { get; init; } = "asc";

        public SortOrderType CastToEnumSortOrderType()
        {
            var sortOrderTypeString = SortOrder?.ToLower();

            Dictionary<string, SortOrderType> sortOrderMatches = new()
            {
                { "asc", SortOrderType.Asc },
                { "0", SortOrderType.Asc },

                { "desc", SortOrderType.Desc },
                { "1", SortOrderType.Desc },
            };

            if (sortOrderTypeString == null || !sortOrderMatches.ContainsKey(sortOrderTypeString))
            {
                return SortOrderType.Asc;
            }

            return sortOrderMatches[sortOrderTypeString];
        }
    }
}
