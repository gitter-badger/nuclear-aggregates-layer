using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditDealHandler : RequestHandler<EditRequest<Deal>, EmptyResponse>
    {
        private readonly IDealRepository _dealRepository;
        private readonly IGetBaseCurrencyService _currencyService;

        public EditDealHandler(IDealRepository dealRepository, IGetBaseCurrencyService currencyService)
        {
            _dealRepository = dealRepository;
            _currencyService = currencyService;
        }

        protected override EmptyResponse Handle(EditRequest<Deal> request)
        {
            var deal = request.Entity;

            if (deal.StartReason == (int)ReasonForNewDeal.None)
            {
                throw new NotificationException(BLResources.PickReasonForNewDeal);
            }

            var dealInfo = _dealRepository.GetClientAndFirmForDealInfo(deal);

            // check that deal client exists
            if (dealInfo == null)
            {
                throw new NotificationException(BLResources.EditDealClientNotFound);
            }

            var client = dealInfo.Client;

            // check that main firm exists
            var mainFirm = dealInfo.MainFirm;
            if (deal.MainFirmId != null && mainFirm == null)
            {
                throw new NotificationException(BLResources.EditDealMainFirmNotFound);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                if (deal.Id == 0)
                {
                    // if deal name not set, construct it from client and firm name
                    if (string.IsNullOrWhiteSpace(deal.Name))
                    {
                        deal.Name = client.Name;

                        if (mainFirm != null)
                        {
                            deal.Name = (deal.Name + " - " + mainFirm.Name).Substring(0, 300);
                        }
                    }

                    // set deal curency as default currency
                    deal.CurrencyId = _currencyService.GetBaseCurrency().Id;
                    _dealRepository.Add(deal);
                }
                else
                {
                    _dealRepository.Update(deal);
                }
   
                transaction.Complete();
            }

            return Response.Empty;
        }
    }
}