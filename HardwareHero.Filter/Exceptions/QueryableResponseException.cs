namespace HardwareHero.Filter.Exceptions
{
    [Serializable]
    public class QueryableResponseException : Exception
    {
        public QueryableResponseException(string message)
            : base(message) { }
    }
}
