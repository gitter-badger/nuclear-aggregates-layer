using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine
{
    public sealed class UkraineKazakhstanContactViewModel : EntityViewModelBase<Contact>, IUkraineAdapted, IKazakhstanAdapted
    {
        [StringLengthLocalized(160)]
        public string FullName { get; set; }

        [StringLengthLocalized(160)]
        [RequiredLocalized]
        public string FirstName { get; set; }

        [StringLengthLocalized(160)]
        public string MiddleName { get; set; }

        [StringLengthLocalized(160)]
        public string LastName { get; set; }

        [StringLengthLocalized(160)]
        [RequiredLocalized]
        public string Salutation { get; set; }

        [StringLengthLocalized(64)]
        [DisplayNameLocalized("WorkPhoneNumber")]
        public string MainPhoneNumber { get; set; }

        [StringLengthLocalized(64)]
        public string AdditionalPhoneNumber { get; set; }

        [StringLengthLocalized(64)]
        public string MobilePhoneNumber { get; set; }

        [StringLengthLocalized(50)]
        public string Fax { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(100)]
        public string WorkEmail { get; set; }

        [UrlLocalized]
        [StringLengthLocalized(200)]
        [DisplayNameLocalized("WebsiteOrBlog")]
        public string Website { get; set; }

        [StringLengthLocalized(64)]
        public string Im { get; set; }

        [StringLengthLocalized(170)]
        public string JobTitle { get; set; }

        [StringLengthLocalized(100)]
        [DisplayNameLocalized("ContactDepartmentName")]
        public string Department { get; set; }

        [StringLengthLocalized(450)] 
        public string WorkAddress { get; set; }

        [StringLengthLocalized(450)] 
        public string HomeAddress { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        [RequiredLocalized]
        public Gender Gender { get; set; }

        [RequiredLocalized]
        public AccountRole AccountRole { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Transfer, "ClientReplicationCode", "(this.item && this.item.data)?this.item.data.ReplicationCode:undefined;")]
        public LookupField Client { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        [YesNoRadio]
        public bool IsFired { get; set; }

        public DateTime? BirthDate { get; set; }

        // Доступные для выбора значения комбобокса "Обращение" в зависимости от выбранного пола контакта
        public IDictionary<string, string[]> AvailableSalutations { get; set; }

        // Определяет имя зоны для груповых операций 
        public string BusinessModelArea { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (ContactDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            FullName = modelDto.FullName;

            FirstName = modelDto.FirstName;
            MiddleName = modelDto.MiddleName;
            LastName = modelDto.LastName;
            Salutation = modelDto.Salutation;
            MainPhoneNumber = modelDto.MainPhoneNumber;
            AdditionalPhoneNumber = modelDto.AdditionalPhoneNumber;
            MobilePhoneNumber = modelDto.MobilePhoneNumber;
            Fax = modelDto.Fax;
            WorkEmail = modelDto.WorkEmail;
            Website = modelDto.Website;
            Im = modelDto.ImIdentifier;
            JobTitle = modelDto.JobTitle;
            Department = modelDto.Department;
            WorkAddress = modelDto.WorkAddress;
            HomeAddress = modelDto.HomeAddress;
            Comment = modelDto.Comment;

            Gender = modelDto.GenderCode;
            AccountRole = modelDto.AccountRole;

            Client = LookupField.FromReference(modelDto.ClientRef);

            IsFired = modelDto.IsFired;
            BirthDate = modelDto.BirthDate;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new ContactDomainEntityDto
                {
                    Id = Id,
                    FullName = FullName,

                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    LastName = LastName,
                    Salutation = Salutation,
                    MainPhoneNumber = MainPhoneNumber,
                    AdditionalPhoneNumber = AdditionalPhoneNumber,
                    MobilePhoneNumber = MobilePhoneNumber,
                    Fax = Fax,
                    WorkEmail = WorkEmail,
                    Website = Website,
                    ImIdentifier = Im,
                    JobTitle = JobTitle,
                    Department = Department,
                    WorkAddress = WorkAddress,
                    HomeAddress = HomeAddress,
                    Comment = Comment,

                    GenderCode = Gender,
                    AccountRole = AccountRole,

                    ClientRef = Client.ToReference(),

                    IsFired = IsFired,
                    BirthDate = BirthDate,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}