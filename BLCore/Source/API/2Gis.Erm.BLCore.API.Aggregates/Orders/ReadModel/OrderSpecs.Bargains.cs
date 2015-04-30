using System;
using System.Linq;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class Bargains
        {
            public static class Find
            {
                public static FindSpecification<Bargain> NonClosed
                {
                    get { return new FindSpecification<Bargain>(x => x.ClosedOn == null); }
                }

                public static FindSpecification<Bargain> NotClosedByCertainDate(DateTime dateToCompare)
                {
                    return new FindSpecification<Bargain>(bargain => !bargain.ClosedOn.HasValue || bargain.ClosedOn >= dateToCompare);
                }

                public static FindSpecification<Bargain> ByLegalPersons(long legalPersonId, long branchOfficeOrganizationUnitId)
                {
                    return new FindSpecification<Bargain>(bargain => bargain.CustomerLegalPersonId == legalPersonId
                                                                     && bargain.ExecutorBranchOfficeId == branchOfficeOrganizationUnitId);
                }

                public static FindSpecification<Bargain> ByOrder(long orderId)
                {
                    return new FindSpecification<Bargain>(bargain => bargain.Orders.Any(order => order.Id == orderId));
                }

                public static FindSpecification<Bargain> ClientBargains()
                {
                    return new FindSpecification<Bargain>(x => x.BargainKind == BargainKind.Client);
                }

                public static FindSpecification<Bargain> AgentBargains()
                {
                    return new FindSpecification<Bargain>(x => x.BargainKind == BargainKind.Agent);
                }

                public static FindSpecification<Bargain> Duplicate(long bargainId,
                                                                   long legalPersonId,
                                                                   long branchOfficeOrganizationUnitId,
                                                                   DateTime bargainBeginDate,
                                                                   DateTime bargainEndDate)
                {
                    return
                        new FindSpecification<Bargain>(
                            x => x.Id != bargainId && x.CustomerLegalPersonId == legalPersonId && x.ExecutorBranchOfficeId == branchOfficeOrganizationUnitId &&
                                 x.BargainEndDate >= bargainBeginDate && x.SignedOn <= bargainEndDate);
                }
            }

            public static class Select
            {
            }
        }
    }
}