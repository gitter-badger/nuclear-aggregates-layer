using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    public interface IGetAccountDetailsForExportContentOperationService : IOperation<GetAccountDetailsForExportContentIdentity>
    {
        IEnumerable<IntegrationResponse> Get(DateTime startPeriodDate, DateTime endPeriodDate, IEnumerable<long> organizationUnitIds);
    }
}