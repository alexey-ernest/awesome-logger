namespace AwesomeLogger.Web.Exceptions
{
    public class NotFoundException : WebException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}