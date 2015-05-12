using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.Orders.DTO;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.Emirates.Orders.ReadModel
{
    public interface IEmiratesOrderReadModel : IAggregateReadModel, IEmiratesAdapted
    {
        IEnumerable<OrderForAcceptanceReportDto> GetOrdersToGenerateAcceptanceReports(DateTime month, long organizationUnitId);
    }
}