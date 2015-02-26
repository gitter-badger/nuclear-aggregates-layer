using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo
{
    public interface IGetFirmInfoService : IOperation<GetFirmInfoIdentity>
    {
        IEnumerable<FirmInfoDto> GetFirmInfosByIds(IEnumerable<long> ids);
    }
}