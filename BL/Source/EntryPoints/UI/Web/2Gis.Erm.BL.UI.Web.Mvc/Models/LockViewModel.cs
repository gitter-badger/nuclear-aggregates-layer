using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class LockViewModel : EntityViewModelBase<Lock>
    {
        // Юр. лицо отделения организации
        public LookupField BranchOfficeOrganizationUnit { get; set; }

        // Заказ
        [DisplayNameLocalized("OrderNumber")]
        public LookupField Order { get; set; }

        // Юр. лицо клиента
        public LookupField LegalPerson { get; set; }

        // Дата создания
        public DateTime CreateDate { get; set; }

        // Дата начала периода
        public DateTime PeriodStartDate { get; set; }

        // Дата окончания периода
        public DateTime PeriodEndDate { get; set; }

        // Дата закрытия
        [DisplayNameLocalized("CloseLockDate")]
        public DateTime? CloseDate { get; set; }

        // Сумма блокировки (план)
        [DisplayNameLocalized("LockPlannedAmount")]
        public decimal PlannedAmount { get; set; }

        // Использованная сумма (текущая)
        [DisplayNameLocalized("LockBalance")]
        public decimal Balance { get; set; }

        // Использованная сумма (фактическая)
        [DisplayNameLocalized("LockClosedBalance")]
        public decimal? ClosedBalance { get; set; }

        // Операция
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