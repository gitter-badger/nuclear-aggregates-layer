using System;

using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class LockViewModel : EntityViewModelBase<Lock>, ILockViewModel
    {
        // ��. ���� ��������� �����������
        public LookupField BranchOfficeOrganizationUnit { get; set; }

        // �����
        [DisplayNameLocalized("OrderNumber")]
        public LookupField Order { get; set; }

        // ��. ���� �������
        public LookupField LegalPerson { get; set; }

        // ���� ��������
        public DateTime CreateDate { get; set; }

        // ���� ������ �������
        public DateTime PeriodStartDate { get; set; }

        // ���� ��������� �������
        public DateTime PeriodEndDate { get; set; }

        // ���� ��������
        [DisplayNameLocalized("CloseLockDate")]
        public DateTime? CloseDate { get; set; }

        // ����� ���������� (����)
        [DisplayNameLocalized("LockPlannedAmount")]
        public decimal PlannedAmount { get; set; }

        // �������������� ����� (�������)
        [DisplayNameLocalized("LockBalance")]
        public decimal Balance { get; set; }

        // �������������� ����� (�����������)
        [DisplayNameLocalized("LockClosedBalance")]
        public decimal? ClosedBalance { get; set; }

        // ��������
        [DisplayNameLocalized("OperationOnAccount")]
        public LookupField AccountDetail { get; set; }

        public string Status { get; set; }

        // hidden fields
        public long AccountId { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LockDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            AccountId = modelDto.AccountRef.Id.Value;
            BranchOfficeOrganizationUnit = new LookupField { Key = modelDto.BranchOfficeOrganizationUnitRef.Id, Value = modelDto.BranchOfficeOrganizationUnitRef.Name };
            LegalPerson = new LookupField { Key = modelDto.LegalPersonRef.Id, Value = modelDto.LegalPersonRef.Name };
            AccountDetail = new LookupField { Key = modelDto.DebitAccountDetailRef.Id, Value = modelDto.DebitAccountDetailRef.Name };
            Order = new LookupField { Key = modelDto.OrderRef.Id, Value = modelDto.OrderRef.Name };
            PeriodStartDate = modelDto.PeriodStartDate;
            PeriodEndDate = modelDto.PeriodEndDate;
            CreateDate = modelDto.CreatedOn;
            CloseDate = modelDto.CloseDate;
            Balance = modelDto.Balance;
            PlannedAmount = modelDto.PlannedAmount;
            ClosedBalance = modelDto.ClosedBalance;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            throw new NotSupportedException();
        }
    }
}