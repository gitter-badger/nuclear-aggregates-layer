using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public interface IRemoteCollection : ICollection
    {
        int TotalCount { get; }
    }

    public sealed class RemoteCollection<T> : Collection<T>, IRemoteCollection
    {
        public RemoteCollection(IList<T> list, int totalCount)
            : base(list)
        {
            TotalCount = totalCount;
        }

        public int TotalCount { get; private set; }
    }
}
