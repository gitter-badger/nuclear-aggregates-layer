using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum OrderAggregate
    {
        Order = EntityName.Order, 
        OrderPosition = EntityName.OrderPosition,
        OrderPositionAdvertisement = EntityName.OrderPositionAdvertisement,
        Bill = EntityName.Bill, //
        OrderFile = EntityName.OrderFile, 
        FileWithContent = EntityName.FileWithContent,
        OrderReleaseTotal = EntityName.OrderReleaseTotal,
        Bargain = EntityName.Bargain,
        ReleaseWithdrawal = EntityName.ReleaseWithdrawal,
        ReleaseWithdrawalPosition = EntityName.ReleasesWithdrawalsPosition
    } 
}
