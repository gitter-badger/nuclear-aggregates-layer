using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import
{
    public sealed class ImportFlowCardsHandler : RequestHandler<ImportFlowCardsRequest, EmptyResponse>
    {
        private readonly ICommonLog _logger;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFirmRepository _firmRepository;

        private readonly long _reserveUserId;
        private readonly long _currentUserId;

        public ImportFlowCardsHandler(
            ICommonLog logger,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier, 
            IUserContext userContext, 
            IFirmRepository firmRepository,
            IIntegrationSettings integrationSettings,
            IClientProxyFactory clientProxyFactory, 
            IOperationScopeFactory scopeFactory)
        {
            _logger = logger;
            _firmRepository = firmRepository;
            _integrationSettings = integrationSettings;
            _clientProxyFactory = clientProxyFactory;
            _scopeFactory = scopeFactory;

            _reserveUserId = securityServiceUserIdentifier.GetReserveUserIdentity().Code;
            _currentUserId = userContext.Identity.Code;
        }

        protected override EmptyResponse Handle(ImportFlowCardsRequest request)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            clientProxy.Execute(brokerApiReceiver =>
            {
                brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowCards");

                try
                {
                    while (true)
                    {
                        var package = brokerApiReceiver.ReceivePackage();
                        if (package == null)
                        {
                            _logger.InfoEx("Импорт фирм - шина пустая");
                            break;
                        }

                        _logger.InfoFormatEx("Импорт фирм - загружено {0} объектов из шины", package.Length);
                        if (package.Length != 0)
                        {
                            var dto = ParseDataObjects(package, request.BasicLanguage, request.ReserveLanguage);

                            // todo {all, 2013-08-12}: тут вызов метода репозитория, котрый делает несколько бизнес-операций.
                            //       для улучшения ситуации стоит его распилить и уже отдельные части оборачивать в операции
                            var importResult = ProcessPackage(dto, request.PregeneratedIdsAmount, request.RegionalTerritoryLocaleSpecificWord);

                            // todo {all, 30.09.2013}: Множество операций проведены вызовом одной хранимки, логируем постфактум. Нужно рефакторить реализацию операции.
                            if (!string.IsNullOrWhiteSpace(dto.FirmXml))
                            {
                                using (var operationScope = _scopeFactory.CreateNonCoupled<ImportFirmIdentity>())
                                {
                                    operationScope.Updated<Firm>(GetFirmIdsFromParsedDto(dto));
                                    operationScope.Complete();
                                }
                            }
                            
                            if (importResult.FirmIdsOfImportedCards != null)
                            {
                                // TODO {all, 06.08.2013}: обратить внимание на фэйковость данного scope - вся логика находится вне этого scope - может можно зарефакторить 
                                // comment {all, 2013-08-12}: Зарефакторить нужно, но при этом имеет смысл разобраться с хранимкой, чтобы она не делала всё сама в одиночку.

                                // Да, операция на самом деле более комплексная, чем отмечает логгер. Пожалуй, даже слишком комплексная...
                                // Да, операция уже совершена и отмечается постфактум.
                                using (var operationScope = _scopeFactory.CreateNonCoupled<ImportCardsFromServiceBusIdentity>())
                                {
                                    operationScope
                                        .Updated<Firm>(importResult.FirmIdsOfImportedCards.ToArray())
                                        .Complete();
                                }
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

        private long[] GetFirmIdsFromParsedDto(ImportFirmServiceBusDto dto)
        {
            var element = XElement.Parse(dto.FirmXml);
            var firmNodeName = XName.Get("Firm", string.Empty);
            var firmIdAttributeName = XName.Get("Code", string.Empty);

            return element.Nodes()
                          .OfType<XElement>()
                          .Where(node => node.Name == firmNodeName)
                          .Select(node => node.Attribute(firmIdAttributeName))
                          .Select(node => long.Parse(node.Value))
                          .ToArray();
        }

        private ImportFirmServiceBusDto ParseDataObjects(IEnumerable<string> dataObjects, string basicLanguage, string reserveLanguage)
        {
            var firmXmlBuilder = new StringBuilder();
            var cardsXmlBuilder = new StringBuilder();
            var cardRelationDtos = new List<CardRelationServiceBusDto>();
            var referenceItemDtos = new List<ReferenceItemServiceBusDto>();
            var referenceDtos = new List<ReferenceServiceBusDto>();

            foreach (var dataObject in dataObjects)
            {
                var document = XDocument.Parse(dataObject);
                var root = document.Root;
                if (root == null)
                {
                    continue;
                }

                switch (root.Name.LocalName)
                {
                    case "Firm":
                        firmXmlBuilder.Append(dataObject);
                        break;
                    case "Card":
                        if (!_integrationSettings.UseWarehouseIntegration)
                        {
                            cardsXmlBuilder.Append(dataObject);
                        }

                        break;
                    case "CardRelation":
                        cardRelationDtos.Add(ParseCardRelationXml(root));
                        break;
                    case "Reference":
                        if (!_integrationSettings.UseWarehouseIntegration)
                        {
                            referenceDtos.Add(ParseReferenceXml(root));
                        }

                        break;
                    case "ReferenceItem":
                        if (!_integrationSettings.UseWarehouseIntegration)
                        {
                            referenceItemDtos.Add(ParseReferenceItemXml(root, basicLanguage, reserveLanguage));
                        }

                        break;
                }
            }

            return new ImportFirmServiceBusDto
            {
                FirmXml = firmXmlBuilder.Length > 0 ? string.Format("<Root>{0}</Root>", firmXmlBuilder) : string.Empty,
                CardsXml = cardsXmlBuilder.Length > 0 ? string.Format("<Root>{0}</Root>", cardsXmlBuilder) : string.Empty,
                CardRelationDtos = cardRelationDtos,
                ReferenceItemDtos = referenceItemDtos,
                ReferenceDtos = referenceDtos,
            };
        }

        private static ReferenceServiceBusDto ParseReferenceXml(XElement referenceXml)
        {
            var referenceDto = new ReferenceServiceBusDto();

            // Code
            var codeAttr = referenceXml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            referenceDto.Code = codeAttr.Value;

            return referenceDto;
        }

        private static ReferenceItemServiceBusDto ParseReferenceItemXml(XElement referenceItemXml, string basicLanguage, string reserveLanguage)
        {
            var referenceItemDto = new ReferenceItemServiceBusDto();

            // ReferenceCode
            var referenceCodeAttr = referenceItemXml.Attribute("ReferenceCode");
            if (referenceCodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут ReferenceCode");
            }

            referenceItemDto.ReferenceCode = referenceCodeAttr.Value;

            // Code
            var codeAttr = referenceItemXml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            referenceItemDto.Code = (int)codeAttr;

            var name =
                referenceItemXml.Elements("Localizations").Elements("Localization")
                                .FirstOrDefault(x => string.Equals(basicLanguage, x.Attribute("Lang").Value, StringComparison.InvariantCultureIgnoreCase)) ??
                referenceItemXml.Elements("Localizations").Elements("Localization")
                                .FirstOrDefault(x => string.Equals(reserveLanguage, x.Attribute("Lang").Value, StringComparison.InvariantCultureIgnoreCase)) ??
                referenceItemXml.Elements("Localizations").Elements("Localization")
                                .FirstOrDefault();

            if (name == null)
            {
                throw new BusinessLogicException(BLResources.ReferenceItemDoesntContainName);
            }

            // Name
            var nameAttr = name.Attribute("Name");
            if (nameAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Name");
            }

            referenceItemDto.Name = nameAttr.Value;

            // IsDeleted
            var isDeletedAttr = referenceItemXml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                referenceItemDto.IsDeleted = (bool)isDeletedAttr;
            }

            return referenceItemDto;
        }

        private static CardRelationServiceBusDto ParseCardRelationXml(XElement cardRelationXml)
        {
            var сardRelationDto = new CardRelationServiceBusDto();

            // Задача 2704 - временно отключена, пока не готовы изменения в IR
            // Code
            var codeAttr = cardRelationXml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            сardRelationDto.Code = (long)codeAttr;

            // Card1Code
            var card1CodeAttr = cardRelationXml.Attribute("Card1Code");
            if (card1CodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Card1Code");
            }

            сardRelationDto.PointOfServiceCardCode = (long)card1CodeAttr;

            // Card2Code
            var card2CodeAttr = cardRelationXml.Attribute("Card2Code");
            if (card2CodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Card2Code");
            }

            сardRelationDto.DepartmentCardCode = (long)card2CodeAttr;

            // OrderNo
            var orderNoAttr = cardRelationXml.Attribute("OrderNo");
            if (orderNoAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут OrderNo");
            }

            // в erm для простоты sorting position должен начинаться с 1
            сardRelationDto.DepartmentCardSortingPosition = (int)orderNoAttr + 1;

            // IsDeleted
            var isDeletedAttr = cardRelationXml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                сardRelationDto.IsDeleted = (bool)isDeletedAttr;
            }

            return сardRelationDto;
        }

        private ImportFirmsResultDto ProcessPackage(ImportFirmServiceBusDto dto, int pregeneratedIdsAmount, string regionalTerritoryLocaleSpecificWord)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var result = _firmRepository.ImportFirmFromServiceBus(dto, _currentUserId, _reserveUserId, pregeneratedIdsAmount, regionalTerritoryLocaleSpecificWord);
                transactionScope.Complete();
                return result;
            }
        }
    }
}
