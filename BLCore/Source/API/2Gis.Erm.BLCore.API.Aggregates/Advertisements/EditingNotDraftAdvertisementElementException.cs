using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements
{
    /// <summary>
    /// Попытка изменения РМ не в статусе "Черновик"
    /// </summary>
    [Serializable]
    public class EditingNotDraftAdvertisementElementException : BusinessLogicException
    {
        public EditingNotDraftAdvertisementElementException()
        {
        }

        public EditingNotDraftAdvertisementElementException(string message)
            : base(message)
        {
        }

        public EditingNotDraftAdvertisementElementException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected EditingNotDraftAdvertisementElementException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}