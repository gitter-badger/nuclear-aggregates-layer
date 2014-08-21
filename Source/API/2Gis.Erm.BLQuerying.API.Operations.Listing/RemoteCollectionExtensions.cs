using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing
{
    public static class RemoteCollectionExtensions
    {
        public static RemoteCollection<T> Transform<T>(this RemoteCollection<T> remoteCollection, Func<T, T> func)
        {
            foreach (var item in remoteCollection)
            {
                // FIXME {m.pashuk, 02.07.2014}: Возвращаемое значение Func<T, T> никак не используется, может стоит использовать Action<T>?
                func(item);
            }

            return remoteCollection;
        }
    }
}