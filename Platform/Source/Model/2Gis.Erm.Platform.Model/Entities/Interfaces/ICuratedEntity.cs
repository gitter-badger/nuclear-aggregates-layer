namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces
{
    public interface ICuratedEntity
    {
        long OwnerCode { get; set; }
        long? OldOwnerCode { get; }
    }
}
