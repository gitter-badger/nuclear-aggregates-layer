using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Web.Mvc.Utils;
using DoubleGis.Erm.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;
using DoubleGis.Erm.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.UI.Web.Mvc.Models
{
    public sealed class CyprusAccountViewModel : EntityViewModelBase<Account>, ICyprusAdapted
    {
        // Юридическое лицо отделения организации
        [RequiredLocalized]
        public LookupField BranchOfficeOrganizationUnit { get; set; }

        // Юридическое лицо клиента
        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

        // Валюта
        public LookupField Currency { get; set; }

        // Баланс (по операциям с лицевым счетом)
        public decimal AccountDetailBalance { get; set; }

        // Баланс (с учетом блокировок)
        public decimal LockDetailBalance { get; set; }

        public string LegalPesonSyncCode1C { get; set; }

        public bool OwnerCanBeChanged { get; set; }

        public override byte[] Timestamp { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var accountDto = (AccountDomainEntityDto)domainEntityDto;

            Id = accountDto.Id;
            AccountDetailBalance = accountDto.AccountDetailBalance;
            BranchOfficeOrganizationUnit = LookupField.FromReference(accountDto.BranchOfficeOrganizationUnitRef);
            LegalPerson = LookupField.FromReference(accountDto.LegalPersonRef);
            Currency = LookupField.FromReference(accountDto.CurrencyRef);
            LegalPesonSyncCode1C = accountDto.LegalPesonSyncCode1C;
            LockDetailBalance = accountDto.LockDetailBalance;
            OwnerCanBeChanged = accountDto.OwnerCanBeChanged;
            Timestamp = accountDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AccountDomainEntityDto
                {
                    Id = Id,
                    AccountDetailBalance = AccountDetailBalance,
                    BranchOfficeOrganizationUnitRef = BranchOfficeOrganizationUnit.ToReference(),
                    LegalPersonRef = LegalPerson.ToReference(),
                    CurrencyRef = Currency.ToReference(),
                    LegalPesonSyncCode1C = LegalPesonSyncCode1C,
                    LockDetailBalance = LockDetailBalance,
                    OwnerCanBeChanged = OwnerCanBeChanged,
                    Timestamp = Timestamp,
                    IsActive = IsActive,
                    IsDeleted = IsDeleted
                };
        }
    }
}