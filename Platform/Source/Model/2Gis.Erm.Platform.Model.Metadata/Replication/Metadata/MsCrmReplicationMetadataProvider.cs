using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    public class MsCrmReplicationMetadataProvider : IMsCrmReplicationMetadataProvider
    {
        private static readonly IReadOnlyCollection<EntityReplicationInfo> ReplicationMetadata;

        private readonly IReadOnlyDictionary<Type, EntityReplicationInfo[]> _metadata;

        static MsCrmReplicationMetadataProvider()
        {
            // TODO {a.tukaev, 25.08.2014}: ����� �� ������� ��� ��������� �� ������ ���������� ����������, �� IMetadataElement, IMetadataSource, IMetadataProcessor
            ReplicationMetadata = EntityReplicationMetadata.Config
                                                           .Single<OrganizationUnit>("Billing")
                                                           .Then.Single<Currency>("Billing")
                                                           .Then.Single<Category>("BusinessDirectory")
                
                // COMMENT {a.tukaev, 25.08.2014}: ����� ��, ��� ��� Territory � Contact - ������� Single, ����� Batch, �� ��� Firm � FirmAddress - ��������?
                // COMMENT {d.ivanov, 26.08.2014}: ������� ���������� Single � Batch �������� �� ����� ��� ����������� ����, 
                // �.�. �� ���� Single � Batch �������� ������ ���� ������ � ����������� �� preferredMode
                                                           .Then.Batch<Territory>("BusinessDirectory", "ReplicateTerritories")
                                                           .Then.Batch<Client>("Billing", "ReplicateClients")
                                                           .Then.Batch<Firm>("BusinessDirectory", "ReplicateFirms")
                                                           .Then.Batch<FirmAddress>("BusinessDirectory", "ReplicateFirmAddresses")
                                                           .Then.Batch<Contact>("Billing", "ReplicateContacts")
                                                           .Then.Single<Position>("Billing")
                                                           .Then.Single<BranchOffice>("Billing")
                                                           .Then.Single<BranchOfficeOrganizationUnit>("Billing")
                                                           .Then.Batch<LegalPerson>("Billing", "ReplicateLegalPersons")
                                                           .Then.Batch<Account>("Billing", "ReplicateAccounts")
                                                           .Then.Single<OperationType>("Billing")
                                                           .Then.Batch<AccountDetail>("Billing", "ReplicateAccountDetails")
                                                           .Then.Batch<Deal>("Billing", "ReplicateDeals")
                                                           .Then.Batch<Limit>("Billing", "ReplicateLimits")
                                                           .Then.Batch<Order>("Billing", "ReplicateOrders")
                                                           .Then.Batch<OrderPosition>("Billing", "ReplicateOrderPositions")
                                                           .Then.Batch<Bargain>("Billing", "ReplicateBargains")
                                                           .Then.Single<OrderProcessingRequest>("Billing")
                                                           .Then.Single<User>("Security")
                                                           .Then.Single<UserTerritory>("Security")
                                                           .Then.Batch<Appointment>("Activity", "ReplicateAppointments")
                                                           .Then.Batch<Letter>("Activity", "ReplicateLetters")
                                                           .Then.Batch<Phonecall>("Activity", "ReplicatePhonecalls")
                                                           .Then.Batch<Task>("Activity", "ReplicateTasks")
                                                           .Freeze();
        }

        public MsCrmReplicationMetadataProvider(IEnumerable<Type> asyncTypes)
        {
            _metadata = ReplicationMetadata.Join(asyncTypes, x => x.EntityType, x => x, (info, type) => info)
                                           .GroupBy(x => x.EntityType)
                                           .ToDictionary(x => x.Key, x => x.ToArray());
        }

        public bool TryGetMetadata(Type entityType, ReplicationMode preferredMode, out EntityReplicationInfo replicationInfo)
        {
            replicationInfo = null;

            EntityReplicationInfo[] metadata;
            if (_metadata.TryGetValue(entityType, out metadata))
            {
                replicationInfo = metadata.FirstOrDefault(x => x.ReplicationMode == preferredMode) ?? metadata[0];
            }

            return replicationInfo != null;
        }

        public IEnumerable<Type> GetReplicationTypeSequence()
        {
            return _metadata.Select(x => x.Key);
        }
    }
}