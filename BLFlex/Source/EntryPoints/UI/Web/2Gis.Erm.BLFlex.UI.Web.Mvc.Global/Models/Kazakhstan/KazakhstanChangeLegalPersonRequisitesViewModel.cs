using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan
{
    public class KazakhstanChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, IKazakhstanAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string LegalName { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Bin", "this.value!='LegalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BinIin", "this.value!='Businessman'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value!='LegalPerson' && this.value!='Businessman'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Iin", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "IdentityCardNumber", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "IdentityCardIssuedOn", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "IdentityCardIssuedBy", "this.value!='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(12, MinimumLength = 12)]
        public string BinIin { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(12, MinimumLength = 12)]
        public string Bin { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(12, MinimumLength = 12)]
        public string Iin { get; set; }

        [StringLengthLocalized(9, MinimumLength = 9)]
        public string IdentityCardNumber { get; set; }

        public DateTime? IdentityCardIssuedOn { get; set; }

        public string IdentityCardIssuedBy { get; set; }

        [Dependency(DependencyType.ReadOnly, "Bin", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "BinIin", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "Iin", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "IdentityCardNumber", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "IdentityCardIssuedOn", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "IdentityCardIssuedBy", "this.value!='Granted'")]
        public LegalPersonChangeRequisitesAccess LegalPersonP { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            throw new System.NotImplementedException();
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new System.NotImplementedException();
        }
    }

}