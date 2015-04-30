using DoubleGis.Erm.Platform.API.Core;
using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel
{
    public static partial class AccountSpecs
    {
        public static class Limits
        {
            public static class Find
            {
                public static FindSpecification<Limit> ForAccount(long accountId)
                {
                    return new FindSpecification<Limit>(x => x.AccountId == accountId);
                }

                public static FindSpecification<Limit> ApprovedForPeriod(TimePeriod period)
                {
                    return new FindSpecification<Limit>(x =>
                                                        x.Status == LimitStatus.Approved
                                                        && x.StartPeriodDate == period.Start
                                                        && x.EndPeriodDate == period.End);
                }
            }
        }
    }
}