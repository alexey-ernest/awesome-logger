using System;

namespace AwesomeLogger.Web.Exceptions
{
    public class WebException : Exception
    {
        public WebException(string message)
            : base(message)
        {
        }
    }
}