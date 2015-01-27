using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan
{
    public sealed class KazakhstanLegalPersonViewModel : EntityViewModelBase<LegalPerson>, ILegalNameAspect, IDoesLegalPersonHaveAnyProfilesAspect, IKazakhstanAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [Dependency(DependencyType.ReadOnly, "LegalName", "Ext.getDom('Id').value != '0'")]
        public string LegalName { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Bin", "this.value!=''")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BinIin", "this.value!='Businessman'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value!='' && this.value!='Businessman'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Iin", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "IdentityCardNumber", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "IdentityCardIssuedOn", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "IdentityCardIssuedBy", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.Disable, "LegalPersonType", "Ext.getDom('Id').value != '0'")]
        public LegalPersonType LegalPersonType { get; set; }

        [RequiredLocalized]
        public LookupField Client { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(12, MinimumLength = 12)]
        [Dependency(DependencyType.ReadOnly, "BinIin", "Ext.getDom('Id').value != '0'")]
        public string BinIin { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(12, MinimumLength = 12)]
        [Dependency(DependencyType.ReadOnly, "Bin", "Ext.getDom('Id').value != '0'")]
        public string Bin { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(12, MinimumLength = 12)]
        [Dependency(DependencyType.ReadOnly, "Iin", "Ext.getDom('Id').value != '0'")]
        public string Iin { get; set; }

        [StringLengthLocalized(512)]
        [Dependency(DependencyType.ReadOnly, "LegalAddress", "Ext.getDom('Id').value != '0'")]
        public string LegalAddress { get; set; }
        
        [Dependency(DependencyType.ReadOnly, "IdentityCardNumber", "Ext.getDom('Id').value != '0'")]
        [StringLengthLocalized(9, MinimumLength = 9)]
        public string IdentityCardNumber { get; set; }

        [Dependency(DependencyType.ReadOnly, "IdentityCardIssuedOn", "Ext.getDom('Id').value != '0'")]
        public DateTime? IdentityCardIssuedOn { get; set; }

        [Dependency(DependencyType.ReadOnly, "IdentityCardIssuedBy", "Ext.getDom('Id').value != '0'")]
        public string IdentityCardIssuedBy { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

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
            var modelDto = (KazakhstanLegalPersonDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            BinIin = Bin = modelDto.Bin;

            LegalName = modelDto.LegalName;
            LegalPersonType = modelDto.LegalPersonTypeEnum;
            LegalAddress = modelDto.LegalAddress;
            Client = LookupField.FromReference(modelDto.ClientRef);
            Comment = modelDto.Comment;
            HasProfiles = modelDto.HasProfiles;
            Timestamp = modelDto.Timestamp;

            switch (modelDto.LegalPersonTypeEnum)
            {
                case LegalPersonType.LegalPerson:
                    Bin = modelDto.Bin;
                    LegalAddress = modelDto.LegalAddress;
                    break;
                case LegalPersonType.Businessman:
                    BinIin = modelDto.Bin;
                    LegalAddress = modelDto.LegalAddress;
                    break;
                case LegalPersonType.NaturalPerson:
                    Iin = modelDto.Bin;
                    IdentityCardNumber = modelDto.IdentityCardNumber;
                    IdentityCardIssuedOn = modelDto.IdentityCardIssuedOn;
                    IdentityCardIssuedBy = modelDto.IdentityCardIssuedBy;
                    break;
            }
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new KazakhstanLegalPersonDomainEntityDto
                       {
                           Id = Id,
                           LegalName = LegalName,
                           LegalPersonTypeEnum = LegalPersonType,
                           LegalAddress = LegalAddress,
                           ClientRef = Client.ToReference(),
                           Comment = Comment,
                           OwnerRef = Owner.ToReference(),
                           Timestamp = Timestamp,
                           Bin = ReadBin(LegalPersonType, Bin, BinIin, Iin),
                           IdentityCardNumber = IdentityCardNumber,
                           IdentityCardIssuedOn = IdentityCardIssuedOn,
                           IdentityCardIssuedBy = IdentityCardIssuedBy,
                       };
        }

        private static string ReadBin(LegalPersonType legalPersonType, string forLegalPerson, string forBusinessman, string forNaturalPerson)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return forLegalPerson;
                case LegalPersonType.Businessman:
                    return forBusinessman;
                case LegalPersonType.NaturalPerson:
                    return forNaturalPerson;
                default:
                    throw new ArgumentException("legalPersonType");
            }
        }
    }
}