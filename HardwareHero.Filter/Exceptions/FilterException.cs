namespace HardwareHero.Filter.Exceptions
{
    [Serializable]
    public class FilterException : Exception
    {
        public FilterException(Type type)
            : base($"{type.Name} not exist!") { }

        public FilterException(string message)
            : base(message) { }
    }
}
