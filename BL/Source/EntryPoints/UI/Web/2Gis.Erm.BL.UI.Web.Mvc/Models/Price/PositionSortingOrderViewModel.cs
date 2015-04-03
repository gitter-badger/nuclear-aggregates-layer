using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Price
{
    public sealed class PositionSortingOrderViewModel : EntityViewModelBase
    {
        public override bool IsActive { get; set; }
        public override bool IsDeleted { get; set; }
        public override LookupField Owner { get; set; }
        public override long OwnerCode { get; set; }
        public override long OldOwnerCode { get; set; }
        public override LookupField CreatedBy { get; set; }
        public override LookupField ModifiedBy { get; set; }
        public override DateTime CreatedOn { get; set; }
        public override DateTime? ModifiedOn { get; set; }
        public override byte[] Timestamp { get; set; }

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

        public override bool IsNew
        {
            get { return false; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            throw new NotImplementedException();
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new NotImplementedException();
        }
    }
}
