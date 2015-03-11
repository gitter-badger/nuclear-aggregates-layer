using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.PostIntegrationActivities;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.GeoMaster;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.PostIntegrationActivities
{
    public sealed class SyncFirmAddressesHandler : RequestHandler<SyncFirmAddressesRequest, EmptyResponse>
    {
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IFirmRepository _firmRepository;
        private readonly ITracer _tracer;

        public SyncFirmAddressesHandler(IFirmRepository firmRepository,
                                        IClientProxyFactory clientProxyFactory,
                                        ITracer tracer)
        {
            _firmRepository = firmRepository;
            _clientProxyFactory = clientProxyFactory;
            _tracer = tracer;
        }

        protected override EmptyResponse Handle(SyncFirmAddressesRequest request)
        {
            var addressCodes = request.FirmAddresses.Select(x => x.AddressCode.Value).Distinct().ToArray();

            var geoMasterRequest = new GetAddressesStringRequest
                {
                    MyHeader = new Header { Language = request.Language },
                    codes = addressCodes
                };

            GetAddressesStringResponse geoMasterResponse;
            object faulContract;
            var clientProxy = _clientProxyFactory.GetClientProxy<IMsReadApi>("BasicHttpEndpoint_GeoMasterApi");
            var success = clientProxy.TryExecuteWithFaultContract(x => x.GetAddressesString(geoMasterRequest), out geoMasterResponse, out faulContract);

            if (!success)
            {
                var notSupportedLanguage = faulContract as NotSupportedLanguage;
                if (notSupportedLanguage != null)
                {
                    throw new BusinessLogicException(string.Format("Текущая культура [{0}] не поддерживается в системе GeoMaster", notSupportedLanguage.Language));
                }

                throw new BusinessLogicException("Неизвестная ошибка в системе GeoMaster");
            }

            var addresses = addressCodes.Zip(geoMasterResponse.GetAddressesStringResult, (id, name) => new { Id = id, Name = name }).ToDictionary(arg => arg.Id, arg => arg.Name);

            foreach (var addresNotFound in addresses.Where(x => string.IsNullOrEmpty(x.Value)))
            {
                _tracer.ErrorFormat(string.Format("Адрес фирмы с AddressCode=[{0}] не найден в системе GeoMaster", addresNotFound.Key));
            }

            ProcessGeoMasterResponse(request.FirmAddresses, addresses);
            _firmRepository.UpdateFirmAddresses(request.FirmAddresses);
            
            return Response.Empty;
        }

        private static void ProcessGeoMasterResponse(IEnumerable<FirmAddress> firmAddresses, IDictionary<long, string> addressNames)
        {
            foreach (var firmAddress in firmAddresses)
            {
                string addressName;
                if (addressNames.TryGetValue(firmAddress.AddressCode.Value, out addressName) && !string.IsNullOrEmpty(addressName))
                {
                    firmAddress.Address = addressName;
                }
            }
        }
    }
}