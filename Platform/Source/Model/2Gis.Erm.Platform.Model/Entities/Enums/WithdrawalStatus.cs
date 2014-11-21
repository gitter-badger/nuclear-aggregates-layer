namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum WithdrawalStatus
    {
        None = 0,

        InProgress = 1,
        Success = 2,
        Error = 3,
        Reverted = 4,

        Withdrawing = 5,
        Reverting = 6
    }
}