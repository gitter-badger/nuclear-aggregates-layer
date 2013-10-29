namespace DoubleGis.Erm.Model.Entities.Interfaces
{
    public interface IEntitySecure
    {
        int OwnerCode { get; set; }
        int? OldOwnerCode { get;}
    }
}
