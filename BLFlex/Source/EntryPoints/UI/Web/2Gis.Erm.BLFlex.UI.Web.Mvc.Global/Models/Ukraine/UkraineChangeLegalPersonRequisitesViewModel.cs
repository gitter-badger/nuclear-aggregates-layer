using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine
{
    public class UkraineChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, ILegalPersonEgrpouHolder, IUkraineAdapted
    {
        [RequiredLocalized]
        public string LegalName { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Egrpou", "this.value=='Businessman'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanEgrpou", "this.value!='Businessman'")]
        public LegalPersonType LegalPersonType { get; set; }

        [RequiredLocalized]
        public string LegalAddress { get; set; }

        [StringLengthLocalized(12, MinimumLength = 10)]
        public string Ipn { get; set; }

        [StringLengthLocalized(8, MinimumLength = 8)]
        public string Egrpou { get; set; }

        [StringLengthLocalized(10, MinimumLength = 10)]
        [DisplayNameLocalized("Egrpou")]
        public string BusinessmanEgrpou { get; set; }

        [Dependency(DependencyType.ReadOnly, "Ipn", "this.value!='Granted'")]
        public LegalPersonChangeRequisitesAccess LegalPersonP { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "Ipn", "this.value.toLowerCase() == 'withvat'")]
        [ExcludeZeroValue]
        public TaxationType TaxationType { get; set; }

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