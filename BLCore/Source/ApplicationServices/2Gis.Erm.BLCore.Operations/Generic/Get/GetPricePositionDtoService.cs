using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPricePositionDtoService : GetDomainEntityDtoServiceBase<PricePosition>
    {
        private static readonly PositionBindingObjectType[] AllowedPositionBindingObjectTypes =
            {
                PositionBindingObjectType.CategorySingle,
                PositionBindingObjectType.AddressCategorySingle,
                PositionBindingObjectType.AddressCategoryMultiple,
                PositionBindingObjectType.CategoryMultiple,
                PositionBindingObjectType.AddressFirstLevelCategorySingle,
                PositionBindingObjectType.AddressFirstLevelCategoryMultiple,
                PositionBindingObjectType.CategoryMultipleAsterix,
                PositionBindingObjectType.Firm
            };

        private readonly ISecureFinder _finder;

        public GetPricePositionDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<PricePosition> GetDto(long entityId)
        {
            var dto = _finder.Find<PricePosition>(x => x.Id == entityId)
                          .Select(x => new PricePositionDomainEntityDto
                              {
                                  Id = x.Id,
                                  PriceRef = new EntityReference { Id = x.PriceId, Name = null },
                                  PositionRef = new EntityReference { Id = x.PositionId, Name = x.Position.Name },
                                  RateType = x.RateType,
                                  IsRateTypeAvailable = AllowedPositionBindingObjectTypes.Contains(x.Position.BindingObjectTypeEnum),
                                  IsPositionControlledByAmount = x.Position.IsControlledByAmount,
                                  Cost = x.Cost,
                                  Amount = x.Amount,
                                  AmountSpecificationMode = x.AmountSpecificationMode,
                                  MinAdvertisementAmount = x.MinAdvertisementAmount,
                                  MaxAdvertisementAmount = x.MaxAdvertisementAmount,
                                  Timestamp = x.Timestamp,
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                  CreatedOn = x.CreatedOn,
                                  IsActive = x.IsActive,
                                  IsDeleted = x.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                  ModifiedOn = x.ModifiedOn,
                                  OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null }
                              })
                          .Single();

            return dto;
        }

        protected override IDomainEntityDto<PricePosition> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new PricePositionDomainEntityDto();
        }

        protected override void SetDtoProperties(IDomainEntityDto<PricePosition> domainEntityDto, long entityId, bool readOnly, long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            long priceId;
            var modelDto = (PricePositionDomainEntityDto)domainEntityDto;

            if (parentEntityName.Equals(EntityType.Instance.Price()) && parentEntityId.HasValue)
            {
                priceId = parentEntityId.Value;
            }
            else
            {
                priceId = modelDto.PriceRef.Id.Value;
            }

            // заполняем инфу о прайсе
            var priceInfo = _finder.Find<Price>(x => x.Id == priceId)
                                   .Select(x => new
                                       {
                                           PriceBeginDate = x.BeginDate,
                                           OrganizationUnitName = x.OrganizationUnit.Name,

                                           x.CurrencyId,
                                           CurrencyName = x.Currency.Name,

                                           x.IsDeleted,
                                           x.IsPublished,
                                       })
                                   .Single();

            modelDto.PriceRef = new EntityReference { Id = priceId, Name = string.Format("{0} ({1})", priceInfo.PriceBeginDate.ToShortDateString(), priceInfo.OrganizationUnitName) }; 

            modelDto.PriceIsDeleted = priceInfo.IsDeleted;
            modelDto.PriceIsPublished = priceInfo.IsPublished;

            modelDto.CurrencyRef = new EntityReference
                {
                    Id = priceInfo.CurrencyId,
                    Name = priceInfo.CurrencyName
                };
        }
    }
}