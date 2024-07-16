using System.Linq.Expressions;

namespace HardwareHero.Filter.Operations
{
    public interface ISelectable<T> : IFilterOperation where T : class
    {
        object SetupSelectFields(T? item);
    }
}
