using System;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public static class ExceptionUtils
    {
        public static string ToDecription(this Exception exception)
        {
            return exception.GetType().Name + ". " + exception.Message;
        }
    }
}