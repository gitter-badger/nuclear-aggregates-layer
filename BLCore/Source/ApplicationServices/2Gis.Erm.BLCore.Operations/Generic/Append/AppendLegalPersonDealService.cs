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
    public class AppendLegalPersonDealService : IAppendGenericEntityService<LegalPerson, Deal>
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICreateLegalPersonDealAggregateService _createAggregateService;
        private readonly IDealReadModel _dealReadModel;

        public AppendLegalPersonDealService(IOperationScopeFactory scopeFactory, ICreateLegalPersonDealAggregateService createAggregateService, IDealReadModel dealReadModel)
        {
            _scopeFactory = scopeFactory;
            _createAggregateService = createAggregateService;
            _dealReadModel = dealReadModel;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.DealIdOrLegalPersonIdIsNotSpecified);
            }

            var dealId = appendParams.ParentId.Value;
            var legalPersonId = appendParams.AppendedId.Value;

            var existingLinkInfo = _dealReadModel.GetRelatedDealAndLegalPersonNames(dealId, legalPersonId);
            if (existingLinkInfo != null)
            {
                throw new EntityIsNotUniqueException(typeof(LegalPersonDeal),
                                                     string.Format(BLResources.LegalPersonDealLinkAlreadyExists, existingLinkInfo.DealName, existingLinkInfo.LegalPersonName));
            }

            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, Deal, LegalPerson>())
            {
                var newLink = new LegalPersonDeal
                    {
                        DealId = dealId,
                        LegalPersonId = legalPersonId,
                        IsMain = !_dealReadModel.AreThereAnyLegalPersonsForDeal(dealId)
                    };

                _createAggregateService.Create(newLink);
                scope.Added(newLink);
                scope.Complete();
            }
        }
    }
}