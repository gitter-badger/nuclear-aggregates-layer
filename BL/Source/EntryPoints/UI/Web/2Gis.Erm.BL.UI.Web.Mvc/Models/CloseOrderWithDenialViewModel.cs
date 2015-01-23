using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CloseOrderWithDenialViewModel : EntityViewModelBase
    {
        public long OrderId { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Reason", "!this.checked")]
        public bool CanClose { get; set; }
        public string Confirmation { get; set; }

        [DisplayNameLocalized("ReasonForOrderBeingClosed")]
        [StringLengthLocalized(300)]
        [RequiredLocalized]
        public string Reason { get; set; }

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
        
        public override bool IsDeleted { get; set; }
        public override bool IsActive { get; set; }

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