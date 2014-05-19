using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

            var entity = dto.Id == 0
                             ? new Operation()
                             : _finder.Find(Specs.Find.ById<Operation>(dto.Id)).Single();

            entity.Description = dto.Description;
            entity.FinishTime = dto.FinishTime;
            entity.StartTime = dto.StartTime;
            entity.Status = (byte)dto.Status;
            entity.Type = (short)dto.Type;
            entity.OrganizationUnitId = dto.OrganizationUnitRef.Id;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}