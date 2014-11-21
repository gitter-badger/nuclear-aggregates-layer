using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IFirmPersistenceService : IPersistenceService<Firm>
    {
        IEnumerable<long> ImportFirmPromising(long organizationUnitDgppId, long modifiedBy, TimeSpan timeout);
        void ReplicateObjectsAfterImportCards(TimeSpan timeout);
        EntityChangesContext ImportCardsFromXml(string cardsXml, long modifiedBy, long ownerCode, TimeSpan timeout, long[] pregeneratedIds, string regionalTerritoryLocaleSpecificWord);
        EntityChangesContext ImportFirmFromXml(string firmXml, long modifiedBy, long ownerCode, TimeSpan timeout, bool enableReplication, string regionalTerritoryLocaleSpecificWord);
        IEnumerable<long> UpdateBuildings(string buildingsXml, TimeSpan timeout, string regionalTerritoryLocaleSpecificWord, bool enableReplication, bool useWarehouseIntegration);
        void DeleteBuildings(string codesXml, TimeSpan timeout);
    }
}