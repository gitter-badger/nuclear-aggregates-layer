using System;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class DocsStorageChangesTrackerState : IChangesTrackerState
    {
        readonly IDocsStorage _docsStorage;
        public const string StateRecordId = "0";

        public DocsStorageChangesTrackerState(IDocsStorage docsStorage)
        {
            if (docsStorage == null)
            {
                throw new ArgumentNullException("docsStorage");
            }

            _docsStorage = docsStorage;
        }

        public ITrackState GetState()
        {
            var document = _docsStorage.GetById<RecordIdState>(StateRecordId);
            return document ?? new RecordIdState(null, null);
        }

        public void SetState(ITrackState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            var newState = (RecordIdState)state;
            var currState = GetState() as RecordIdState;

            if (currState != null && currState.RecordId != newState.RecordId)
            {
                _docsStorage.Update(new[] { newState });
            }
        }
    }
}