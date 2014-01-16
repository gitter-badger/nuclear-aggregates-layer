using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.DAL;
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

            public static class Select
            {
                public static ISelectSpecification<Order, CreateBargainInfo> CreateBargainInfoSelector
                {
                    get
                    {
                        return new SelectSpecification<Order, CreateBargainInfo>(x => new CreateBargainInfo
                            {
                                Order = x,
                                LegalPersonId = x.LegalPersonId,
                                BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                                OrderSignupDate = x.SignupDate,
                                BargainTypeId = x.BranchOfficeOrganizationUnit.BranchOffice.BargainTypeId,
                                DestinationSyncCode1C = x.DestOrganizationUnit.SyncCode1C,
                                SourceSyncCode1C = x.SourceOrganizationUnit.SyncCode1C,
                            });
                    }
                }
            }
        }
    }
}
