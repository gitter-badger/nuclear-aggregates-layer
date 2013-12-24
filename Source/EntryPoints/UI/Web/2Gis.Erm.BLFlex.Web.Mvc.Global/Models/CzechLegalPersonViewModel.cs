using System;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Web.Mvc.Utils;
using DoubleGis.Erm.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

namespace DoubleGis.Erm.UI.Web.Mvc.Models
{
    public sealed class CzechLegalPersonViewModel : EntityViewModelBase<LegalPerson>, ICzechAdapted
    {
        public Guid? ReplicationCode { get; set; }
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [Dependency(DependencyType.ReadOnly, "LegalName", "Ext.getDom('Id').value != '0'")]
        public string LegalName { get; set; }

        [Dependency(DependencyType.Disable, "LegalPersonType", "Ext.getDom('Id').value != '0'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Inn", "this.value=='Businessman' || this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanInn", "this.value!='Businessman'")]
        [Dependency(DependencyType.DisableAndHide, "Ic", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value=='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

        public bool IsInSyncWith1C { get; set; }
        
        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        [Dependency(DependencyType.ReadOnly, "LegalAddress", "Ext.getDom('Id').value != '0'")]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [DisplayNameLocalized("Dic")]
        [StringLengthLocalized(12)]
        [Dependency(DependencyType.ReadOnly, "Inn", "Ext.getDom('Id').value != '0'")]
        public string Inn { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.ReadOnly, "Ic", "Ext.getDom('Id').value != '0'")]
        [StringLengthLocalized(8, MinimumLength = 8)]
        public string Ic { get; set; }

        [DisplayNameLocalized("Dic")]
        [StringLengthLocalized(12)]
        [Dependency(DependencyType.ReadOnly, "BusinessmanInn", "Ext.getDom('Id').value != '0'")]
        public string BusinessmanInn { get; set; }

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
            LegalPersonType = modelDto.LegalPersonTypeEnum;
            LegalAddress = modelDto.LegalAddress;
            Inn = modelDto.Inn;
            Ic = modelDto.Ic;
            BusinessmanInn = modelDto.BusinessmanInn;
            Client = LookupField.FromReference(modelDto.ClientRef);
            IsInSyncWith1C = modelDto.IsInSyncWith1C;
            ReplicationCode = modelDto.ReplicationCode;
            Comment = modelDto.Comment;
            HasProfiles = modelDto.HasProfiles;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new LegalPersonDomainEntityDto
                {
                    Id = Id,
                    LegalName = LegalName,
                    ShortName = LegalName,
                    LegalPersonTypeEnum = LegalPersonType,
                    LegalAddress = LegalAddress,
                    Inn = Inn,
                    Ic = Ic,
                    BusinessmanInn = BusinessmanInn,
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