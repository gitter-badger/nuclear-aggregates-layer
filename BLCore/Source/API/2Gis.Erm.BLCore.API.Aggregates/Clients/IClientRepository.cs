using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients
{
    public interface IClientRepository : IAggregateRootRepository<Client>,
                                         IAssignAggregateRepository<Client>,
                                         IAssignAggregateRepository<Contact>,
                                         IQualifyAggregateRepository<Client>,
                                         IDisqualifyAggregateRepository<Client>,
                                         ICheckAggregateForDebtsRepository<Client>,
                                         IChangeAggregateTerritoryRepository<Client>
    {
        int SetMainFirm(Client client, long? mainFirmId);
        int Assign(Client client, long ownerCode);
        int AssignWithRelatedEntities(long clientId, long ownerCode, bool isPartialAssign);
        int Qualify(Client client, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate);
        int Disqualify(Client client, long currentUserCode, long reserveCode, bool bypassValidation, DateTime disqualifyDate);
        int ChangeTerritory(Client client, long territoryId);
        void ChangeTerritory(IEnumerable<Client> clients, long territoryId);
        void ValidateOwnerIsNotReserve(Client client);
        Tuple<Client, Client> MergeErmClients(long mainClientId, long appendedClientId, Client masterClient, bool assignAllObjects);
        void CalculatePromising(long modifiedBy);
        IEnumerable<Client> GetClientsByTerritory(long territoryId);
        void CreateOrUpdate(Contact contact);

        [Obsolete("Используется только в DgppImportFirmsHandler")]
        int HideFirm(long firmId);
    }
}