using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public sealed class GetCountryDtoService : GetDomainEntityDtoServiceBase<Country>
    {
        private readonly IFinder _finder;

        public GetCountryDtoService(IUserContext userContext, IFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Country> GetDto(long entityId)
        {
            return _finder.Find(Specs.Find.ById<Country>(entityId))
                          .Select(entity => new CountryDomainEntityDto
                                                {
                                                    Id = entity.Id,
                                                    Name = entity.Name,
                                                    IsoCode = entity.IsoCode,
                                                    CurrencyRef = new EntityReference { Id = entity.CurrencyId, Name = entity.Currency.Name },
                                                    Timestamp = entity.Timestamp,
                                                    CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                                    CreatedOn = entity.CreatedOn,
                                                    IsActive = entity.IsActive,
                                                    IsDeleted = entity.IsDeleted,
                                                    ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                                    ModifiedOn = entity.ModifiedOn
                                                })
                          .Single();
        }

        protected override IDomainEntityDto<Country> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new CountryDomainEntityDto();
        }
    }
}