using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.AdsManagement;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.AdsManagement
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ManageTextAdsApplicationService : IManageTextAdsApplicationService, IManageTextAdsApplicationRestService
    {
        private static readonly IEntityType AdvertisementElement = EntityType.Instance.AdvertisementElement();

        private readonly ITracer _tracer;
        private readonly IGetDomainEntityDtoService _getDomainEntityDtoService;
        private readonly IModifyDomainEntityService _modifyBusinessModelEntityService;

        public ManageTextAdsApplicationService(ITracer tracer, IOperationServicesManager operationServicesManager)
        {
            _tracer = tracer;
            _getDomainEntityDtoService = operationServicesManager.GetDomainEntityDtoService(AdvertisementElement);
            _modifyBusinessModelEntityService = operationServicesManager.GetModifyDomainEntityService(AdvertisementElement);
        }

        public void UpdatePlainText(string specifiedAdsElementId, string plainText)
        {
            long adsElementId;
            if (!long.TryParse(specifiedAdsElementId, out adsElementId))
            {
                throw new ArgumentException("Ads Elements Id cannot be parsed");
            }

            try
            {
                UpdateText(adsElementId, dto => dto.PlainText = plainText);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ManageTextAdsErrorDescription>(new ManageTextAdsErrorDescription(adsElementId, ex.Message),
                                                                           HttpStatusCode.BadRequest);
            }
        }

        public void UpdateFormattedText(string specifiedAdsElementId, string formattedText)
        {
            long adsElementId;
            if (!long.TryParse(specifiedAdsElementId, out adsElementId))
            {
                throw new ArgumentException("Ads Elements Id cannot be parsed");
            }

            try
            {
                UpdateText(adsElementId, dto => { dto.FormattedText = formattedText; dto.PlainText = GetPlainText(formattedText); });
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<ManageTextAdsErrorDescription>(new ManageTextAdsErrorDescription(adsElementId, ex.Message),
                                                                           HttpStatusCode.BadRequest);
            }
        }

        public void UpdatePlainText(long adsElementId, string plainText)
        {
            try
            {
                UpdateText(adsElementId, dto => dto.PlainText = plainText);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ManageTextAdsErrorDescription>(new ManageTextAdsErrorDescription(adsElementId, ex.Message));
            }
        }

        public void UpdateFormattedText(long adsElementId, string formattedText)
        {
            try
            {
                UpdateText(adsElementId, dto => { dto.FormattedText = formattedText; dto.PlainText = GetPlainText(formattedText); });
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new FaultException<ManageTextAdsErrorDescription>(new ManageTextAdsErrorDescription(adsElementId, ex.Message));
            }
        }

        // FIXME {d.ivanov, 07.07.2014}: Поскольку не договорились с ЛК о получении plainText при сохранении форматированного текста
        //                               приходится вычислять его самим, поскольку он используется при некоторых проверках даже в форматированных РМ
        //                               Наверно, это и к лучшему, нужно сделать полноценную реализацию и использовать её в веб-приложении тоже, не передавая оттуда painText при редактировании форматированных ЭРМ.
        //                               Думаю, решение, основанное на Regex - это поспешное затыкание дыр, чтобы отдать сервис в тестирование ЛК, а не полноценное решение.
        private string GetPlainText(string htmlFormattedText)
        {
            var regexp = new Regex("<.+?>");
            return regexp.Replace(htmlFormattedText, string.Empty);
        }

        private void UpdateText(long adsElementId, Action<AdvertisementElementDomainEntityDto> action)
        {
            var dto = (AdvertisementElementDomainEntityDto)_getDomainEntityDtoService.GetDomainEntityDto(adsElementId, false, null, EntityType.Instance.None(), string.Empty);
            action(dto);
            _modifyBusinessModelEntityService.Modify(dto);
        }
    }
}