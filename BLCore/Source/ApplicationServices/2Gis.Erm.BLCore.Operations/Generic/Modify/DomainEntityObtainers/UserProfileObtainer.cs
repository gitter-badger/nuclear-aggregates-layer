using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class UserProfileObtainer : IBusinessModelEntityObtainer<UserProfile>, IAggregateReadModel<User>
    {
        private readonly IFinder _finder;

        public UserProfileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public UserProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (UserProfileDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<UserProfile>(dto.Id)).One() ??
                         new UserProfile { IsActive = true, Id = dto.Id};

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.Address = dto.Address;
            entity.Birthday = dto.Birthday;
            entity.Company = dto.Company;
            entity.Email = dto.Email;
            entity.Gender = dto.Gender;
            entity.Mobile = dto.Mobile;
            entity.Phone = dto.Phone;
            entity.PlanetURL = dto.PlanetURL;
            entity.Position = dto.Position;
            entity.CultureInfoLCID = dto.CultureInfoLCID;
            entity.TimeZoneId = dto.TimeZoneRef.Id.Value;
            entity.UserId = dto.UserRef.Id.Value;

            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}