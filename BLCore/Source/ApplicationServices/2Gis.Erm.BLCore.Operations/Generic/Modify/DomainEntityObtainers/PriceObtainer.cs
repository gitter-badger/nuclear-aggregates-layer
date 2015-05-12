using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

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
                        ?? new Price { IsActive = true, IsPublished = false, CreateDate = dto.CreateDate };

            price.OrganizationUnitId = dto.OrganizationUnitRef.Id.Value;
            price.Timestamp = dto.Timestamp;

            // TODO {all, 27.03.2015}: Следующие 2 поля выглядят подозрительно: по идее они должны задаваться операцией публикации
            price.BeginDate = dto.BeginDate.AddDays(1 - dto.BeginDate.Day);
            price.PublishDate = dto.PublishDate;
            
            return price;
        }
    }
}