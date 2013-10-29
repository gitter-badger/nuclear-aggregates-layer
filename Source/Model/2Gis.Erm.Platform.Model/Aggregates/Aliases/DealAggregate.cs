using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum DealAggregate
    {
        Deal = EntityName.Deal,
        Order = EntityName.Order,
        OrderPosition = EntityName.OrderPosition,
        AfterSaleServiceActivity = EntityName.AfterSaleServiceActivity // возможно - ППС место в агрегате activity
    }
}
