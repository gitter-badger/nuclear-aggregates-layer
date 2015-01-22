using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class PriceObtainer : IBusinessModelEntityObtainer<Price>, IAggregateReadModel<Price>
    {
        private readonly IFinder _finder;

        public PriceObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Price ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PriceDomainEntityDto)domainEntityDto;

            var price = _finder.FindOne(Specs.Find.ById<Price>(dto.Id)) 
                ?? new Price { IsActive = true };

            price.CreateDate = dto.CreateDate;
            price.BeginDate = dto.BeginDate.AddDays(1 - dto.BeginDate.Day);
            price.PublishDate = dto.PublishDate;
            price.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
            price.CurrencyId = dto.CurrencyRef.Id.Value;
            price.IsPublished = dto.IsPublished;
            price.Timestamp = dto.Timestamp;
            
            return price;
        }
    }
}