namespace HardwareHero.Filter.RequestsModels
{
    public enum SortOrderType
    {
        Asc = 0, Desc = 1
    }

    public class SortByRequestInfo
    {
        public static Dictionary<string, SortOrderType> SortOrderMatches { get; } = new()
        {
            { "asc", SortOrderType.Asc },
            { "0", SortOrderType.Asc },
            { "desc", SortOrderType.Desc },
            { "1", SortOrderType.Desc },
        };

        public SortByRequestInfo() { }

        public string? Property { get; init; }
        public string? SortOrder { get; init; } = "asc";
    }
}
