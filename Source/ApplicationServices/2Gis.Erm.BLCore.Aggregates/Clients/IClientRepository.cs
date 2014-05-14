using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Clients.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients
{
    public interface IClientRepository : IAggregateRootRepository<Client>,
                                         IAssignAggregateRepository<Client>,
                                         IAssignAggregateRepository<Contact>,
                                         IQualifyAggregateRepository<Client>,
                                         IDisqualifyAggregateRepository<Client>,
                                         ICheckAggregateForDebtsRepository<Client>,
                                         IChangeAggregateTerritoryRepository<Client>
    {
        string GetClientName(long clientId);
        Client GetClient(long clientId);
        ClientReplicationDto GetClientReplicationData(long clientId);
        Client CreateFromFirm(Firm firm, long ownerCode);
        int SetMainFirm(Client client, long? mainFirmId);
        int Assign(Client client, long ownerCode);
        int AssignWithRelatedEntities(long clientId, long ownerCode, bool isPartialAssign);
        int Qualify(Client client, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate);
        int Disqualify(Client client, long currentUserCode, long reserveCode, bool bypassValidation, DateTime disqualifyDate);
        int ChangeTerritory(Client client, long territoryId);
        void ChangeTerritory(IEnumerable<Client> clients, long territoryId);
        void ValidateOwnerIsNotReserve(Client client);
        Tuple<Client, Client> MergeErmClients(long mainClientId, long appendedClientId, Client masterClient, bool assignAllObjects);
        void CalculatePromising();
        IEnumerable<Client> GetClientsByTerritory(long territoryId);
        void CreateOrUpdate(Client client);
        void CreateOrUpdate(Contact contact);
        int HideFirm(long firmId);
    }
}