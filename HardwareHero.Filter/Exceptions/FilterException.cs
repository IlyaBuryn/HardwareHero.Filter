using HardwareHero.Filter.Operations;
using HardwareHero.Filter.Responses;

namespace HardwareHero.Filter.Exceptions
{
    [Serializable]
    public class FilterException : Exception
    {
        public FilterException(Type type)
            : base($"{type.Name} not exist!") { }

        public FilterException(string message)
            : base(message) { }

        public FilterException(string operation, int index)
            : base($"{index}: Can't move to {operation}!") { }
    }
}
