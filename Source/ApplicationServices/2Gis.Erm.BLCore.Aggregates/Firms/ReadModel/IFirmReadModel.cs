using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel
{
    public interface IFirmReadModel : IAggregateReadModel<Firm>
    {
        IReadOnlyDictionary<Guid, FirmWithAddressesAndProjectDto> GetFirmInfosByCrmIds(IEnumerable<Guid> crmIds);
        bool HasFirmClient(long firmId);
    }
}