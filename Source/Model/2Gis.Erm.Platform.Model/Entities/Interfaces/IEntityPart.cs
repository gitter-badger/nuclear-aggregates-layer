namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces
{
    public interface IEntityPart : IEntity, IEntityKey, IAuditableEntity, IStateTrackingEntity, IDeactivatableEntity, IDeletableEntity
    {
        long EntityId { get; set; }
    }
}