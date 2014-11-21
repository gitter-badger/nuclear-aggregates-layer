namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum LocalMessageStatus
    {
        None = 0,

        NotProcessed = 1,
        WaitForProcess = 2,
        Processing = 3,
        Processed = 4,
        Failed = 5
    }
}