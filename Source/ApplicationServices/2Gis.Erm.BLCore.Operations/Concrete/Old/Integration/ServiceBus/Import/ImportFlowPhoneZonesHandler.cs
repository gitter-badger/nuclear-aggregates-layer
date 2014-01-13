using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import
{
    public sealed class ImportFlowPhoneZonesHandler : RequestHandler<ImportFlowPhoneZonesRequest, EmptyResponse>
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IFirmRepository _firmRepository;

        public ImportFlowPhoneZonesHandler(IClientProxyFactory clientProxyFactory, IIntegrationSettings integrationSettings, IFirmRepository firmRepository)
        {
            _clientProxyFactory = clientProxyFactory;
            _integrationSettings = integrationSettings;
            _firmRepository = firmRepository;
        }

        protected override EmptyResponse Handle(ImportFlowPhoneZonesRequest request)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            clientProxy.Execute(brokerApiReceiver =>
            {
                brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowPhoneZones");

                try
                {
                    while (true)
                    {
                        var package = brokerApiReceiver.ReceivePackage();
                        if (package == null)
                        {
                            break;
                        }

                        if (package.Length != 0)
                        {
                            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                            {
                                ProcessPackage(package);
                                transaction.Complete();
                            }
                        }

                        brokerApiReceiver.Acknowledge();
                    }
                }
                finally
                {
                    brokerApiReceiver.EndReceiving();
                }
            });

            return Response.Empty;
        }

        private static CityPhoneZone ParseCityPhoneZoneXml(XElement сityPhoneZoneXml)
        {
            var cityPhoneZone = new CityPhoneZone();

            // Code
            var codeAttr = сityPhoneZoneXml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            cityPhoneZone.Id = (int)codeAttr;

            // Name
            var nameAttr = сityPhoneZoneXml.Attribute("Name");
            if (nameAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Name");
            }

            cityPhoneZone.Name = nameAttr.Value;

            // CityCode
            var cityCodeAttr = сityPhoneZoneXml.Attribute("CityCode");
            if (cityCodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут CityCode");
            }

            cityPhoneZone.CityCode = (long)cityCodeAttr;

            // IsDefault
            var isDefaultAttr = сityPhoneZoneXml.Attribute("IsDefault");
            if (isDefaultAttr != null)
            {
                cityPhoneZone.IsDefault = (bool)isDefaultAttr;
            }

            // IsDeleted
            var isDeletedAttr = сityPhoneZoneXml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                cityPhoneZone.IsDeleted = (bool)isDeletedAttr;
            }

            return cityPhoneZone;
        }

        private void ProcessPackage(IEnumerable<string> package)
        {
            var cityPhoneZones = new List<CityPhoneZone>();

            foreach (var dataObject in package)
            {
                var document = XDocument.Parse(dataObject);
                var root = document.Root;
                if (root == null)
                {
                    continue;
                }

                switch (root.Name.LocalName)
                {
                    case "CityPhoneZone":
                        {
                            var cityPhoneZone = ParseCityPhoneZoneXml(root);
                            cityPhoneZones.Add(cityPhoneZone);
                        }

                        break;
                }
            }

            if (cityPhoneZones.Any())
            {
                _firmRepository.ImportCityPhoneZonesFromServiceBus(cityPhoneZones);
            }
        }
    }
}
