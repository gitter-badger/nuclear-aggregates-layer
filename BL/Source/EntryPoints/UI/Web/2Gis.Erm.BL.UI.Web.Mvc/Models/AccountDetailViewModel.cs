using System;
using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AccountDetailViewModel : EntityViewModelBase<AccountDetail>
    {
        // Лицевой счет
        public long? AccountId { get; set; }

        // Вид операции
        [RequiredLocalized]
        public LookupField OperationType { get; set; }

        // Наименование
        [StringLengthLocalized(200)]
        public string Description { get; set; }

        // Дата проведения
        public DateTime TransactionDate { get; set; }

        // Сумма
        [RequiredLocalized]
        [RegularExpression(@"^([\d]((.|,)[\d]{1,})+|[1-9][\d]*((.|,)[\d]{1,})?)$", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "MustBePositiveDigit")]
        public decimal? Amount { get; set; }

        public bool OwnerCanBeChanged { get; set; }

        public override byte[] Timestamp { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var accountDetailDto = (AccountDetailDomainEntityDto)domainEntityDto;


            Id = accountDetailDto.Id;
            AccountId = accountDetailDto.AccountRef.Id;
            OperationType = LookupField.FromReference(accountDetailDto.OperationTypeRef);
            Owner = LookupField.FromReference(accountDetailDto.OwnerRef);
            Description = accountDetailDto.Description;
            TransactionDate = accountDetailDto.TransactionDate;
            Amount = accountDetailDto.Amount;
            Timestamp = accountDetailDto.Timestamp;
            OwnerCanBeChanged = accountDetailDto.OwnerCanBeChanged;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (AccountId == null)
            {
                throw new ArgumentException("AccountId");
            }

            if (Amount == null)
            {
                throw new ArgumentException("Amount");
            }

            return new AccountDetailDomainEntityDto
                {
                    Id = Id,
                    AccountRef = new EntityReference(AccountId),
                    OperationTypeRef = OperationType.ToReference(),
                    Description = Description,
                    TransactionDate = TransactionDate,
                    Amount = Amount.Value,
                    OwnerRef = Owner.ToReference(),
                    OwnerCanBeChanged = OwnerCanBeChanged,
                    Timestamp = Timestamp
                };
        }
    }
}