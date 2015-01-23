using System;
using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class LocalMessageImportFromFileViewModel : EntityViewModelBase
    {
        [RequiredLocalized, ExcludeZeroValue]
        [Dependency(DependencyType.NotRequiredDisableHide, "BranchOfficeOrganizationUnit", "this.value != 'AccountDetailsFrom1C'")]
        public IntegrationTypeImport IntegrationType { get; set; }

        [DataType(DataType.MultilineText)]
        public string MessageErrors { get; set; }

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

        [RequiredLocalized]
        public LookupField BranchOfficeOrganizationUnit { get; set; }

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