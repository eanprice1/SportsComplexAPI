namespace SportsComplex.Logic.Exceptions
{
    public class DbWriteEntityException : Exception
    {
        public DbWriteEntityException() : base()
        {
        }

        public DbWriteEntityException(string message) : base(message)
        {
        }

        public DbWriteEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
