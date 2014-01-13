using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Withdrawals
{
    public class WithdrawalInfoSpecifications
    {
        public static ISelectSpecification<IQueryable<WithdrawalInfo>, WithdrawalStatus> SelectLastWithdrawal()
        {
            return new SelectSpecification<IQueryable<WithdrawalInfo>, WithdrawalStatus>(
                x => x.OrderByDescending(y => y.StartDate)
                         .Select(y => (WithdrawalStatus) y.Status)
                         .FirstOrDefault());
        }
    }
}