using System;
using System.Linq;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class DocsStorageChangesTrackerState : IChangesTrackerState
    {
        readonly IDocsStorage _docsStorage;
        readonly IQueryDsl _queryDsl;
        public const string IdFieldName = "id";
        public const long StateRecordId = 0;

        public DocsStorageChangesTrackerState(IDocsStorage docsStorage, IQueryDsl queryDsl)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _docsStorage = docsStorage;
            _queryDsl = queryDsl;
        }

        public ITrackState GetState()
        {
            return _docsStorage.Find<RecordIdState>(_queryDsl.ByFieldValue(IdFieldName, StateRecordId)).Single();
        }

        public void SetState(ITrackState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            var newState = (RecordIdState)state;
            var currState = GetState() as RecordIdState;

            if (currState!=null && currState.RecordId != newState.RecordId)
            {
                _docsStorage.Update(new[] { newState, });
            }
        }
    }
}