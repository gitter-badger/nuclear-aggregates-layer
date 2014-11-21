using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport
{
    /// <summary>
    /// Ошибка печати AcceptanceReport
    /// </summary>
    [Serializable]
    public class AcceptanceReportPrintingException : BusinessLogicException
    {
        public AcceptanceReportPrintingException()
        {
        }

        public AcceptanceReportPrintingException(string message) : base(message)
        {
        }

        public AcceptanceReportPrintingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AcceptanceReportPrintingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}