using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountBulkCreateLockDetailsAggregateService : IAggregateSpecificOperation<Account, BulkCreateIdentity>
    {
        void Create(IReadOnlyCollection<CreateLockDetailDto> createLockDetailDtos);
    }
}