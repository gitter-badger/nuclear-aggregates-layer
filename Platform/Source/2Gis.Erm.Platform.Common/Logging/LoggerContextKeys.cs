namespace DoubleGis.Erm.Platform.Common.Logging
{
    public static class LoggerContextKeys
    {
        public static class Required
        {
            public const string SessionId = "sessionId";
            public const string UserName = "userName";
            public const string UserIP = "userIP";
            public const string UserBrowser = "userBrowser";
            public const string SeanceCode = "seanceCode";
            public const string Module = "moduleName";
        }

        public static class Optional
        {
            public const string MethodName = "methodName";
        }
    }
}