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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public class ChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, IRussiaAdapted
    {
        [RequiredLocalized]
        public string LegalName { get; set; }

        [RequiredLocalized]
        public string ShortName { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Inn", "this.value!='LegalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Kpp", "this.value!='LegalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanInn", "this.value!='Businessman'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PassportNumber", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PassportSeries", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "RegistrationAddress", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value=='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }
        
        public string LegalAddress { get; set; }

        [StringLengthLocalized(10, MinimumLength = 10)]
        [OnlyDigitsLocalized]
        public string Inn { get; set; }

        [StringLengthLocalized(9, MinimumLength = 9)]
        [OnlyDigitsLocalized]
        public string Kpp { get; set; }

        [DisplayNameLocalized("Inn")]
        [StringLengthLocalized(12, MinimumLength = 12)]
        [OnlyDigitsLocalized]
        public string BusinessmanInn { get; set; }

        [StringLengthLocalized(4, MinimumLength = 4)]
        [OnlyDigitsLocalized]
        public string PassportSeries { get; set; }

        [StringLengthLocalized(6, MinimumLength = 6)]
        [OnlyDigitsLocalized]
        public string PassportNumber { get; set; }

        [StringLengthLocalized(512)]
        public string RegistrationAddress { get; set; }

        [Dependency(DependencyType.ReadOnly, "Inn", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "BusinessmanInn", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "PassportNumber", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "PassportSeries", "this.value!='Granted'")]
        public LegalPersonChangeRequisitesAccess LegalPersonP { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            throw new NotImplementedException();
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new NotImplementedException();
        }
    }
}