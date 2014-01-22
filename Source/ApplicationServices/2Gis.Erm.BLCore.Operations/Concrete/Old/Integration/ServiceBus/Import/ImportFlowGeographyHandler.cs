using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import
{
    public sealed class ImportFlowGeographyHandler : RequestHandler<ImportFlowGeographyRequest, EmptyResponse>
    {
        private readonly ICommonLog _logger;
        private readonly IFirmRepository _firmRepository;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;

        public ImportFlowGeographyHandler(
            ICommonLog logger,
            IFirmRepository firmRepository,
            IIntegrationSettings integrationSettings,
            IClientProxyFactory clientProxyFactory)
        {
            _logger = logger;
            _firmRepository = firmRepository;
            _integrationSettings = integrationSettings;
            _clientProxyFactory = clientProxyFactory;
        }

        protected override EmptyResponse Handle(ImportFlowGeographyRequest request)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            clientProxy.Execute(brokerApiReceiver =>
            {
                brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowGeography");

                try
                {
                    while (true)
                    {
                        var package = brokerApiReceiver.ReceivePackage();
                        if (package == null)
                        {
                            _logger.InfoEx("Импорт географии - шина пустая");
                            break;
                        }

                        _logger.InfoFormatEx("Импорт географии - загружено {0} объектов из шины", package.Length);
                        if (package.Length != 0)
                        {
                            ProcessPackage(package, request.RegionalTerritoryLocaleSpecificWord);
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

        private static TerritoryServiceBusDto ParseTerritoryXml(XElement territoryXml)
        {
            var territoryDto = new TerritoryServiceBusDto();

            // Code
            var codeAttr = territoryXml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            territoryDto.Code = (long)codeAttr;

            // IsDeleted
            var isDeletedAttr = territoryXml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                territoryDto.IsDeleted = (bool)isDeletedAttr;

                // При выставленном IsDeleted = true, остальные поля не приходят
                if (territoryDto.IsDeleted)
                {
                    return territoryDto;
                }
            }

            // Name
            var nameAttr = territoryXml.Attribute("Name");
            if (nameAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Name");
            }

            territoryDto.Name = nameAttr.Value;

            // BranchCode
            var branchCodeAttr = territoryXml.Attribute("BranchCode");
            if (branchCodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут BranchCode");
            }

            territoryDto.OrganizationUnitDgppId = (int)branchCodeAttr;

            return territoryDto;
        }

        private static BuildingServiceBusDto ParseBuildingXml(XElement buildingXml)
        {
            var buildingDto = new BuildingServiceBusDto();

            // Code
            var codeAttr = buildingXml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            buildingDto.Code = (long)codeAttr;

            // IsDeleted
            var isDeletedAttr = buildingXml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                buildingDto.IsDeleted = (bool)isDeletedAttr;

                // При выставленном IsDeleted = true, остальные поля не приходят
                if (buildingDto.IsDeleted)
                {
                    return buildingDto;
                }
            }
 
            // SaleTerritoryCode
            var saleTerritoryCodeAttr = buildingXml.Attribute("SaleTerritoryCode");
            if (saleTerritoryCodeAttr != null)
            {
                buildingDto.SaleTerritoryCode = (long)saleTerritoryCodeAttr;
            }

            return buildingDto;
        }

        private void ProcessPackage(IEnumerable<string> dataObjects, string regionalTerritoryLocaleSpecificWord )
        {
            var territoryDtos = new List<TerritoryServiceBusDto>();
            var buildingDtos = new List<BuildingServiceBusDto>();

            var xmlRoots = dataObjects.Select(XDocument.Parse)
                .Where(document => document.Root != null)
                .Select(document => document.Root);

            foreach (var root in xmlRoots)
            {
                switch (root.Name.LocalName)
                {
                    case "SaleTerritory":
                        territoryDtos.Add(ParseTerritoryXml(root));
                        break;

                    case "Building":
                        buildingDtos.Add(ParseBuildingXml(root));
                        break;
                }
            }

            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _firmRepository.ImportTerritoryFromServiceBus(territoryDtos);
                _firmRepository.ImportBuildingFromServiceBus(buildingDtos, regionalTerritoryLocaleSpecificWord);
                transactionScope.Complete();
            }
        }
    }
}