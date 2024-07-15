namespace HardwareHero.Filter.Operations
{
    public interface IPaginable : IFilterOperation
    {
        uint PageNumber { get; init; }
        uint PageSize { get; init; }
    }
}
