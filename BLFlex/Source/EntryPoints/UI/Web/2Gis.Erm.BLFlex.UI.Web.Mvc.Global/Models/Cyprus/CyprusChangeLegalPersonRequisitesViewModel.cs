using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus
{
    public class CyprusChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, ICyprusAdapted
    {
        [RequiredLocalized]
        public string LegalName { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "PassportNumber", "this.value!='Businessman' && this.value!='NaturalPerson'")]
        [Dependency(DependencyType.DisableAndHide, "CardNumber", "this.value!='Businessman' && this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Inn", "this.value=='Businessman' || this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanInn", "this.value!='Businessman'")]
        [Dependency(DependencyType.DisableAndHide, "VAT", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value=='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }
        
        public string LegalAddress { get; set; }

        [StringLengthLocalized(11, MinimumLength = 11)]
        [DisplayNameLocalized("Tic")]
        public string Inn { get; set; }

        [StringLengthLocalized(11, MinimumLength = 11)]
        public string VAT { get; set; }

        [DisplayNameLocalized("Tic")]
        [StringLengthLocalized(11, MinimumLength = 11)]
        public string BusinessmanInn { get; set; }

        [StringLengthLocalized(9, MinimumLength = 9)]
        public string PassportNumber { get; set; }

        [StringLengthLocalized(15)]
        [OnlyDigitsLocalized]
        public string CardNumber { get; set; }

        [Dependency(DependencyType.ReadOnly, "Inn", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "BusinessmanInn", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "PassportNumber", "this.value!='Granted'")]
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