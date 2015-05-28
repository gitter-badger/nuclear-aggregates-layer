using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class BargainObtainer : IBusinessModelEntityObtainer<Bargain>, IAggregateReadModel<Bargain>
    {
        private readonly IFinder _finder;

        public BargainObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Bargain ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BargainDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<Bargain>(dto.Id)).One() ??
                         new Bargain
                             {
                                 IsActive = true,
                                 OwnerCode = dto.OwnerRef.Id.Value
                             };

            entity.Comment = dto.Comment;
            entity.SignedOn = dto.SignedOn;
            entity.ClosedOn = dto.ClosedOn;
            entity.BargainKind = dto.BargainKind;
            entity.BargainEndDate = dto.BargainEndDate;
            entity.HasDocumentsDebt = dto.HasDocumentsDebt;
            entity.DocumentsComment = dto.DocumentsComment;
            entity.CustomerLegalPersonId = dto.CustomerLegalPersonRef.Id.Value;
            entity.ExecutorBranchOfficeId = dto.ExecutorBranchOfficeRef.Id.Value;

            return entity;
        }
    }
}