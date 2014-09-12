using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    public class MsCrmReplicationMetadataProvider : IMsCrmReplicationMetadataProvider
    {
        private static readonly IReadOnlyCollection<EntityReplicationInfo> ReplicationMetadata;

        private readonly IReadOnlyDictionary<Type, EntityReplicationInfo[]> _asyncMetadata;
        private readonly IReadOnlyDictionary<Type, EntityReplicationInfo[]> _syncMetadata;

        static MsCrmReplicationMetadataProvider()
        {
            // TODO {a.tukaev, 25.08.2014}: Лучше бы сделать эти настройки на основе подсистемы метаданных, см IMetadataElement, IMetadataSource, IMetadataProcessor
            ReplicationMetadata = EntityReplicationMetadata.Config
                                                           .Single<OrganizationUnit>("Billing")
                                                           .Then.Single<Currency>("Billing")
                                                           .Then.Single<Category>("BusinessDirectory")
                                                            // COMMENT {a.tukaev, 25.08.2014}: Верно ли, что для Territory и Contact - сначала Single, потом Batch, но для Firm и FirmAddress - наоборот?
                                                            // COMMENT {d.ivanov, 26.08.2014}: Порядок следования Single и Batch процедур не важен для вызывающего кода, 
                                                            //                                 т.к. из пары Single и Batch отдается только одна запись в зависимости от preferredMode
                                                           .Then.Single<Territory>("BusinessDirectory")
                                                           .Then.Batch<Territory>("BusinessDirectory", "ReplicateTerritories")
                                                           .Then.Single<Client>("Billing")
                                                           .Then.Batch<Firm>("BusinessDirectory", "ReplicateFirms")
                                                           .Then.Single<Firm>("BusinessDirectory")
                                                           .Then.Batch<FirmAddress>("BusinessDirectory", "ReplicateFirmAddresses")
                                                           .Then.Single<FirmAddress>("BusinessDirectory")
                                                           .Then.Single<Contact>("Billing")
                                                           .Then.Batch<Contact>("Billing", "ReplicateContacts")
                                                           .Then.Single<Position>("Billing")
                                                           .Then.Single<BranchOffice>("Billing")
                                                           .Then.Single<BranchOfficeOrganizationUnit>("Billing")
                                                           .Then.Single<LegalPerson>("Billing")
                                                           .Then.Single<Account>("Billing")
                                                           .Then.Single<OperationType>("Billing")
                                                           .Then.Single<AccountDetail>("Billing")
                                                           .Then.Single<Deal>("Billing")
                                                           .Then.Single<Limit>("Billing")
                                                           .Then.Single<Order>("Billing")
                                                           .Then.Single<OrderPosition>("Billing")
                                                           .Then.Single<Bargain>("Billing")
                                                           .Then.Single<OrderProcessingRequest>("Billing")
                                                           .Then.Single<User>("Security")
                                                           .Then.Single<UserTerritory>("Security")
                                                           .Freeze();
        }

        public MsCrmReplicationMetadataProvider(IEnumerable<Type> asyncTypes, IEnumerable<Type> syncTypes)
        {
            _asyncMetadata = ReplicationMetadata.Join(asyncTypes, x => x.EntityType, x => x, (info, type) => info)
                                                .GroupBy(x => x.EntityType)
                                                .ToDictionary(x => x.Key, x => x.ToArray());

            _syncMetadata = ReplicationMetadata.Join(syncTypes, x => x.EntityType, x => x, (info, type) => info)
                                               .GroupBy(x => x.EntityType)
                                               .ToDictionary(x => x.Key, x => x.ToArray());
        }

        public bool TryGetAsyncMetadata(Type entityType, ReplicationMode preferredMode, out EntityReplicationInfo replicationInfo)
        {
            replicationInfo = null;

            EntityReplicationInfo[] metadata;
            if (_asyncMetadata.TryGetValue(entityType, out metadata))
            {
                replicationInfo = metadata.FirstOrDefault(x => x.ReplicationMode == preferredMode) ?? metadata[0];
            }

            return replicationInfo != null;
        }

        public bool TryGetSyncMetadata(Type entityType, out EntityReplicationInfo replicationInfo)
        {
            replicationInfo = null;

            EntityReplicationInfo[] metadata;
            if (_syncMetadata.TryGetValue(entityType, out metadata))
            {
                replicationInfo = metadata.FirstOrDefault(x => x.ReplicationMode == ReplicationMode.Single);
            }

            return replicationInfo != null;
        }

        public IEnumerable<Type> GetAsyncReplicationTypeSequence()
        {
            return _asyncMetadata.Select(x => x.Key);
        }
    }
}