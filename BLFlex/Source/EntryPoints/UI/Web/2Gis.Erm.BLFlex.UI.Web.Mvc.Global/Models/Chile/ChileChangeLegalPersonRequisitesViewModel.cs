using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile
{
    public class ChileChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, IChileAdapted
    {
        [RequiredLocalized]
        public string LegalName { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string OperationsKind { get; set; }

        [RequiredLocalized]
        public string LegalAddress { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(12)]
        public string Rut { get; set; }

        [Dependency(DependencyType.ReadOnly, "Rut", "this.value!='Granted'")]
        public LegalPersonChangeRequisitesAccess LegalPersonP { get; set; }

        [RequiredLocalized]
        public LookupField Commune { get; set; }

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