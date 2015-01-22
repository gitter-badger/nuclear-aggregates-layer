using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify
{
    public interface IModifyDomainEntityService
    {
        long Modify(IDomainEntityDto domainEntityDto);
    }
}