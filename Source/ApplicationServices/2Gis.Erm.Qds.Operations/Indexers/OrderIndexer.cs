using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class OrderIndexer :
        IEntityIndexer<Order>,
        IEntityIndexerIndirect<Order>,
        IDocumentIndexer<OrderGridDoc>
    {
        private readonly IFinder _finder;
        private readonly IElasticApi _elasticApi;
        private readonly ILocalizationSettings _localizationSettings;

        public OrderIndexer(IFinder finder, IElasticApi elasticApi, ILocalizationSettings localizationSettings)
        {
            _finder = finder;
            _elasticApi = elasticApi;
            _localizationSettings = localizationSettings;
        }

        void IEntityIndexer<Order>.IndexEntities(params long[] ids)
        {
            var orders = _finder.Find<Order>(x => ids.Contains(x.Id));
            var result = GetOrderGridDoc(orders);
            _elasticApi.Bulk(result);
        }

        void IEntityIndexerIndirect<Order>.IndexEntitiesIndirectly(IQueryable<Order> query)
        {
            var result = GetOrderGridDoc(query, true);
            _elasticApi.Bulk(result);
        }

        void IDocumentIndexer<OrderGridDoc>.IndexAllDocuments()
        {
            var orderes = _finder.FindAll<Order>();
            var result = GetOrderGridDoc(orderes);
            _elasticApi.Bulk(result);
            _elasticApi.Refresh(x => x.Index<OrderGridDoc>());
        }

        private IEnumerable<Func<BulkDescriptor, BulkDescriptor>> GetOrderGridDoc(IQueryable<Order> query, bool indirectly = false)
        {
            var dtos = query.Select(x => new
                {
                    x.Id,
                    x.Number,

                    x.BeginDistributionDate,
                    x.EndDistributionDatePlan,
                    x.EndDistributionDateFact,

                    x.IsActive,
                    x.IsDeleted,
                    x.HasDocumentsDebt,

                    x.ModifiedOn,
                    x.CreatedOn,

                    x.PayablePlan,
                    x.WorkflowStepId,

                    x.AmountToWithdraw,
                    x.AmountWithdrawn,
                });
            
            var bulkDescriptors = dtos
                .AsEnumerable()
                .Select((x, y) => new Func<BulkDescriptor, BulkDescriptor>(bulkDescriptor => bulkDescriptor
                .Index<OrderGridDoc>(bulkIndexDescriptor => bulkIndexDescriptor
                .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                .Object(new OrderGridDoc
                {
                    Id = x.Id.ToString(),
                    Number = x.Number,

                    BeginDistributionDate = x.BeginDistributionDate,
                    EndDistributionDatePlan = x.EndDistributionDatePlan,
                    EndDistributionDateFact = x.EndDistributionDateFact,

                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    HasDocumentsDebt = x.HasDocumentsDebt,

                    CreatedOn = x.CreatedOn,
                    ModifiedOn = x.ModifiedOn,

                    PayablePlan = (double)x.PayablePlan,
                    WorkflowStepId = x.WorkflowStepId,
                    WorkflowStep = ((OrderState)x.WorkflowStepId).ToStringLocalized(EnumResources.ResourceManager, _localizationSettings.ApplicationCulture),

                    AmountToWithdraw = (double)x.AmountToWithdraw,
                    AmountWithdrawn = (double)x.AmountWithdrawn,

                }))));

            return bulkDescriptors;
        }
    }
}