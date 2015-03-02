using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    public interface IGetDebitsInfoInitialForExportOperationService : IOperation<GetDebitsInfoInitialForExportIdentity>
    {
        IDictionary<long, DebitsInfoInitialDto> Get(DateTime startPeriodDate, DateTime endPeriodDate, IEnumerable<long> organizationUnitIds);
    }
}