using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;
using System.Xml.XPath;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects.DTO;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import
{
    public sealed class ImportFlowGeoClassifierHandler : RequestHandler<ImportFlowGeoClassifierRequest, EmptyResponse>
    {
        private readonly ICommonLog _logger;
        private readonly IProjectService _projectService;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;

        public ImportFlowGeoClassifierHandler(
            ICommonLog logger,
            IIntegrationSettings integrationSettings,
            IClientProxyFactory clientProxyFactory,
            IProjectService projectService)
        {
            _logger = logger;
            _integrationSettings = integrationSettings;
            _clientProxyFactory = clientProxyFactory;
            _projectService = projectService;
        }

        protected override EmptyResponse Handle(ImportFlowGeoClassifierRequest request)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            clientProxy.Execute(brokerApiReceiver =>
                {
                    brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowGeoClassifier");

                    try
                    {
                        while (true)
                        {
                            var package = brokerApiReceiver.ReceivePackage();
                            if (package == null)
                            {
                                _logger.InfoEx("Импорт справочников географии - шина пустая");
                                break;
                            }

                            _logger.InfoFormatEx("Импорт справочников географии - загружено {0} объектов из шины", package.Length);
                            if (package.Length != 0)
                            {
                                ProcessPackage(package, request.BasicLanguage, request.ReserveLanguage);
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

        private ImportProjectDTO ParseProjectXml(XElement projectXml, string basicLanguage, string reserveLanguage)
        {
            var projectDto = new ImportProjectDTO();

            // Code
            var codeAttr = projectXml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            projectDto.Code = (int)codeAttr;

            var nameAttr = projectXml.XPathSelectElements("./Localizations/Localization")
                                     .FirstOrDefault(x => x.Attribute("Lang").Value == basicLanguage);

            if (nameAttr == null)
            {
                nameAttr = projectXml.XPathSelectElements("./Localizations/Localization")
                                     .FirstOrDefault(x => x.Attribute("Lang").Value == reserveLanguage);
            }
                
            if (nameAttr == null)
            {
                throw new BusinessLogicException("Проект не содержит названия ни на базовом, ни на резервном языке");
            }

            projectDto.DisplayName = nameAttr.Attribute("Name").Value;

            projectDto.DefaultLang = projectXml.Attribute("DefaultLang").Value;

            // NameLat
            var nameLatAttr = projectXml.Attribute("NameLat");
            if (nameLatAttr != null)
            {
                projectDto.NameLat = nameLatAttr.Value;
            }

            // IsDeleted
            var isDeletedAttr = projectXml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                projectDto.IsDeleted = (bool)isDeletedAttr;
            }

            return projectDto;
        }

        private void ProcessPackage(IEnumerable<string> dataObjects, string basicLanguage, string reserveLanguage)
        {
            var projectDtos = new List<ImportProjectDTO>();

            var xmlRoots = dataObjects.Select(XDocument.Parse)
                                      .Where(document => document.Root != null)
                                      .Select(document => document.Root);

            foreach (var root in xmlRoots)
            {
                switch (root.Name.LocalName)
                {
                    case "Branch":
                        projectDtos.Add(ParseProjectXml(root, basicLanguage, reserveLanguage));
                        break;
                }
            }

            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                _projectService.CreateOrUpdate(projectDtos);
                transactionScope.Complete();
            }
        }
    }
}
