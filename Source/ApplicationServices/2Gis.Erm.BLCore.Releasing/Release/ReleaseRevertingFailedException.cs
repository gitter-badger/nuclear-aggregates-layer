using System;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public class ReleaseRevertingFailedException : Exception
    {
        public ReleaseRevertingFailedException(string message, Exception innerException) 
            : base(message, innerException)
        {   
        }
    }
}