namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class RecordIdState : ITrackState, IDoc
    {
        public RecordIdState(string id, string recordId)
        {
            Id = id;
            RecordId = recordId;
        }

        public string Id { get; set; }
        public string RecordId { get; private set; }
    }
}