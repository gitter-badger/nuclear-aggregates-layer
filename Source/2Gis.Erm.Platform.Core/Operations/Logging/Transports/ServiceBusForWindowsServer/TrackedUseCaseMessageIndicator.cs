using System;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public static class TrackedUseCaseMessageProperties
    {
        public static class Indicator
        {
            public const string Name = "TrackedUseCaseMessageIndicator";
            public readonly static Guid Value = new Guid("A68C1063-2F31-4F43-B069-F24139CDC982");
        }

        public static class Names
        {
            public const string FormatVersion = "FormatVersion";
            public const string MessageBodyType = "MessageBodyType";
            public const string Operation = "UseCaseOperation";
            public const string EntitiesSetHash = "EntitiesSetHash";
            public const string UseCaseId = "UseCaseId";
        }
    }
}