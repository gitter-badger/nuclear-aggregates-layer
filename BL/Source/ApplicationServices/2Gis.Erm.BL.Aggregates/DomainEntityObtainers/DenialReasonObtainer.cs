using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BL.Aggregates.DomainEntityObtainers
{
    public sealed class DenialReasonObtainer : ISimplifiedModelEntityObtainer<DenialReason>
    {
        private readonly IFinder _finder;

        public DenialReasonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public DenialReason ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (DenialReasonDomainEntityDto)domainEntityDto;
            var entity = _finder.Find(Specs.Find.ById<DenialReason>(dto.Id)).One()
                         ?? new DenialReason { IsActive = true };

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.ProofLink = dto.ProofLink;
            entity.Type = dto.Type;
            entity.Timestamp = dto.Timestamp;
            return entity;
        }
    }
}