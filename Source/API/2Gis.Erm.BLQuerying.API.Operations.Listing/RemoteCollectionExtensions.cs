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
                func(item);
            }

            return remoteCollection;
        }
    }
}