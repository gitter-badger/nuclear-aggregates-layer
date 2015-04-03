using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class OrderProcessingRequestViewModel : EntityViewModelBase<OrderProcessingRequest>
    {
        public LookupField Firm { get; set; }

        public LookupField LegalPerson { get; set; }

        public LookupField LegalPersonProfile { get; set; }

        [DisplayNameLocalized("OrderProcessingRequestBaseOrder")]
        public LookupField BaseOrder { get; set; }

        [DisplayNameLocalized("OrderProcessingRequestRenewedOrder")]
        public LookupField RenewedOrder { get; set; }

        public Guid ReplicationCode { get; set; }

        [DisplayNameLocalized("OrderProcessingRequestState")]
        public OrderProcessingRequestState State { get; set; }

        [DisplayNameLocalized("OrderProcessingRequestTitle")]
        public string Title { get; set; }

        public LookupField SourceOrganizationUnit { get; set; }

        [DisplayNameLocalized("DueDate")]
        public string DueDateString { get; set; }

        public DateTime DueDate { get; set; }

        public string Description { get; set; }

        public short ReleaseCountPlan { get; set; }

        public DateTime BeginDistributionDate { get; set; }

        // Определяет хватает ли у пользователя прав на создание заказа по заявке
        public bool CanCreateOrder { get; set; }

        public OrderProcessingRequestType RequestType { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrderProcessingRequestDomainEntityDto)domainEntityDto;

            Id = dto.Id;
            BaseOrder = LookupField.FromReference(dto.BaseOrderRef);
            BeginDistributionDate = dto.BeginDistributionDate;
            Description = dto.Description;
            DueDate = dto.DueDate;
            DueDateString = string.Format("{0} {1}", dto.DueDate.ToShortDateString(), dto.DueDate.ToShortTimeString());
            ReleaseCountPlan = dto.ReleaseCountPlan;
            Firm = LookupField.FromReference(dto.FirmRef);
            LegalPerson = LookupField.FromReference(dto.LegalPersonRef);
            LegalPersonProfile = LookupField.FromReference(dto.LegalPersonProfileRef);
            RenewedOrder = LookupField.FromReference(dto.RenewedOrderRef);
            ReplicationCode = dto.ReplicationCode;
            SourceOrganizationUnit = LookupField.FromReference(dto.SourceOrganizationUnitRef);
            State = dto.State;
            RequestType = dto.RequestType;
            
            Title = dto.Title;
            Timestamp = dto.Timestamp;
            IsActive = dto.IsActive;
            IsDeleted = dto.IsDeleted;

            Owner = LookupField.FromReference(dto.OwnerRef);
            CreatedBy = LookupField.FromReference(dto.CreatedByRef);
            ModifiedBy = LookupField.FromReference(dto.ModifiedByRef);
            CreatedOn = dto.CreatedOn;
            ModifiedOn = dto.ModifiedOn;
        }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new OrderProcessingRequestDomainEntityDto
                {
                    Id = Id,
                    BaseOrderRef = BaseOrder.ToReference(),
                    BeginDistributionDate = BeginDistributionDate,
                    Description = Description,
                    DueDate = DueDate,
                    FirmRef = Firm.ToReference(),
                    LegalPersonRef = LegalPerson.ToReference(),
                    LegalPersonProfileRef = LegalPersonProfile.ToReference(),
                    RenewedOrderRef = RenewedOrder.ToReference(),
                    ReplicationCode = ReplicationCode,
                    SourceOrganizationUnitRef = SourceOrganizationUnit.ToReference(),
                    State = State,
                    RequestType = RequestType,
                    Title = Title,
                    ReleaseCountPlan = ReleaseCountPlan,
                    Timestamp = Timestamp,
                    IsActive = IsActive,
                    IsDeleted = IsDeleted,
                    OwnerRef = Owner.ToReference(),
                    CreatedByRef = CreatedBy.ToReference(),
                    CreatedOn = CreatedOn,
                    ModifiedByRef = ModifiedBy.ToReference(),
                    ModifiedOn = ModifiedOn
                };
        }
    }
}