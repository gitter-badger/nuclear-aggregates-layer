using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class OperationObtainer : ISimplifiedModelEntityObtainer<Operation>
    {
        private readonly IFinder _finder;

        public OperationObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Operation ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (OperationDomainEntityDto)domainEntityDto;

            var entity = _finder.FindOne(Specs.Find.ById<Operation>(dto.Id)) 
                ?? new Operation();

            entity.Description = dto.Description;
            entity.FinishTime = dto.FinishTime;
            entity.StartTime = dto.StartTime;
            entity.Status = dto.Status;
            entity.Type = dto.Type;
            entity.OrganizationUnitId = dto.OrganizationUnitRef.Id;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}