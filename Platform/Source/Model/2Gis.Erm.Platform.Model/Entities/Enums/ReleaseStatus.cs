namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum ReleaseStatus : short
    {
        None = 0,

        Success = 2,
        Error = 3,
        Reverted = 4,

        InProgressInternalProcessingStarted = 6,
        InProgressWaitingExternalProcessing = 7,

        Reverting = 8
    }
}