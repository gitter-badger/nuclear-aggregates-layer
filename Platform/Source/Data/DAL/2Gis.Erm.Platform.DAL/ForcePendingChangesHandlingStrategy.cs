using System;
using System.Runtime.InteropServices;

using NuClear.Storage.Core;

namespace DoubleGis.Erm.Platform.DAL
{
    public class PendingChangesNotHandledException : Exception
    {
        public PendingChangesNotHandledException() : base("IPendingChangesMonitorable object contains not saved pending changes")
        {
        }
    }

    /// <summary>
    /// Реализация стратегии PendingChangesHandling, в которой бросается исключение при наличии отложенных изменений
    /// </summary>
    public class ForcePendingChangesHandlingStrategy : IPendingChangesHandlingStrategy
    {
        public void HandlePendingChanges(IPendingChangesMonitorable pendingChangesMonitorableObject)
        {
            var isInException = Marshal.GetExceptionPointers() != IntPtr.Zero || Marshal.GetExceptionCode() != 0;
            if (isInException)
            {
                return;
            }

            // Если объект содержит несохраненные изменения и ниже по стеку выполенения не было брошено исключение
            // -> бросаем исключение PendingChangesNotHandledException
            if (pendingChangesMonitorableObject.AnyPendingChanges)
            {
                throw new PendingChangesNotHandledException();
            }
        }
    }
}