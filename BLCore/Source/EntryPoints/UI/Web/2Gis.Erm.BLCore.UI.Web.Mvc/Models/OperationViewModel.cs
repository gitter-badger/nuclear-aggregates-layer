using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public class OperationViewModel : EntityViewModelBase<Operation>, IBusinessOperationTypeAspect
    {
        [DisplayNameLocalized("OperationDescription")]
        public string Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? FinishTime { get; set; }

        [DisplayNameLocalized("OperationStatus")]
        public OperationStatus Status { get; set; }

        [DisplayNameLocalized("OperationType")]
        public BusinessOperation Type { get; set; }

        public LookupField OrganizationUnit { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (OperationDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Description = modelDto.Description;
            StartTime = modelDto.StartTime;
            FinishTime = modelDto.FinishTime;
            Status = modelDto.Status;
            Type = modelDto.Type;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new OperationDomainEntityDto
                {
                    Id = Id,
                    Description = Description,
                    StartTime = StartTime,
                    FinishTime = FinishTime,
                    Status = Status,
                    Type = Type,
                    OrganizationUnitRef = new EntityReference(OrganizationUnit.Key,  OrganizationUnit.Value),
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}