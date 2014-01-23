using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class OperationsLogChangesCollector : IChangesCollector
    {
        readonly IFinder _finder;
        RecordIdState _currentState;

        public OperationsLogChangesCollector(IFinder finder)
        {
            if (finder == null)
            {
                throw new ArgumentNullException("finder");
            }

            _finder = finder;
        }

        public IEnumerable<IChangeDescriptor> LoadChanges(ITrackState fromState)
        {
            if (fromState == null)
            {
                throw new ArgumentNullException("fromState");
            }

            var fromRecordIdState = fromState as RecordIdState;
            if (fromRecordIdState == null)
                throw new NotSupportedException(fromState.GetType().FullName);

            var operations = _finder.Find<PerformedBusinessOperation>(pbo => pbo.Id > fromRecordIdState.RecordId);
            var changes = new List<PboChangeDescriptor>();

            var newId = fromRecordIdState.RecordId;
            foreach (var operation in operations)
            {
                newId = operation.Id;
                changes.Add(new PboChangeDescriptor(operation));
            }

            _currentState = newId != fromRecordIdState.RecordId ? new RecordIdState(0, newId) : fromRecordIdState;

            return changes;
        }

        public ITrackState CurrentState
        {
            get { return _currentState; }
        }
    }
}