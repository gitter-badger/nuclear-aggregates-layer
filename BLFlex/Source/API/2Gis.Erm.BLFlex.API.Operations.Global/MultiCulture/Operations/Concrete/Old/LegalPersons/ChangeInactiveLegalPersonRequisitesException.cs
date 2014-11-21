using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Old.LegalPersons
{
    [Serializable]
    public class ChangeInactiveLegalPersonRequisitesException : BusinessLogicException
    {
        public ChangeInactiveLegalPersonRequisitesException()
        {
        }

        public ChangeInactiveLegalPersonRequisitesException(string message)
            : base(message)
        {
        }

        public ChangeInactiveLegalPersonRequisitesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ChangeInactiveLegalPersonRequisitesException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}