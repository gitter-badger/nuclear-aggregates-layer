using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    // FIXME {all, 26.02.2014}: при разделении на create и update usecase учесть логику этого класса, через метаданные, либо явно через create и update
    [Obsolete]
    public class ModifyWithdrawalInfoOperationService : IModifyBusinessModelEntityService<WithdrawalInfo>
    {
        // Virtual for interception
        public virtual long Modify(IDomainEntityDto domainEntityDto)
        {
            throw new InvalidOperationException("WithdrawalInfo can't be directly created. Elements can be modified through the withdrawing and revert withdrawing operations");
        }
    }
}