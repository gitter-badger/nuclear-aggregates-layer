using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.Orders.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.Orders.ReadModel
{
    public interface IEmiratesOrderReadModel : IAggregateReadModel<Order>, IEmiratesAdapted
    {
        IEnumerable<OrderForAcceptanceReportDto> GetOrdersToGenerateAcceptanceReports(DateTime month, long organizationUnitId);
    }
}