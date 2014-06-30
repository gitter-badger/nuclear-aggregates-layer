using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class ClientIndexer :
        IEntityIndexer<Client>,
        IEntityIndexerIndirect<Client>,
        IDocumentIndexer<ClientGridDoc>
    {
        private readonly IFinder _finder;
        private readonly IElasticApi _elasticApi;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public ClientIndexer(IFinder finder, IElasticApi elasticApi, IDebtProcessingSettings debtProcessingSettings)
        {
            _finder = finder;
            _elasticApi = elasticApi;
            _debtProcessingSettings = debtProcessingSettings;
        }

        void IEntityIndexer<Client>.IndexEntities(params long[] ids)
        {
            var clients = _finder.Find<Client>(x => ids.Contains(x.Id));
            var result = GetClientGridDoc(clients);
            _elasticApi.Bulk(result);
        }

        void IEntityIndexerIndirect<Client>.IndexEntitiesIndirectly(IQueryable<Client> query)
        {
            var result = GetClientGridDoc(query, true);
            _elasticApi.Bulk(result);
        }

        void IDocumentIndexer<ClientGridDoc>.IndexAllDocuments()
        {
            var clients = _finder.FindAll<Client>();
            var result = GetClientGridDoc(clients);
            _elasticApi.Bulk(result);
            _elasticApi.Refresh(x => x.Index<ClientGridDoc>());
        }

        private IEnumerable<Func<BulkDescriptor, BulkDescriptor>> GetClientGridDoc(IQueryable<Client> query, bool indirectly = false)
        {
            var minDebtAmount = _debtProcessingSettings.MinDebtAmount;

            var accounts = _finder.FindAll<Account>().ToArray();

            var dtos = query.Select(x => new
            {
                x.Id,
                x.ReplicationCode,
                x.Name,
                x.MainAddress,
                x.TerritoryId,
                TerritoryName = x.Territory.Name,
                x.OwnerCode,
                x.IsActive,
                x.IsDeleted,
                x.CreatedOn,
                InformationSource = (InformationSource)x.InformationSource,
                x.Timestamp,
                x.MainFirmId,
                MainFirmName = x.Firm.Name,
                x.LastQualifyTime,
                x.LastDisqualifyTime,
                x.Territory.OrganizationUnitId,
                x.IsAdvertisingAgency,
                // ошибка с IPartable, потом разберёмся
                //x.LegalPersons,
                HasAccountDebt = x.LegalPersons.Where(y => y.IsActive && !y.IsDeleted)
                          .SelectMany(y => y.Accounts).Where(y => y.IsActive && !y.IsDeleted)
                          .Any(y => y.Balance < minDebtAmount),
            });

            // TODO: optimize
            var allUsers = _finder.FindAll<User>().ToArray();

            var bulkDescriptors = dtos
                .AsEnumerable()
                .Join(
                allUsers,
                x => x.OwnerCode,
                x => x.Id,
                (x, y) => new Func<BulkDescriptor, BulkDescriptor>(bulkDescriptor => bulkDescriptor
                        .Index<ClientGridDoc>(bulkIndexDescriptor => bulkIndexDescriptor
                            .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                            .Object(new ClientGridDoc
                    {
                        Id = x.Id.ToString(),
                        ReplicationCode = x.ReplicationCode.ToString(),
                        Name = x.Name,
                        MainAddress = x.MainAddress,
                        TerritoryId = x.TerritoryId.ToString(),
                        TerritoryName = x.TerritoryName,
                        OwnerCode = x.OwnerCode.ToString(),
                        OwnerName = y.DisplayName,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        CreatedOn = x.CreatedOn,
                        MainFirmId = x.MainFirmId.ToString(),
                        MainFirmName = x.MainFirmName,
                        LastQualifyTime = x.LastQualifyTime,
                        LastDisqualifyTime = x.LastDisqualifyTime,
                        InformationSourceEnum = x.InformationSource,
                        IsAdvertisingAgency = x.IsAdvertisingAgency,
                        // ошибка с IPartable, потом разберёмся
                        //LegalPersons = x.LegalPersons.Select(lp => CreateLPDoc(lp, accounts)).ToArray(),

                        Authorization = new DocumentAuthorization
                        {
                            Tags = new[]
                            {
                                "list/" + typeof(ClientGridDoc).Name,
                                "byUserId/" + x.OwnerCode,
                                "byDepartmentId/" + y.DepartmentId,
                                "byTerritoryId/" + x.TerritoryId,
                                "byOrganizationUnitId/" + x.OrganizationUnitId
                            },
                        },
                    }))));

            return bulkDescriptors;
        }

        LegalPersonDoc CreateLPDoc(LegalPerson lp, Account[] accounts)
        {
            return new LegalPersonDoc
                {
                    Id = lp.Id.ToString(),
                    IsActive = lp.IsActive,
                    IsDeleted = lp.IsDeleted,
                    Accounts = accounts.Where(acc => acc.LegalPersonId == lp.Id).Select(CreateAccDoc).ToArray()
                };
        }

        AccountDoc CreateAccDoc(Account acc)
        {
            return new AccountDoc
                {
                    Id = acc.Id.ToString(),
                    IsActive = acc.IsActive,
                    IsDeleted = acc.IsDeleted,
                    Balance = (double)acc.Balance
                };
        }
    }
}