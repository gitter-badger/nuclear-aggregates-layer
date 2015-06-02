using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class ReleaseInfoViewModel : EntityViewModelBase<ReleaseInfo>
    {
        public DateTime PeriodStartDate { get; set; }

        public DateTime PeriodEndDate { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [YesNoRadio]
        public bool IsBeta { get; set; }

        public ReleaseStatus Status { get; set; }

        public string Comment { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (ReleaseInfoDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            PeriodStartDate = modelDto.PeriodStartDate;
            PeriodEndDate = modelDto.PeriodEndDate;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            IsBeta = modelDto.IsBeta;
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