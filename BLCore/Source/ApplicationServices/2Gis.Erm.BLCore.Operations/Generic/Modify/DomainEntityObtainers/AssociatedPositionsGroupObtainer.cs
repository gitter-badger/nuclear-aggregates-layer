using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AssociatedPositionsGroupObtainer : IBusinessModelEntityObtainer<AssociatedPositionsGroup>, IAggregateReadModel<Price>
    {
        private readonly IFinder _finder;

        public AssociatedPositionsGroupObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AssociatedPositionsGroup ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AssociatedPositionsGroupDomainEntityDto)domainEntityDto;

            var associatedPositionsGroup = _finder.Find(Specs.Find.ById<AssociatedPositionsGroup>(dto.Id)).One() ?? new AssociatedPositionsGroup { IsActive = true };

            associatedPositionsGroup.Name = dto.Name;
            associatedPositionsGroup.PricePositionId = dto.PricePositionRef.Id.Value;
            associatedPositionsGroup.Timestamp = dto.Timestamp;

            return associatedPositionsGroup;
        }
    }
}