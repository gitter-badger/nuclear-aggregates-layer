using System;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public class EmiratesChangeLegalPersonRequisitesViewModel : EntityViewModelBase<LegalPerson>, IEmiratesAdapted
    {
        [RequiredLocalized]
        public string LegalName { get; set; }

        [RequiredLocalized]
        public string LegalAddress { get; set; }

        [StringLengthLocalized(10)]
        [RequiredLocalized]
        [OnlyDigitsLocalized]
        public string CommercialLicense { get; set; }

        [RequiredLocalized]
        public DateTime? CommercialLicenseBeginDate { get; set; }

        [RequiredLocalized]
        [GreaterOrEqualThan("CommercialLicenseBeginDate", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "EndDateMustntBeLessThanBeginDate")]
        public DateTime? CommercialLicenseEndDate { get; set; }

        [Dependency(DependencyType.ReadOnly, "CommercialLicense", "this.value!='Granted'")]
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