using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class LimitViewModel : EntityViewModelBase<Limit>, ILimitStateAspect
    {
        [PresentationLayerProperty]
        public long AccountId { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("BranchOfficeOrganizationUnit")]
        public LookupField BranchOffice { get; set; }

        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

        [DisplayNameLocalized("Period")]
        public DateTime StartPeriodDate { get; set; }

        [DisplayNameLocalized("LimitCloseDate")]
        public DateTime? CloseDate { get; set; }

        [RequiredLocalized]
        public decimal Amount { get; set; }

        [RequiredLocalized, ExcludeZeroValue]
        public LimitStatus Status { get; set; }

        [StringLengthLocalized(255)]
        public string Comment { get; set; }

        [RequiredLocalized]
        public LookupField Inspector { get; set; }

        public bool HasEditPeriodPrivelege { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LimitDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            AccountId = modelDto.AccountRef.Id.Value;
            Amount = modelDto.Amount;
            LegalPerson = new LookupField { Key = modelDto.LegalPersonRef.Id, Value = modelDto.LegalPersonRef.Name };
            BranchOffice = new LookupField { Key = modelDto.BranchOfficeRef.Id, Value = modelDto.BranchOfficeRef.Name };
            Status = modelDto.Status;
            StartPeriodDate = modelDto.StartPeriodDate;
            Inspector = new LookupField { Key = modelDto.InspectorRef.Id, Value = modelDto.InspectorRef.Name };
            Comment = modelDto.Comment;
            CloseDate = modelDto.CloseDate;

            if (!string.IsNullOrEmpty(modelDto.ErrorMessage))
            {
                ViewConfig.ReadOnly = true;
                SetCriticalError(modelDto.ErrorMessage);
            }

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new LimitDomainEntityDto
                {
                    Id = Id,
                    AccountRef = new EntityReference(AccountId),
                    Amount = Amount,
                    LegalPersonRef = new EntityReference(LegalPerson.Key, LegalPerson.Value),
                    BranchOfficeRef = new EntityReference(BranchOffice.Key, BranchOffice.Value),      
                    Status = Status,
                    StartPeriodDate = StartPeriodDate,
                    InspectorRef = new EntityReference(Inspector.Key, Inspector.Value),
                    Comment = Comment,
                    CloseDate = CloseDate,
                    Timestamp = Timestamp
                };
        }
    }
}