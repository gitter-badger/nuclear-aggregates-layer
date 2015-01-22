using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Deal;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify
{
    public class ModifyDealService : IModifyBusinessModelEntityService<Deal>
    {
        private readonly IDealRepository _dealRepository;
        private readonly IGetBaseCurrencyService _currencyService;
        private readonly IGenerateDealNameService _generateDealNameService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IBusinessModelEntityObtainer<Deal> _dealObtainer;
        private readonly ICreateDealAggregateService _createDealAggregateService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public ModifyDealService(IDealRepository dealRepository,
                                 IGetBaseCurrencyService currencyService,
                                 IGenerateDealNameService generateDealNameService,
                                 IFirmReadModel firmReadModel,
                                 IClientReadModel clientReadModel,
                                 IOperationScopeFactory operationScopeFactory,
                                 IBusinessModelEntityObtainer<Deal> dealObtainer,
                                 ICreateDealAggregateService createDealAggregateService,
                                 ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _dealRepository = dealRepository;
            _currencyService = currencyService;
            _generateDealNameService = generateDealNameService;
            _firmReadModel = firmReadModel;
            _clientReadModel = clientReadModel;
            _operationScopeFactory = operationScopeFactory;
            _dealObtainer = dealObtainer;
            _createDealAggregateService = createDealAggregateService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var deal = _dealObtainer.ObtainBusinessModelEntity(domainEntityDto);

            if (deal.StartReason == ReasonForNewDeal.None)
            {
                throw new NotificationException(BLResources.PickReasonForNewDeal);
            }

            if (deal.AgencyFee.HasValue && (deal.AgencyFee < 0M || deal.AgencyFee > 100M))
            {
                throw new AgencyFeeFormatException(BLResources.AgencyFeePercentMustBeBetweenZeroAndOneHundred);
            }

            if (deal.AdvertisingCampaignBeginDate.HasValue && !deal.AdvertisingCampaignEndDate.HasValue)
            {
                throw new AdvertisingCampaignPeriodException(BLResources.AdvertisingCampaignEndDateMustBeSpecified);
            }

            if (deal.AdvertisingCampaignEndDate.HasValue && !deal.AdvertisingCampaignBeginDate.HasValue)
            {
                throw new AdvertisingCampaignPeriodException(BLResources.AdvertisingCampaignBeginDateMustBeSpecified);
            }

            if (deal.AdvertisingCampaignEndDate.HasValue && deal.AdvertisingCampaignBeginDate.HasValue &&
                deal.AdvertisingCampaignEndDate.Value < deal.AdvertisingCampaignBeginDate.Value)
            {
                throw new AdvertisingCampaignPeriodException(BLResources.AdvertisingCampaignEndDateMustNotBeLessThanBeginDate);
            }

            var client = _clientReadModel.GetClient(deal.ClientId);

            if (client.OwnerCode == _securityServiceUserIdentifier.GetReserveUserIdentity().Code)
            {
                throw new DealClientIsInReserveException(BLResources.DealClientIsInReserve);
            }

            if (deal.IsNew())
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Deal>())
                {
                    // if deal name not set, construct it from client and firm name
                    if (string.IsNullOrWhiteSpace(deal.Name))
                    {
                        var clientName = client.Name;
                        deal.Name = clientName;
                        if (deal.MainFirmId != null)
                        {
                            var firmName = _firmReadModel.GetFirmName(deal.MainFirmId.Value);
                            deal.Name = _generateDealNameService.GenerateDealName(clientName, firmName);
                        }
                    }

                    // set deal curency as default currency
                    deal.CurrencyId = _currencyService.GetBaseCurrency().Id;
                    _createDealAggregateService.Create(deal);

                    scope.Added(deal).Complete();
                }
            }
            else
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Deal>())
                {
                    _dealRepository.Update(deal);

                    scope.Updated(deal).Complete();
                }
            }

            return deal.Id;
        }
    }
}