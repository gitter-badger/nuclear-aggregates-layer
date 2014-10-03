using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    [Serializable]
    public class AdvertisingCampaignPeriodException : BusinessLogicException
    {
        public AdvertisingCampaignPeriodException()
        {
        }

        public AdvertisingCampaignPeriodException(string message)
            : base(message)
        {
        }

        public AdvertisingCampaignPeriodException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AdvertisingCampaignPeriodException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
