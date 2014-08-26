using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class DealViewModel : EntityViewModelBase<Deal>
    {
        [Dependency(DependencyType.Required, "Name", "this.value != 0")]
        public override long Id { get; set; }

        public Guid ReplicationCode { get; set; }

        [StringLengthLocalized(300)]
        [DisplayNameLocalized("DealName")]
        public string Name { get; set; }

        public DealStage DealStage { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        public DateTime? CloseDate { get; set; }

        [RequiredLocalized]
        public ReasonForNewDeal StartReason { get; set; }

        public CloseDealReason CloseReason { get; set; }

        [StringLengthLocalized(256)]
        public string CloseReasonOther { get; set; }

        public LookupField Currency { get; set; }

        public override byte[] Timestamp { get; set; }

        // Куратор
        [RequiredLocalized]
        public override LookupField Owner { get; set; }
        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        // Клиент
        [RequiredLocalized]
        [Dependency(DependencyType.Transfer, "ClientReplicationCode", "(this.item && this.item.data)?this.item.data.ReplicationCode:undefined;")]
        [Dependency(DependencyType.Disable, "MainFirm", "this.getValue()==undefined")]
        public LookupField Client { get; set; }
        
        public Guid? ClientReplicationCode { get; set; }
        
        public string ClientName { get; set; }

        // Фирма
        public LookupField MainFirm { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (DealDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            ReplicationCode = modelDto.ReplicationCode;

            Name = modelDto.Name;
            ClientReplicationCode = modelDto.ClientReplicationCode;
            CloseDate = modelDto.CloseDate;
            CloseReasonOther = modelDto.CloseReasonOther;
            Comment = modelDto.Comment;
            CreatedOn = modelDto.CreatedOn;

            CloseReason = modelDto.CloseReason;
            DealStage = modelDto.DealStage;
            StartReason = modelDto.StartReason;

            MainFirm = LookupField.FromReference(modelDto.MainFirmRef);
            Client = LookupField.FromReference(modelDto.ClientRef);
            Currency = LookupField.FromReference(modelDto.CurrencyRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            var dto = new DealDomainEntityDto
                {
                    Id = Id,
                    ReplicationCode = ReplicationCode,

                    Name = Name,
                    ClientReplicationCode = ClientReplicationCode.Value,
                    CloseDate = CloseDate,
                    CloseReasonOther = CloseReasonOther,
                    Comment = Comment,
                    CreatedOn = CreatedOn,
                    CloseReason = CloseReason,
                    DealStage = DealStage,
                    StartReason = StartReason,
                    MainFirmRef = MainFirm.ToReference(),
                    CurrencyRef = Currency.ToReference(),
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };

            if (Client.Key != null)
            {
                dto.ClientRef = Client.ToReference();
            }

            return dto;
        }
    }
}