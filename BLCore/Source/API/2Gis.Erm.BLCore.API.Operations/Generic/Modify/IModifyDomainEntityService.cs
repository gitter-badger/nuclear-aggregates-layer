using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify
{
    public interface IModifyDomainEntityService
    {
        long Modify(IDomainEntityDto domainEntityDto);
    }
}