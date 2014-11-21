using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport
{
    /// <summary>
    /// Возникает при печати AcceptanceReport по заказу, для которого не указан профиль юр. лица
    /// </summary>
    [Serializable]
    public class OrderToPrintAcceptanceReportDoesntHaveSpecifiedProfileException : AcceptanceReportPrintingException
    {
        public OrderToPrintAcceptanceReportDoesntHaveSpecifiedProfileException()
        {
        }

        public OrderToPrintAcceptanceReportDoesntHaveSpecifiedProfileException(string message) : base(message)
        {
        }

        public OrderToPrintAcceptanceReportDoesntHaveSpecifiedProfileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OrderToPrintAcceptanceReportDoesntHaveSpecifiedProfileException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}