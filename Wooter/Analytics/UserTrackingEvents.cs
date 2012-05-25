namespace Wooter.Analytics
{
    public static class UserTrackingEvents
    {
        public const string ExitApplicationException = "ExitApplicationException Occured";
        public const string ApplicationCrashException = "ApplicationCrashException Occured";
        public const string UnexpectedException = "UnexpectedException Occured";

        public const string ApplicationLoaded = "Application Loaded";
        public const string RefreshPressed = "Refresh Button Pressed";
        public const string IWantOnePressed = "I Want One Pressed";
        public const string AdditionalPhotosPressed = "Additional Photos Pressed";
        public const string DocTypeFound = "DocType was found";

        public const string NetworkNotAvailable = "Network Not Available";
        public const string TimedOutWaitingForWebResponse = "Timed Out Waiting For Web Response";
        public const string TimedOutWaitingForRender = "Timed Out Waiting For Render";

        public const int SubStringLength = 100;

        public static class AttributeNames
        {
            public const string ExceptionType = "ExceptionType";
            public const string ExceptionMessage = "ExceptionMessage";
            public const string StackTrace = "StackTrace";
            public const string InnerExceptionType = "InnerExceptionType";
            public const string InnerExceptionMessage = "InnerExceptionMessage";
            public const string InnerStackTrace = "InnerStackTrace";

            public const string TotalLoadTime = "ApplicationLoadTime";

            public const string Device = "Device";
            public const string DeviceId = "DeviceId";
            public const string DeviceType = "DeviceType";
            public const string DeviceManufacturer = "DeviceManufacturer";
            public const string OsVersion = "OsVersion";
            public const string ApplicationVersion = "ApplicationVersion";

            public const string WootType = "WootType";
            public const string WootUrl = "WootUrl";
            public const string ResponseLine = "ResponseLine";
        }
    }
}
