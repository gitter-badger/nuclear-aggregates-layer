using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class DeniedPositionObtainer : IBusinessModelEntityObtainer<DeniedPosition>, IAggregateReadModel<Price>
    {
        private readonly IFinder _finder;

        public DeniedPositionObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public DeniedPosition ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (DeniedPositionDomainEntityDto)domainEntityDto;

            var deniedPosition = _finder.Find(Specs.Find.ById<DeniedPosition>(dto.Id)).One()
                ?? new DeniedPosition { IsActive = true };

            deniedPosition.PositionId = dto.PositionRef.Id.Value;
            deniedPosition.PositionDeniedId = dto.PositionDeniedRef.Id.Value;
            deniedPosition.PriceId = dto.PriceRef.Id.Value;
            deniedPosition.ObjectBindingType = dto.ObjectBindingType;
            deniedPosition.Timestamp = dto.Timestamp;

            return deniedPosition;
        }
    }
}