using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech
{
    public class CzechChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, ICzechAdapted
    {
        [RequiredLocalized]
        public string LegalName { get; set; }

        [Dependency(DependencyType.DisableAndHide, "CardNumber", "this.value!='Businessman' && this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Inn", "this.value=='Businessman' || this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanInn", "this.value!='Businessman'")]
        [Dependency(DependencyType.DisableAndHide, "Ic", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value=='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

        public string LegalAddress { get; set; }

        [StringLengthLocalized(12)]
        [DisplayNameLocalized("Dic")]
        public string Inn { get; set; }

        [DisplayNameLocalized("Dic")]
        [StringLengthLocalized(12)]
        public string BusinessmanInn { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(8, MinimumLength = 8)]
        public string Ic { get; set; }

        [StringLengthLocalized(15)]
        [OnlyDigitsLocalized]
        public string CardNumber { get; set; }

        [Dependency(DependencyType.ReadOnly, "Inn", "this.value!='Granted'")]
        [Dependency(DependencyType.ReadOnly, "BusinessmanInn", "this.value!='Granted'")]
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