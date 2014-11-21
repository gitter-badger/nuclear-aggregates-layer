using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport
{
    /// <summary>
    /// Возникает при печати AcceptanceReport по заказам, для которых не указан профиль юр. лица
    /// </summary>
    [Serializable]
    public class SomeOrdersToPrintAcceptanceReportDontHaveSpecifiedProfileException : AcceptanceReportPrintingException
    {
        public SomeOrdersToPrintAcceptanceReportDontHaveSpecifiedProfileException()
        {
        }

        public SomeOrdersToPrintAcceptanceReportDontHaveSpecifiedProfileException(string message) : base(message)
        {
        }

        public SomeOrdersToPrintAcceptanceReportDontHaveSpecifiedProfileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SomeOrdersToPrintAcceptanceReportDontHaveSpecifiedProfileException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}