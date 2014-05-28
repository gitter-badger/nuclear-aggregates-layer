using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Dto
{
    public class WithdrawalInfoDto
    {
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public WithdrawalStatus WithdrawalStatus { get; set; }
    }
}