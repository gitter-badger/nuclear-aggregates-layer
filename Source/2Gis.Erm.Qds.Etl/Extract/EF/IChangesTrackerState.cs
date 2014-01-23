namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public interface IChangesTrackerState
    {
        ITrackState GetState();
        void SetState(ITrackState state);
    }
}