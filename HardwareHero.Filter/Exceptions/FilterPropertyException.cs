namespace HardwareHero.Filter.Exceptions
{
    [Serializable]
    public class FilterPropertyException : Exception
    {
        public FilterPropertyException()
            : base($"Can't use the filter property for this operation.")
        { }

        public FilterPropertyException(string operation)
            : base($"Can't use the filter property for ${operation} operation.")
        { }

        public FilterPropertyException(string operation, string filterProperty)
            : base($"Can't use the ${filterProperty} property for ${operation} operation.")
        { }

    }
}
