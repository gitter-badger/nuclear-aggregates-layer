using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class CurrencyRateObtainer : ISimplifiedModelEntityObtainer<CurrencyRate>
    {
        private readonly IFinder _finder;

        public CurrencyRateObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public CurrencyRate ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CurrencyRateDomainEntityDto)domainEntityDto;

            var currencyRate = _finder.FindOne(Specs.Find.ById<CurrencyRate>(dto.Id)) 
                ?? new CurrencyRate { IsActive = true };

            currencyRate.Rate = dto.Rate;
            currencyRate.CurrencyId = dto.CurrencyRef.Id.Value;
            currencyRate.BaseCurrencyId = dto.BaseCurrencyRef.Id.Value;
            currencyRate.Timestamp = dto.Timestamp;

            return currencyRate;
        }
    }
}