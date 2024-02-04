namespace HardwareHero.Filter.Exceptions
{
    [Serializable]
    public class NullOrEmptyCollectionException : Exception
    {
        public NullOrEmptyCollectionException()
            : base($"Collection was null or empty!")
        { }

        public NullOrEmptyCollectionException(string collection)
            : base($"{collection} - was null or empty!")
        { }
    }
}
