using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public interface IChangesCollector
    {
        IEnumerable<IChangeDescriptor> LoadChanges(ITrackState fromState);
        ITrackState CurrentState { get; }
    }
}