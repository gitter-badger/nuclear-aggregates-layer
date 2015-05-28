using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms
{
    public interface IFirmRepository : IAggregateRootRepository<Firm>,
                                       IQualifyAggregateRepository<Firm>,
                                       IDisqualifyAggregateRepository<Firm>,
                                       IChangeAggregateTerritoryRepository<Firm>,
                                       IChangeAggregateClientRepository<Firm>,
                                       IAssignAggregateRepository<Firm>
    {
        int Assign(Firm firm, long ownerCode);
        Firm GetFirm(long firmId);
        void Update(Firm firm);
        int Qualify(Firm firm, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate);
        Client PerformDisqualificationChecks(long firmId, long currentUserCode);
        int Disqualify(Firm firm, long currentUserCode, long reserveCode, DateTime disqualifyDate);
        Client GetFirmClient(long firmId);
        int SetFirmClient(Firm firm, long clientId);
        int ChangeTerritory(Firm firm, long territoryId);
        int ChangeTerritory(IEnumerable<Firm> firm, long territoryId);
        IEnumerable<Firm> GetFirmsByTerritory(long territoryId);

        int? GetOrganizationUnitDgppId(long organizationUnitId);

        void ImportFirmPromisingValues(long userId);
        void UpdateFirmAddresses(IEnumerable<FirmAddress> syncFirmAddressesDtos);

        IReadOnlyCollection<long> GetAdvertisementIds(long firmId);

        bool IsTerritoryReplaceable(long oldTerritoryId, long newTerritoryId);
    }
}