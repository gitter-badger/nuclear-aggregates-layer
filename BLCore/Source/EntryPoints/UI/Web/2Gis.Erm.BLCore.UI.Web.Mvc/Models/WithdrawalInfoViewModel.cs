using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class WithdrawalInfoViewModel : EntityViewModelBase<WithdrawalInfo>
    {
        public DateTime PeriodStartDate { get; set; }

        public DateTime PeriodEndDate { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        public WithdrawalStatus Status { get; set; }

        public string Comment { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (WithdrawalInfoDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            PeriodStartDate = modelDto.PeriodStartDate;
            PeriodEndDate = modelDto.PeriodEndDate;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            Status = modelDto.Status;
            Comment = modelDto.Comment;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new NotSupportedException();
        }
    }
}