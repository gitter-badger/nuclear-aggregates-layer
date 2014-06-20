using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport
{
    /// <summary>
    /// Возникает при печати AcceptanceReport по заказам, для которых не указан профиль юр. лица
    /// </summary>
    [Serializable]
    public class ThereIsNoOrdersToPrintException : AcceptanceReportPrintingException
    {
        public ThereIsNoOrdersToPrintException()
        {
        }

        public ThereIsNoOrdersToPrintException(string message) : base(message)
        {
        }

        public ThereIsNoOrdersToPrintException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ThereIsNoOrdersToPrintException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}