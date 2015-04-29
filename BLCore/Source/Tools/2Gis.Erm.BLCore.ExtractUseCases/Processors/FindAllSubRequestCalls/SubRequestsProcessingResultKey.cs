using System;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllSubRequestCalls
{
    public class SubRequestsProcessingResultKey : IContextKey<SubRequestsProcessingResult>
    {
        private static readonly Lazy<SubRequestsProcessingResultKey> KeyInstance = new Lazy<SubRequestsProcessingResultKey>();

        public static SubRequestsProcessingResultKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}