using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class PositionChildrenObtainer : IBusinessModelEntityObtainer<PositionChildren>, IAggregateReadModel<Position>
    {
        private readonly IFinder _finder;

        public PositionChildrenObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public PositionChildren ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PositionChildrenDomainEntityDto)domainEntityDto;

            var position = _finder.Find(Specs.Find.ById<PositionChildren>(dto.Id)).One() ??
                               new PositionChildren { IsActive = true };

            position.MasterPositionId = dto.MasterPositionRef.Id.Value;
            position.ChildPositionId = dto.ChildPositionRef.Id.Value;
            position.Timestamp = dto.Timestamp;

            return position;
        }
    }
}