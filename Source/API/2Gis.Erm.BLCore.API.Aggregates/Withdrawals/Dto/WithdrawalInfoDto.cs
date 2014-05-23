using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals.ReadModel
{
    public class WithdrawalInfoDto
    {
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public WithdrawalStatus WithdrawalStatus { get; set; }
    }
}