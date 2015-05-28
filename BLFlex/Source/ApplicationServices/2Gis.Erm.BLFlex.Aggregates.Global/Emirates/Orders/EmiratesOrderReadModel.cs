using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.Orders.DTO;
using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.Orders.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.Orders
{
    public class EmiratesOrderReadModel : IEmiratesOrderReadModel
    {
        private readonly ISecureFinder _secureFinder;

        public EmiratesOrderReadModel(ISecureFinder secureFinder)
        {
            _secureFinder = secureFinder;
        }

        public IEnumerable<OrderForAcceptanceReportDto> GetOrdersToGenerateAcceptanceReports(DateTime month, long organizationUnitId)
        {
            return _secureFinder.Find(OrderSpecs.Orders.Find.BySourceOrganizationUnit(organizationUnitId) &&
                                      OrderSpecs.Orders.Find.ByEndDistributionDateFact(month) &&
                                      OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved, OrderState.Archive, OrderState.OnTermination) &&
                                      Specs.Find.ActiveAndNotDeleted<Order>())
                                .Map(q => q.Select(x => new OrderForAcceptanceReportDto
                                    {
                                        OrderId = x.Id,
                                        ProfileId = x.LegalPersonProfileId,
                                        OrderNumber = x.Number
                                    }))
                                .Many();
        }
    }
}