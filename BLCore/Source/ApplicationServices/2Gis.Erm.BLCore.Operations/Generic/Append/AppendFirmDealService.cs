using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendFirmDealService : IAppendGenericEntityService<Firm, Deal>
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICreateFirmDealAggregateService _createAggregateService;
        private readonly IDealReadModel _dealReadModel;

        public AppendFirmDealService(IOperationScopeFactory scopeFactory, ICreateFirmDealAggregateService createAggregateService, IDealReadModel dealReadModel)
        {
            _scopeFactory = scopeFactory;
            _createAggregateService = createAggregateService;
            _dealReadModel = dealReadModel;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.DealIdOrFirmIdIsNotSpecified);
            }

            var dealId = appendParams.ParentId.Value;
            var firmId = appendParams.AppendedId.Value;

            var dealAndFirmNames = _dealReadModel.GetRelatedDealAndFirmNames(dealId, firmId);
            if (dealAndFirmNames != null)
            {
                throw new EntityIsNotUniqueException(typeof(FirmDeal), string.Format(BLResources.FirmDealRelationAlreadyExists, dealAndFirmNames.DealName, dealAndFirmNames.FirmName));
            }

            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, Deal, Firm>())
            {
                var newLink = new FirmDeal
                    {
                        DealId = dealId,
                        FirmId = firmId
                    };

                _createAggregateService.Create(newLink);
                scope.Added(newLink);
                scope.Complete();
            }
        }
    }
}