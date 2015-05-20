using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class ChangeOrderStateOnTerminationViewModel : EntityViewModelBase
    {
        public long OrderId { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "TerminationReasonComment", "this.value=='TemporaryRejectionOther' || this.value=='RejectionOther'")]
        public OrderTerminationReason TerminationReason { get; set; }

        [DisplayNameLocalized("Comment")]
        public string TerminationReasonComment { get; set; }

        public override DateTime? ModifiedOn
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override byte[] Timestamp
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool IsActive
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override LookupField Owner
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long OwnerCode
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long OldOwnerCode
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool IsAuditable
        {
            get { throw new NotSupportedException(); }
        }

        public override bool IsDeletable
        {
            get { throw new NotSupportedException(); }
        }

        public override bool IsCurated
        {
            get { throw new NotSupportedException(); }
        }

        public override bool IsDeactivatable
        {
            get { throw new NotSupportedException(); }
        }

        public override bool IsDeleted
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool IsNew
        {
            get { throw new NotSupportedException(); }
        }

        public override LookupField CreatedBy
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override LookupField ModifiedBy
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override DateTime CreatedOn
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

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