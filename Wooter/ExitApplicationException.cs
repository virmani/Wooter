using System;

namespace Wooter
{
    public class ExitApplicationException : Exception
    {
        public ExitApplicationException(string message, Exception ex)
            : base("ExitApplicationException occured" + message, ex)
        {

        }

        public ExitApplicationException(string message)
            : base("ExitApplicationException occured: " + message)
        {

        }
    }
}
