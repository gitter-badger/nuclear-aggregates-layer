using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus
{
    public sealed class CyprusAccountViewModel : EntityViewModelBase<Account>, ICyprusAdapted
    {
        // ����������� ���� ��������� �����������
        [RequiredLocalized]
        public LookupField BranchOfficeOrganizationUnit { get; set; }

        // ����������� ���� �������
        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

        // ������
        public LookupField Currency { get; set; }

        // ������ (�� ��������� � ������� ������)
        public decimal AccountDetailBalance { get; set; }

        // ������ (� ������ ����������)
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
            LegalPesonSyncCode1C = accountDto.LegalPersonSyncCode1C;
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
                    LegalPersonSyncCode1C = LegalPesonSyncCode1C,
                    LockDetailBalance = LockDetailBalance,
                    OwnerCanBeChanged = OwnerCanBeChanged,
                    Timestamp = Timestamp,
                    IsActive = IsActive,
                    IsDeleted = IsDeleted
                };
        }
    }
}