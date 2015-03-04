using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.AdsManagement
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class HandleAdsStateApplicationService : IHandleAdsStateApplicationService, IHandleAdsStateApplicationRestService
    {
        private readonly ITracer _logger;
        private readonly IChangeAdvertisementElementStatusOperationService _changeAdvertisementElementStatusOperationService;

        public HandleAdsStateApplicationService(ITracer logger,
                                                IChangeAdvertisementElementStatusOperationService changeAdvertisementElementStatusOperationService)
        {
            _logger = logger;
            _changeAdvertisementElementStatusOperationService = changeAdvertisementElementStatusOperationService;
        }

        public void TransferToDraft(string adsElementId)
        {
            ChangeStatusRest(adsElementId, AdvertisementElementStatusValue.Draft);
        }

        public void TransferToDraft(long adsElementId)
        {
            ChangeStatusSoap(adsElementId,  AdvertisementElementStatusValue.Draft);
        }

        public void TransferToReadyForValidation(string adsElementId)
        {
            ChangeStatusRest(adsElementId, AdvertisementElementStatusValue.ReadyForValidation);
        }

        public void TransferToReadyForValidation(long adsElementId)
        {
            ChangeStatusSoap(adsElementId, AdvertisementElementStatusValue.ReadyForValidation);
        }

        public void TransferToApproved(string adsElementId)
        {
            ChangeStatusRest(adsElementId, AdvertisementElementStatusValue.Valid);
        }

        public void TransferToApproved(long adsElementId)
        {
            ChangeStatusSoap(adsElementId, AdvertisementElementStatusValue.Valid);
        }

        private void ChangeStatusSoap(long adsElementId, AdvertisementElementStatusValue statusValue)
        {
            try
            {
                _changeAdvertisementElementStatusOperationService.ChangeStatus(adsElementId, statusValue, null);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<HandleAdsStateErrorDescription>(new HandleAdsStateErrorDescription(adsElementId, statusValue, ex.Message));
            }
        }

        private void ChangeStatusRest(string specifiedAdsElementId, AdvertisementElementStatusValue statusValue)
        {
            long adsElementId;
            if (!long.TryParse(specifiedAdsElementId, out adsElementId))
            {
                throw new ArgumentException("Ads Elements Id cannot be parsed");
            }

            try
            {
                _changeAdvertisementElementStatusOperationService.ChangeStatus(adsElementId, statusValue, null);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<HandleAdsStateErrorDescription>(new HandleAdsStateErrorDescription(adsElementId, statusValue, ex.Message),
                                                                            HttpStatusCode.BadRequest);
            }
        }
    }
}