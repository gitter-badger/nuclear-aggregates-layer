using System;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public static class ExceptionHelper
    {
        public static Boolean RecursiveSearchMessage(this Exception ex, String pattern, out Exception foundException)
        {
            Exception currentEx = ex;
            while (currentEx != null)
            {
                if (currentEx.Message.Contains(pattern))
                {
                    foundException = currentEx;
                    return true;
                }
                currentEx = currentEx.InnerException;
            }

            foundException = null;
            return false;
        }
    }
}
