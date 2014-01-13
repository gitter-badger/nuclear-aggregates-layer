using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM
{
    // FIXME {all, 23.10.2013}: заявлен как simplified model cosumer, хотя вообще не использует сущности из simplified model - скорее это crosscuttingservice
    [Obsolete]
    public interface IMsCrmInteractionService : ISimplifiedModelConsumer
    {
        CrmUserDto GetCrmUserInfo(string userAccount);
        Dictionary<long, CrmUserDto> GetUserMappings(Dictionary<long, string> usersDomainMapping);
    }
}
