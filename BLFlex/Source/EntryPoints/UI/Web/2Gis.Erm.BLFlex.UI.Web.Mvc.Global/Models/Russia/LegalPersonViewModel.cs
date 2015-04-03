using System;

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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public sealed class LegalPersonViewModel : EntityViewModelBase<LegalPerson>, IRussiaAdapted
    {
        public Guid? ReplicationCode { get; set; }

        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string LegalName { get; set; }

        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ShortName { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Inn", "this.value!=''")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Kpp", "this.value!=''")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PassportSeries", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PassportNumber", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PassportIssuedBy", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "RegistrationAddress", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanInn", "this.value!='Businessman'")]
        [Dependency(DependencyType.Transfer, "Inn", "this.value == '' ? Ext.getDom('Inn').value : ''")] // именно пустая строка в качестве значения списка, см метод DefaultEnumAdaptationService.GetEnumValues
        [Dependency(DependencyType.Transfer, "Kpp", "this.value == '' ? Ext.getDom('Kpp').value : ''")]
        [Dependency(DependencyType.Transfer, "BusinessmanInn", "this.value == 'Businessman' ? Ext.getDom('BusinessmanInn').value : ''")]
        [Dependency(DependencyType.Transfer, "PassportSeries", "this.value == 'NaturalPerson' ? Ext.getDom('PassportSeries').value : ''")]
        [Dependency(DependencyType.Transfer, "PassportNumber", "this.value == 'NaturalPerson' ? Ext.getDom('PassportNumber').value : ''")]
        public LegalPersonType LegalPersonType { get; set; }

        [Dependency(DependencyType.Disable, "LegalPersonType", "(this.value && this.value.toLowerCase() == 'true') || Ext.getDom('Id').value != '0'")]
        [Dependency(DependencyType.ReadOnly, "Inn", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "Kpp", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "BusinessmanInn", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "PassportSeries", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "PassportNumber", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "LegalName", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "ShortName", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "LegalAddress", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "RegistrationAddress", "this.value && this.value.toLowerCase() == 'true'")]
        [Dependency(DependencyType.ReadOnly, "PassportNumber", "this.value && this.value.toLowerCase() == 'true'")]
        public bool IsInSyncWith1C { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [SanitizedString]
        [StringLengthLocalized(10, MinimumLength = 10)]
        [Dependency(DependencyType.ReadOnly, "Inn", "Ext.getDom('Id').value != '0'")]
        [OnlyDigitsLocalized]
        public string Inn { get; set; }

        [SanitizedString]
        [StringLengthLocalized(9, MinimumLength = 9)]
        [Dependency(DependencyType.ReadOnly, "Kpp", "Ext.getDom('Id').value != '0'")]
        [OnlyDigitsLocalized]
        public string Kpp { get; set; }

        [SanitizedString]
        [DisplayNameLocalized("Inn")]
        [StringLengthLocalized(12, MinimumLength = 12)]
        [Dependency(DependencyType.ReadOnly, "BusinessmanInn", "Ext.getDom('Id').value != '0'")]
        [OnlyDigitsLocalized]
        public string BusinessmanInn { get; set; }

        [SanitizedString]
        [Dependency(DependencyType.ReadOnly, "PassportSeries", "Ext.getDom('Id').value != '0'")]
        [StringLengthLocalized(4, MinimumLength = 4)]
        [OnlyDigitsLocalized]
        public string PassportSeries { get; set; }

        [SanitizedString]
        [StringLengthLocalized(6, MinimumLength = 6)]
        [Dependency(DependencyType.ReadOnly, "PassportNumber", "Ext.getDom('Id').value != '0'")]
        [OnlyDigitsLocalized]
        public string PassportNumber { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512)]
        public string PassportIssuedBy { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512)]
        public string RegistrationAddress { get; set; }

        [RequiredLocalized]
        public LookupField Client { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public bool HasProfiles { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LegalPersonDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            LegalName = modelDto.LegalName;
            ShortName = modelDto.ShortName;
            LegalPersonType = modelDto.LegalPersonTypeEnum;
            LegalAddress = modelDto.LegalAddress;
            Inn = modelDto.LegalPersonTypeEnum  == LegalPersonType.LegalPerson ? modelDto.Inn : null;
            Kpp = modelDto.Kpp;
            BusinessmanInn = modelDto.LegalPersonTypeEnum == LegalPersonType.Businessman ? modelDto.Inn : null;
            PassportSeries = modelDto.PassportSeries;
            PassportNumber = modelDto.PassportNumber;
            PassportIssuedBy = modelDto.PassportIssuedBy;
            RegistrationAddress = modelDto.RegistrationAddress;
            Client = LookupField.FromReference(modelDto.ClientRef);
            IsInSyncWith1C = modelDto.IsInSyncWith1C;
            ReplicationCode = modelDto.ReplicationCode;
            Comment = modelDto.Comment;
            HasProfiles = modelDto.HasProfiles;
            Timestamp = modelDto.Timestamp;
            Owner = LookupField.FromReference(modelDto.OwnerRef);
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new LegalPersonDomainEntityDto
            {
                Id = Id,
                LegalName = LegalName,
                ShortName = ShortName,
                LegalPersonTypeEnum = LegalPersonType,
                LegalAddress = LegalAddress,
                Inn = LegalPersonType == LegalPersonType.LegalPerson ? Inn : BusinessmanInn,
                Kpp = Kpp,
                PassportSeries = PassportSeries,
                PassportNumber = PassportNumber,
                PassportIssuedBy = PassportIssuedBy,
                RegistrationAddress = RegistrationAddress,
                ClientRef = Client.ToReference(),
                IsInSyncWith1C = IsInSyncWith1C,
                ReplicationCode = ReplicationCode.Value,
                Comment = Comment,
                OwnerRef = Owner.ToReference(),
                Timestamp = Timestamp
            };
        }
    }
}