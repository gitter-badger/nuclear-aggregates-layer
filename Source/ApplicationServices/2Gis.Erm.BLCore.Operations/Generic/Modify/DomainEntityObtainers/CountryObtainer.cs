using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

            var country = _finder.Find(CountrySpecifications.Find.ById(dto.Id)).SingleOrDefault() ??
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