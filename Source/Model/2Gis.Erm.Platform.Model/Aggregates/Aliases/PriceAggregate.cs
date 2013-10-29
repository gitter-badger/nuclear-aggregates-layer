using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum PriceAggregate
    {
        Price = EntityName.Price,
        PricePosition = EntityName.PricePosition,
        AssociatedPositionsGroup = EntityName.AssociatedPositionsGroup,
        AssociatedPosition = EntityName.AssociatedPosition,
        DeniedPosition = EntityName.DeniedPosition
    }
}
