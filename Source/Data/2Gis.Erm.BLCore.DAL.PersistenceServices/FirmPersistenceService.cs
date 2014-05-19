using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public class FirmPersistenceService : IFirmPersistenceService
    {
        // fixme {all}: installation-depended string, НЕ должно быть ресурсом, поскольку не зависит от локали пользователя
        // done {all, 10.10.2013}: вынес в конфиг

        private readonly IDatabaseCaller _databaseCaller;

        public FirmPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public void ImportFirmPromising(long organizationUnitDgppId, long modifiedBy, int timeout, bool enableReplication)
        {
            _databaseCaller.ExecuteProcedure("Integration.ImportFirmPromising",
                                             timeout,
                                             new Tuple<string, object>("OrganizationUnitStableId", organizationUnitDgppId),
                                             new Tuple<string, object>("ModifiedBy", modifiedBy),
                                             new Tuple<string, object>("EnableReplication", enableReplication));
        }

        public void ReplicateObjectsAfterImportCards(int timeout)
        {
            _databaseCaller.ExecuteProcedure("Integration.ReplicateObjectsAfterImportCards", timeout);
        }

        public IEnumerable<long> ImportCardsFromXml(string cardsXml,
                                                    long modifiedBy,
                                                    long ownerCode,
                                                    int timeout,
                                                    long[] pregeneratedIds,
                                                    string regionalTerritoryLocaleSpecificWord)
        {
            return _databaseCaller.ExecuteProcedureWithPreeneratedIdsAndSelectListOf<long>("Integration.ImportCardsFromXml",
                                                                                           timeout,
                                                                                           pregeneratedIds,
                                                                                           new Tuple<string, object>("Doc", cardsXml),
                                                                                           new Tuple<string, object>("ModifiedBy", modifiedBy),
                                                                                           new Tuple<string, object>("OwnerCode", ownerCode),
                                                                                           new Tuple<string, object>("RegionalTerritoryLocalName",
                                                                                                                     regionalTerritoryLocaleSpecificWord));
        }

        public void ImportFirmFromXml(string firmXml,
                                      long modifiedBy,
                                      long ownerCode,
                                      int timeout,
                                      bool enableReplication,
                                      string regionalTerritoryLocaleSpecificWord)
        {
            _databaseCaller.ExecuteProcedure("Integration.ImportFirmFromXml",
                                             timeout,
                                             new Tuple<string, object>("Xml", firmXml),
                                             new Tuple<string, object>("ModifiedBy", modifiedBy),
                                             new Tuple<string, object>("OwnerCode", ownerCode),
                                             new Tuple<string, object>("EnableReplication", enableReplication),
                                             new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord));
        }

        public void UpdateBuildings(string buildingsXml, int timeout, string regionalTerritoryLocaleSpecificWord, bool enableReplication)
        {
            _databaseCaller.ExecuteProcedure("Integration.UpdateBuildings",
                                             timeout,
                                             new Tuple<string, object>("buildingsXml", buildingsXml),
                                             new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord),
                                             new Tuple<string, object>("EnableReplication", enableReplication));
        }

        public void DeleteBuildings(string codesXml, int timeout)
        {
            _databaseCaller.ExecuteProcedure("Integration.DeleteBuildings",
                                             timeout,
                                             new Tuple<string, object>("codesXml", codesXml));
        }
    }
}