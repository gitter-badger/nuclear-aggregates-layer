using System;
using System.Collections.Generic;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public class FirmPersistenceService : IFirmPersistenceService
    {
        // fixme {all}: installation-depended string, НЕ должно быть ресурсом, поскольку не зависит от локали пользователя
        // done {all, 10.10.2013}: вынес в конфиг

        private readonly IClientProxyFactory _clitnProxyFactory;

        private readonly IDatabaseCaller _databaseCaller;

        public FirmPersistenceService(IDatabaseCaller databaseCaller, IClientProxyFactory clitnProxyFactory)
        {
            _databaseCaller = databaseCaller;
            _clitnProxyFactory = clitnProxyFactory;
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

        public IEnumerable<long> ImportCardsFromXml(string cardsXml, long modifiedBy, long ownerCode, int timeout, int pregeneratedIdsAmount, string regionalTerritoryLocaleSpecificWord)
        {
            var ids = _clitnProxyFactory.GetClientProxy<IIdentityProviderApplicationService, WSHttpBinding>()
                                        .Execute(x => x.GetIdentities(pregeneratedIdsAmount));

            return _databaseCaller.ExecuteProcedureWithPreeneratedIdsAndSelectListOf<long>(
                "Integration.ImportCardsFromXml",
                                             timeout,
                                             ids,
                                             new Tuple<string, object>("Doc", cardsXml),
                                             new Tuple<string, object>("ModifiedBy", modifiedBy),
                                             new Tuple<string, object>("OwnerCode", ownerCode),
                                             new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord)); 
        }

        public void ImportFirmFromXml(string firmXml, long modifiedBy, long ownerCode, int timeout, bool enableReplication, string regionalTerritoryLocaleSpecificWord)
        {
            _databaseCaller.ExecuteProcedure("Integration.ImportFirmFromXml",
                                             timeout,
                                             new Tuple<string, object>("Xml", firmXml),
                                             new Tuple<string, object>("ModifiedBy", modifiedBy),
                                             new Tuple<string, object>("OwnerCode", ownerCode),
                                             new Tuple<string, object>("EnableReplication", enableReplication),
                                             new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord));
        }

        public void UpdateBuildings(string buildingsXml, int timeout, string regionalTerritoryLocaleSpecificWord)
        {
            _databaseCaller.ExecuteProcedure("Integration.UpdateBuildings",
                                             timeout,
                                             new Tuple<string, object>("buildingsXml", buildingsXml),
                                             new Tuple<string, object>("RegionalTerritoryLocalName", regionalTerritoryLocaleSpecificWord));
        }

        public void DeleteBuildings(string codesXml, int timeout)
        {
            _databaseCaller.ExecuteProcedure("Integration.DeleteBuildings",
                                             timeout,
                                             new Tuple<string, object>("codesXml", codesXml));
        }
    }
}