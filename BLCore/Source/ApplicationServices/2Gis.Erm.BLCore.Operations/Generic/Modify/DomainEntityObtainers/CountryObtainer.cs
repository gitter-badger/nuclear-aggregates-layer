using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class CountryObtainer : ISimplifiedModelEntityObtainer<Country>
    {
        private readonly IFinder _finder;

        public CountryObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Country ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CountryDomainEntityDto)domainEntityDto;

            var country = _finder.Find(Specs.Find.ById<Country>(dto.Id)).One() ??
                          new Country { IsActive = true, Id = dto.Id };

            if (dto.Timestamp == null && country.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            country.Name = dto.Name;
            country.IsoCode = dto.IsoCode;
            country.CurrencyId = dto.CurrencyRef.Id.Value;
            country.Timestamp = dto.Timestamp;

            return country;
        }
    }
}