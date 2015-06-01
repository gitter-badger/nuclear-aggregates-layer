using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetUserProfileDtoService : IGetDomainEntityDtoService<UserProfile>
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;

        public GetUserProfileDtoService(IFinder finder, IUserContext userContext)
        {
            _finder = finder;
            _userContext = userContext;
        }

        public IDomainEntityDto GetDomainEntityDto(long entityId, bool readOnly, long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var dto = _finder.Find(new FindSpecification<UserProfile>(x => (entityId != 0 && x.Id == entityId) || (parentEntityId.HasValue && x.UserId == parentEntityId)))
                             .Map(q => q.Select(entity => new UserProfileDomainEntityDto
                                 {
                                     Id = entity.Id,
                                     UserRef = new EntityReference { Id = entity.UserId },
                                     ProfileId = entity.Id,
                                     DomainAccountName = entity.User.Account,
                                     TimeZoneRef = new EntityReference { Id = entity.TimeZoneId },
                                     CultureInfoLCID = entity.CultureInfoLCID,
                                     Email = entity.Email,
                                     Phone = entity.Phone,
                                     Mobile = entity.Mobile,
                                     Address = entity.Address,
                                     Company = entity.Company,
                                     Position = entity.Position,
                                     Birthday = entity.Birthday,
                                     Gender = entity.Gender,
                                     PlanetURL = entity.PlanetURL,

                                     CreatedByRef = new EntityReference { Id = entity.CreatedBy },
                                     CreatedOn = entity.CreatedOn,
                                     IsActive = entity.IsActive,
                                     IsDeleted = entity.IsDeleted,
                                     ModifiedByRef = new EntityReference { Id = entity.ModifiedBy },
                                     ModifiedOn = entity.ModifiedOn,

                                     Timestamp = entity.Timestamp
                                 }))
                             .Top();

            if (dto != null)
            {
                return dto;
            }

            if (!parentEntityId.HasValue)
            {
                throw new NotificationException(BLResources.UserProfileCantCreateForUnspecifiedUser);
            }

            return new UserProfileDomainEntityDto
            {
                Id = 0,
                UserRef = new EntityReference { Id = parentEntityId.Value },
                CreatedByRef = new EntityReference { Id = _userContext.Identity.Code, Name = _userContext.Identity.DisplayName },
                ModifiedByRef = new EntityReference { Id = _userContext.Identity.Code, Name = _userContext.Identity.DisplayName },
                CreatedOn = DateTime.Now,
                IsActive = true
            };
        }
    }
}