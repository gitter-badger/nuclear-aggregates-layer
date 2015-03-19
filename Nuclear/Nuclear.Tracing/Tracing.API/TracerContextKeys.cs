namespace Nuclear.Tracing.API
{
    public static class TracerContextKeys
    {
        public static class Required
        {
            public const string Environment = "environment";
            public const string EntryPoint = "entryPoint";
            public const string EntryPointHost = "entryPointHost";
            public const string EntryPointInstanceId = "entryPointInstanceId";
            public const string UserAccount = "userAccount";
        }

        public static class Optional
        {
            public const string UserSession = "userSession";
            public const string UserAddress = "userAddress";
            public const string UserAgent = "userAgent";
        }
    }
}