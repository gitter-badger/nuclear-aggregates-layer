using System.Linq;

using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetFirmContactDtoService : GetDomainEntityDtoServiceBase<FirmContact>
    {
        private readonly IFinder _finder;

        public GetFirmContactDtoService(IUserContext userContext, IFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<FirmContact> GetDto(long entityId)
        {
            return _finder.FindObsolete(new FindSpecification<FirmContact>(x => x.Id == entityId))
                          .Select(entity => new FirmContactDomainEntityDto
                              {
                                  IsFirmAddressDeleted = entity.FirmAddress.IsDeleted,
                                  IsFirmDeleted = entity.FirmAddress.Firm.IsDeleted,
                                  IsFirmAddressActive = entity.FirmAddress.IsActive,
                                  IsFirmActive = entity.FirmAddress.Firm.IsActive,
                                  FirmAddressClosedForAscertainment = entity.FirmAddress.ClosedForAscertainment,
                                  FirmClosedForAscertainment = entity.FirmAddress.Firm.ClosedForAscertainment,
                                  Id = entity.Id,
                                  ContactType = entity.ContactType,
                                  Contact = entity.Contact,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                  CreatedOn = entity.CreatedOn,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                  ModifiedOn = entity.ModifiedOn,
                                  Timestamp = entity.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<FirmContact> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {   
            return new FirmContactDomainEntityDto();
        }
    }
}