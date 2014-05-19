using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel
{
    public interface IFirmReadModel : IAggregateReadModel<Firm>
    {
        IReadOnlyDictionary<Guid, FirmWithAddressesAndProjectDto> GetFirmInfosByCrmIds(IEnumerable<Guid> crmIds);
        long GetOrderFirmId(long orderId);
        IEnumerable<long> GetFirmNonArchivedOrderIds(long firmId);
        long GetOrgUnitId(long firmId);
        bool HasFirmClient(long firmId);
        IEnumerable<CategoryGroup> GetFirmAddressCategoryGroups(long firmAddressId);
        FirmAndClientDto GetFirmAndClientByFirmAddress(long firmAddressCode);
    }
}