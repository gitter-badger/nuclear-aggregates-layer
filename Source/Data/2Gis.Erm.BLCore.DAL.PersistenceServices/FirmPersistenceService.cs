using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Utils;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public sealed class FirmPersistenceService : IFirmPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public FirmPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public IEnumerable<long> ImportFirmPromising(long organizationUnitDgppId, long modifiedBy, TimeSpan timeout)
        {
            return _databaseCaller.ExecuteProcedureWithResultSequenceOf<long>(
                                        "Integration.ImportFirmPromising",
                                        timeout,
                                        new Tuple<string, object>("OrganizationUnitStableId", organizationUnitDgppId),
                                        new Tuple<string, object>("ModifiedBy", modifiedBy));
        }

        public void ReplicateObjectsAfterImportCards(TimeSpan timeout)
        {
            _databaseCaller.ExecuteProcedure("Integration.ReplicateObjectsAfterImportCards", timeout);
        }

        public EntityChangesContext ImportCardsFromXml(
            string cardsXml, 
            long modifiedBy, 
            long ownerCode, 
            TimeSpan timeout,
            long[] pregeneratedIds, 
            string regionalTerritoryLocaleSpecificWord)
        {
            var changedEntitiesReport = _databaseCaller.ExecuteProcedureWithResultTable(
                                                            "Integration.ImportCardsFromXml",
                                                            timeout,
                                                            new Tuple<string, object>("Doc", cardsXml),
                                                            new Tuple<string, object>("ModifiedBy", modifiedBy),
                                                            new Tuple<string, object>("OwnerCode", ownerCode),
                                                            new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord),
                                                            new Tuple<string, object>("PregenaratedIds", pregeneratedIds.ToIdsContainer()));
            return changedEntitiesReport.ToEntityChanges();
        }

        public EntityChangesContext ImportFirmFromXml(string firmXml, long modifiedBy, long ownerCode, TimeSpan timeout, bool enableReplication, string regionalTerritoryLocaleSpecificWord)
        {
            var changedEntitiesReport = _databaseCaller.ExecuteProcedureWithResultTable(
                                                            "Integration.ImportFirmFromXml",
                                                            timeout,
                                                            new Tuple<string, object>("Xml", firmXml),
                                                            new Tuple<string, object>("ModifiedBy", modifiedBy),
                                                            new Tuple<string, object>("OwnerCode", ownerCode),
                                                            new Tuple<string, object>("EnableReplication", enableReplication),
                                                            new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord));
            return changedEntitiesReport.ToEntityChanges();
        }

        public IEnumerable<long> UpdateBuildings(string buildingsXml, TimeSpan timeout, string regionalTerritoryLocaleSpecificWord, bool enableReplication, bool useWarehouseIntegration)
        {
            return _databaseCaller.ExecuteProcedureWithResultSequenceOf<long>(
                                        "Integration.UpdateBuildings",
                                        timeout,
                                        new Tuple<string, object>("buildingsXml", buildingsXml),
                                        new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord),
                                        new Tuple<string, object>("EnableReplication", enableReplication),
                                        new Tuple<string, object>("UseWarehouseIntegration", useWarehouseIntegration));
        }

        public void DeleteBuildings(string codesXml, TimeSpan timeout)
        {
            _databaseCaller.ExecuteProcedure(
                                "Integration.DeleteBuildings",
                                timeout,
                                new Tuple<string, object>("codesXml", codesXml));
        }
    }
}