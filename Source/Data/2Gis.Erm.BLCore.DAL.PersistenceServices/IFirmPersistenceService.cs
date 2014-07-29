using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IFirmPersistenceService : IPersistenceService<Firm>
    {
        IEnumerable<long> ImportFirmPromising(long organizationUnitDgppId, long modifiedBy, int timeout);
        void ReplicateObjectsAfterImportCards(int timeout);
        EntityChangesContext ImportCardsFromXml(string cardsXml, long modifiedBy, long ownerCode, int timeout, long[] pregeneratedIds, string regionalTerritoryLocaleSpecificWord);
        EntityChangesContext ImportFirmFromXml(string firmXml, long modifiedBy, long ownerCode, int timeout, bool enableReplication, string regionalTerritoryLocaleSpecificWord);
        IEnumerable<long> UpdateBuildings(string buildingsXml, int timeout, string regionalTerritoryLocaleSpecificWord, bool enableReplication);
        void DeleteBuildings(string codesXml, int timeout);
    }
}