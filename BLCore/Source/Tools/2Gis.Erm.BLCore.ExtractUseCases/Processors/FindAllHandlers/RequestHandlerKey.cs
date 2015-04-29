using System;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers
{
    public class RequestHandlerKey : IContextKey<RequestHandlerProcessingResults>
    {
        private static readonly Lazy<RequestHandlerKey> KeyInstance = new Lazy<RequestHandlerKey>();

        public static RequestHandlerKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}