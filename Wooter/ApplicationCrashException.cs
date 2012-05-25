using System;

namespace Wooter
{
    public class ApplicationCrashException : Exception
    {
        public ApplicationCrashException(string message, Exception ex)
            : base("ApplicationCrashException occured" + message, ex)
        {

        }

        public ApplicationCrashException()
            : base("ApplicationCrashException occured: No inner exception")
        {

        }
    }
}
