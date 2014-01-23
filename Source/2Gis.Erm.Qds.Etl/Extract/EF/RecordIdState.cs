namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class RecordIdState : ITrackState, IDoc
    {
        public RecordIdState(long id, long recordId)
        {
            Id = id;
            RecordId = recordId;
        }

        public long Id { get; private set; }
        public long RecordId { get; private set; }
    }
}