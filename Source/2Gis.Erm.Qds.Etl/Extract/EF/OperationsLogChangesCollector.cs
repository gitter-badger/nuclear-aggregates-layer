using System;
using System.Collections.Generic;
using System.Linq;

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

            var changes = new List<PboChangeDescriptor>();

            if (fromRecordIdState.RecordId == null)
            {
                var pbo = _finder.FindAll<PerformedBusinessOperation>().OrderByDescending(x => x.Date).FirstOrDefault();
                if (pbo != null)
                {
                    _currentState = new RecordIdState("0", pbo.Id.ToString());
                }

                return changes;
            }

            var fromRecordIdStateParsed = long.Parse(fromRecordIdState.RecordId);
            var operations = _finder.Find<PerformedBusinessOperation>(pbo => pbo.Id > fromRecordIdStateParsed);
            var maxId = fromRecordIdStateParsed;
            foreach (var operation in operations)
            {
                maxId = operation.Id;
                changes.Add(new PboChangeDescriptor(operation));
            }

            _currentState = maxId == fromRecordIdStateParsed ? fromRecordIdState : new RecordIdState("0", maxId.ToString());

            return changes;
        }

        public ITrackState CurrentState
        {
            get { return _currentState; }
        }
    }
}