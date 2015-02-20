using System.Globalization;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class UserViewModel : EditableIdEntityViewModelBase<User>, IDisplayNameAspect
    {
        [RequiredLocalized]
        public string FirstName { get; set; }

        [RequiredLocalized]
        public string LastName { get; set; }

        [DisplayNameLocalized("UserAccount")]
        [RequiredLocalized]
        public string Account { get; set; }

        // [DisplayNameLocalized(typeof(MetadataResources), "UserDepartment")]
        [RequiredLocalized]
        public LookupField Department { get; set; }

        [DisplayNameLocalized("ParentUser")]
        public LookupField Parent { get; set; }

        public bool IsServiceUser { get; set; }

        public string DisplayName { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (UserDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            FirstName = modelDto.FirstName;
            LastName = modelDto.LastName;
            DisplayName = modelDto.DisplayName;
            Account = modelDto.Account;
            Department = LookupField.FromReference(modelDto.DepartmentRef);
            Parent = LookupField.FromReference(modelDto.ParentRef);
            IsServiceUser = modelDto.IsServiceUser;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new UserDomainEntityDto
                {
                    Id = Id,
                    FirstName = FirstName,
                    LastName = LastName,
                    DisplayName = string.IsNullOrWhiteSpace(LastName)
                                      ? FirstName
                                      : string.Format(CultureInfo.CurrentCulture, "{0}, {1}", LastName, FirstName),
                    Account = Account,
                    ParentRef = Parent.ToReference(),
                    DepartmentRef = Department.ToReference(),
                    IsServiceUser = IsServiceUser,
                    Timestamp = Timestamp
                };
            
            return dto;
        }
    }
}