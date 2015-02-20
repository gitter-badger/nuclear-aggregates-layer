using System.Collections.Specialized;
using System.Globalization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
// ReSharper restore CheckNamespace
{
    public partial class UserProfileViewModel : EditableIdEntityViewModelBase<UserProfile>
    {
        [JsonIgnore]
        public NameValueCollection InitParams
        {
            get
            {
                var collection = new NameValueCollection { { "UserCode", UserId.ToString(CultureInfo.InvariantCulture) } };
                return collection;
            }
        }

        [JsonIgnore]
        public override LookupField Owner { get; set; }
        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (UserProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            UserId = modelDto.UserRef.Id.Value;
            DomainAccountName = modelDto.DomainAccountName;

            CreatedBy = LookupField.FromReference(modelDto.CreatedByRef);
            CreatedOn = modelDto.CreatedOn;
            IsActive = modelDto.IsActive;
            IsDeleted = modelDto.IsDeleted;
            ModifiedBy = LookupField.FromReference(modelDto.ModifiedByRef);
            ModifiedOn = modelDto.ModifiedOn;

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new UserProfileDomainEntityDto
                {
                    Id = Id,
                    Address = PersonalInfo.Address,
                    Birthday = PersonalInfo.BirthDay,
                    Company = PersonalInfo.Company,
                    Email = PersonalInfo.Email,
                    Gender = (int)PersonalInfo.Gender,
                    Mobile = PersonalInfo.Mobile,
                    Phone = PersonalInfo.Phone,
                    PlanetURL = PersonalInfo.PlanetURL,
                    Position = PersonalInfo.Position,
                    CultureInfoLCID = new CultureInfo(Localsettings.CurrentCultureInfoName).LCID,
                    TimeZoneRef = new EntityReference(Localsettings.CurrentTimeZoneId),
                    UserRef = new EntityReference(UserId),
                    Timestamp = Timestamp
                };
            return dto;
        }
    }
}