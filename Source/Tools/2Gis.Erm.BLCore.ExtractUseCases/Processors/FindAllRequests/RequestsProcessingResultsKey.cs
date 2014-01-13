using System;

using DoubleGis.Erm.Platform.API.Core.UseCases.Context;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests
{
    public class RequestsProcessingResultsKey : IContextKey<RequestsProcessingResults>
    {
        private static readonly Lazy<RequestsProcessingResultsKey> KeyInstance = new Lazy<RequestsProcessingResultsKey>();

        public static RequestsProcessingResultsKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}