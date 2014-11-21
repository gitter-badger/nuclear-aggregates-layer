using System;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO
{
    public sealed class AccountDetailInfoToSendNotificationDto
    {
        public string LegalPersonName { get; set; }
        public string BranchOfficeName { get; set; }
        public string OperationName { get; set; }
        public bool IsPlusOperation { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public long AccountId { get; set; }
        public long AccountOwnerCode { get; set; }
    }
}