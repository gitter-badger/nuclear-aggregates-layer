using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IFirmPersistenceService : IPersistenceService<Firm>
    {
        void ImportFirmPromising(long organizationUnitDgppId, long modifiedBy, int timeout, bool enableReplication);
        void ReplicateObjectsAfterImportCards(int timeout);

        IEnumerable<long> ImportCardsFromXml(string cardsXml,
                                             long modifiedBy,
                                             long ownerCode,
                                             int timeout,
                                             long[] pregeneratedIds,
                                             string regionalTerritoryLocaleSpecificWord);

        void ImportFirmFromXml(string firmXml, long modifiedBy, long ownerCode, int timeout, bool enableReplication, string regionalTerritoryLocaleSpecificWord);
        void UpdateBuildings(string buildingsXml, int timeout, string regionalTerritoryLocaleSpecificWord, bool enableReplication);
        void DeleteBuildings(string codesXml, int timeout);
    }
}