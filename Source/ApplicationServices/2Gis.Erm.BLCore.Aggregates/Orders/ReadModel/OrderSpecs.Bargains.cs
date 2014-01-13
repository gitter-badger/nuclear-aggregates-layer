using System;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class Bargains
        {
            public static class Find
            {
                public static FindSpecification<Bargain> NonClosed
                {
                    get
                    {
                        return new FindSpecification<Bargain>(x => x.ClosedOn == null);
                    }
                }

                public static FindSpecification<Bargain> Actual(long? legalPersonId, long? branchOfficeOrganizationUnitId, DateTime orderSignupDate)
                {
                    return new FindSpecification<Bargain>(bargain => !bargain.IsDeleted
                                                                     && bargain.CustomerLegalPersonId == legalPersonId
                                                                     && bargain.ExecutorBranchOfficeId == branchOfficeOrganizationUnitId &&
                                                                     (bargain.ClosedOn == null || bargain.ClosedOn >= orderSignupDate) &&
                                                                     bargain.IsActive && !bargain.IsDeleted);
                }

                public static FindSpecification<Bargain> ForOrder(long? legalPersonId, long? branchOfficeOrganizationUnitId)
                {
                    return new FindSpecification<Bargain>(bargain => !bargain.IsDeleted
                                                                     && bargain.CustomerLegalPersonId == legalPersonId
                                                                     && bargain.ExecutorBranchOfficeId == branchOfficeOrganizationUnitId &&
                                                                     bargain.IsActive && !bargain.IsDeleted);
                }
            }
        }
    }
}
