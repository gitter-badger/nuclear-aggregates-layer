using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BL.Aggregates.DomainEntityObtainers
{
    public sealed class LimitObtainer : IBusinessModelEntityObtainer<Limit>, IAggregateReadModel<Account>
    {
        private readonly IFinder _finder;

        public LimitObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Limit ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LimitDomainEntityDto)domainEntityDto;

            var limit = _finder.Find(Specs.Find.ById<Limit>(dto.Id)).One() 
                ?? new Limit { IsActive = true };

            limit.AccountId = dto.AccountRef.Id.Value;
            limit.CloseDate = dto.CloseDate;
            limit.Amount = dto.Amount;
            limit.Status = dto.Status;
            limit.Comment = dto.Comment;
            limit.StartPeriodDate = dto.StartPeriodDate;
            limit.EndPeriodDate = dto.StartPeriodDate.GetLastDateOfMonth();
            limit.InspectorCode = dto.InspectorRef.Id.Value;
            limit.Timestamp = dto.Timestamp;

            return limit;
        }
    }
}