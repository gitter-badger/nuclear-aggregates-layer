using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetCurrencyRateDtoService : GetDomainEntityDtoServiceBase<CurrencyRate>
    {
        private readonly ISecureFinder _finder;

        public GetCurrencyRateDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<CurrencyRate> GetDto(long entityId)
        {
            return _finder.Find<CurrencyRate>(x => x.Id == entityId)
                          .Select(x => new CurrencyRateDomainEntityDto
                              {
                                  Id = x.Id,
                                  BaseCurrencyRef = new EntityReference { Id = x.BaseCurrencyId, Name = x.BaseCurrency.Name },
                                  CurrencyRef = new EntityReference { Id = x.CurrencyId, Name = x.Currency.Name },
                                  Rate = x.Rate,
                                  IsCurrent = x.Currency.CurrencyRates.Where(y => !y.IsDeleted)
                                               .OrderByDescending(y => y.CreatedOn)
                                               .Select(y => (long?)y.Id)
                                               .FirstOrDefault() == x.Id,
                                  Timestamp = x.Timestamp,
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                  CreatedOn = x.CreatedOn,
                                  IsActive = x.IsActive,
                                  IsDeleted = x.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                  ModifiedOn = x.ModifiedOn
                              })
                          .Single();
        }

        protected override IDomainEntityDto<CurrencyRate> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var dto = new CurrencyRateDomainEntityDto();
            var baseCurrencies = _finder.Find<Currency>(x => x.IsBase && !x.IsDeleted && x.IsActive).Take(2).ToArray();

            if (baseCurrencies.Length == 0)
            {
                throw new NotificationException(BLResources.BaseCurrencyNotFound);
            }

            if (baseCurrencies.Length > 1 && baseCurrencies[1] != null)
            {
                throw new NotificationException(BLResources.MultipleBaseCurrencyFound);
            }

            var baseCurrency = baseCurrencies[0];

            dto.BaseCurrencyRef = new EntityReference { Id = baseCurrency.Id, Name = baseCurrency.Name };

            if (parentEntityId.HasValue && parentEntityName.Equals(EntityType.Instance.Currency()))
            {
                dto.CurrencyRef = _finder.Find<Currency>(x => x.Id == parentEntityId.Value).Select(x => new EntityReference { Id = x.Id, Name = x.Name }).Single();
            }

            return dto;
        }
    }
}