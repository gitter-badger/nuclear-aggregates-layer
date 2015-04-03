using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPriceDtoService : GetDomainEntityDtoServiceBase<Price>
    {
        private readonly ISecureFinder _finder;

        public GetPriceDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Price> GetDto(long entityId)
        {
            var dto = _finder.Find<Price>(x => x.Id == entityId)
                       .Select(x =>
                               new PriceDomainEntityDto
                                   {
                                       Id = x.Id,
                                       CreateDate = x.CreateDate,
                                       BeginDate = x.BeginDate,
                                       PublishDate = x.PublishDate,
                                       OrganizationUnitRef = new EntityReference { Id = x.OrganizationUnitId, Name = x.OrganizationUnit.Name },
                                       CurrencyRef = new EntityReference { Id = x.OrganizationUnit.Country.CurrencyId, Name = null },
                                       IsPublished = x.IsPublished,
                                       CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                       CreatedOn = x.CreatedOn,
                                       IsActive = x.IsActive,
                                       IsDeleted = x.IsDeleted,
                                       ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                       ModifiedOn = x.ModifiedOn,
                                       Timestamp = x.Timestamp
                                   })
                       .Single();

            dto.Name = string.Format("{0} ({1})", dto.BeginDate.ToShortDateString(), dto.OrganizationUnitRef.Name);

            return dto;
        }

        protected override IDomainEntityDto<Price> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var nextMonth = DateTime.UtcNow.Date.AddMonths(1);

            return new PriceDomainEntityDto
            {
                CreateDate = DateTime.UtcNow.Date,
                PublishDate = DateTime.UtcNow.Date.AddDays(1),
                BeginDate = new DateTime(nextMonth.Year, nextMonth.Month, 1),
                IsPublished = false,
                IsActive = true,
                IsDeleted = false,
                CurrencyRef = new EntityReference()
            };
        }
    }
}