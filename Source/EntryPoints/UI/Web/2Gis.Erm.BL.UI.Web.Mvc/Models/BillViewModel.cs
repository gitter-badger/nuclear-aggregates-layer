using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class BillViewModel : EntityViewModelBase<Bill>
    {
        public string BillNumber { get; set; }

        public DateTime BillDate { get; set; }

        public DateTime BeginDistributionDate { get; set; }

        public DateTime EndDistributionDate { get; set; }

        public DateTime PaymentDatePlan { get; set; }

        public decimal PayablePlan { get; set; }

        public string Comment { get; set; }

        [Dependency(DependencyType.ReadOnly, "PaymentDatePlan", "this.value && this.value.toLowerCase()=='false'")]
        public bool IsOrderActive { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BillDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            BillNumber = modelDto.BillNumber;
            BillDate = modelDto.BillDate;
            BeginDistributionDate = modelDto.BeginDistributionDate;
            EndDistributionDate = modelDto.EndDistributionDate;
            PaymentDatePlan = modelDto.PaymentDatePlan;
            PayablePlan = modelDto.PayablePlan;
            Comment = modelDto.Comment;
            IsOrderActive = modelDto.IsOrderActive;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new BillDomainEntityDto
                {
                    Id = Id,
                    BillNumber = BillNumber,
                    BillDate = BillDate,
                    BeginDistributionDate = BeginDistributionDate,
                    EndDistributionDate = EndDistributionDate,
                    PaymentDatePlan = PaymentDatePlan,
                    PayablePlan = PayablePlan,
                    Comment = Comment,
                    IsOrderActive = IsOrderActive,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
   }
}