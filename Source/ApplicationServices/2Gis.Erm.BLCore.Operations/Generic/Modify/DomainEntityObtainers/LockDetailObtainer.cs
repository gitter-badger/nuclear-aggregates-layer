using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class LockDetailObtainer : IBusinessModelEntityObtainer<LockDetail>, IAggregateReadModel<Account>
    {
        private readonly IFinder _finder;

        public LockDetailObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LockDetail ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LockDetailDomainEntityDto)domainEntityDto;

            var lockDetail =
                dto.Id == 0
                    ? new LockDetail { IsActive = true }
                    : _finder.Find(Specs.Find.ById<LockDetail>(dto.Id)).Single();


            lockDetail.Id = dto.Id;
            lockDetail.PriceId = dto.PriceRef.Id.Value;
            lockDetail.LockId = dto.LockRef.Id.Value;
            lockDetail.Amount = dto.Amount;
            lockDetail.Description = dto.Description;
            lockDetail.Timestamp = dto.Timestamp;

            return lockDetail;
        }
    }
}