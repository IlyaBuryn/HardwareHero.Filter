namespace HardwareHero.Filter.Exceptions
{
    [Serializable]
    public class NullOrEmptyCollectionException : Exception
    {
        public NullOrEmptyCollectionException()
            : base($"Collection was null or empty!")
        { }

        public NullOrEmptyCollectionException(string entity)
            : base($"{entity} - was null or empty!")
        { }
    }
}
