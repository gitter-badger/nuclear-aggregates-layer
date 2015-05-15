using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech
{
    public class CzechChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, ICzechAdapted
    {
        [RequiredLocalized]
        public string LegalName { get; set; }

        [Dependency(DependencyType.DisableAndHide, "CardNumber", "this.value!='Businessman'")]
        public LegalPersonType LegalPersonType { get; set; }

        [RequiredLocalized]
        public string LegalAddress { get; set; }

        [StringLengthLocalized(12)]
        [DisplayNameLocalized("Dic")]
        public string Inn { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(8, MinimumLength = 8)]
        public string Ic { get; set; }

        [StringLengthLocalized(15)]
        [OnlyDigitsLocalized]
        public string CardNumber { get; set; }

        [Dependency(DependencyType.ReadOnly, "Inn", "this.value!='Granted'")]
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