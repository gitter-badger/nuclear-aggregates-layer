using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.AutoMailer;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class LocalMessageExportViewModel : EntityViewModelBase
    {
        private DateTime _periodStart;
        private DateTime _periodStartFor1C;

        [RequiredLocalized, ExcludeZeroValue]
        [Dependency(DependencyType.NotRequiredDisableHide, "PeriodStart", "this.value=='FirmsWithActiveOrdersToDgpp' || this.value=='LegalPersonsTo1C'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PeriodStartFor1C", "this.value!='LegalPersonsTo1C'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "OrganizationUnit", "this.value=='FirmsWithActiveOrdersToDgpp' || this.value=='DataForAutoMailer'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "MailSendingType", "this.value!='DataForAutoMailer'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "IncludeRegionalAdvertisement", "this.value!='DataForAutoMailer'")]
        public IntegrationTypeExport IntegrationType { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("PaymentMonth")]
        public DateTime PeriodStart 
        {
            get { return _periodStart; }
            set { _periodStart = value.AssumeUtcKind(); }
        }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("ExportLegalPersonsTo1CPeriodStart")]
        public DateTime PeriodStartFor1C
        {
            get { return _periodStartFor1C; }
            set { _periodStartFor1C = value.AssumeUtcKind(); }
        }

        [RequiredLocalized]
        public MailSendingType MailSendingType { get; set; }

        [RequiredLocalized]
        public bool IncludeRegionalAdvertisement { get; set; }

        public override bool IsNew
        {
            get { return false; }
        }

        public override bool IsAuditable
        {
            get { return false; }
        }

        public override bool IsDeletable
        {
            get { return false; }
        }

        public override bool IsCurated
        {
            get { return false; }
        }

        public override bool IsDeactivatable
        {
            get { return false; }
        }

        public override LookupField CreatedBy { get; set; }
        public override LookupField ModifiedBy { get; set; }
        public override DateTime CreatedOn { get; set; }
        public override DateTime? ModifiedOn { get; set; }
        public override bool IsActive { get; set; }
        public override bool IsDeleted { get; set; }
        public override LookupField Owner { get; set; }
        public override long OwnerCode { get; set; }
        public override long OldOwnerCode { get; set; }
        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            throw new NotSupportedException();
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new NotSupportedException();
        }
    }
}