using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Import
{
    public sealed class ImportFlowStickersHandler : RequestHandler<FlowStickersRequest, EmptyResponse>
    {
        private readonly ICommonLog _logger;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IHotClientRequestService _hotClientRequestService;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ImportFlowStickersHandler(
            ICommonLog logger,
            IIntegrationSettings integrationSettings,
            IClientProxyFactory clientProxyFactory, 
            IHotClientRequestService hotClientRequestService, 
            IOperationScopeFactory operationScopeFactory)
        {
            _logger = logger;
            _integrationSettings = integrationSettings;
            _clientProxyFactory = clientProxyFactory;
            _hotClientRequestService = hotClientRequestService;
            _operationScopeFactory = operationScopeFactory;
        }

        protected override EmptyResponse Handle(FlowStickersRequest request)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>("NetTcpBinding_IBrokerApiReceiver");

            clientProxy.Execute(brokerApiReceiver =>
            {
                brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, "flowStickers");
                
                try
                {
                    while (true)
                    {
                        var package = brokerApiReceiver.ReceivePackage();
                        if (package == null)
                        {
                            _logger.InfoEx("Импорт зацепок - шина пустая");
                            break;
                        }

                        _logger.InfoFormatEx("Импорт зацепок - загружено {0} объектов из шины", package.Length);
                        if (package.Length != 0)
                        {
                            ProcessPackage(package);
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

        private static HotClientRequest ParseHotClientXml(XElement hotClientXml)
        {
            var hotClientRequest = new HotClientRequest();

            // SourceCode
            var sourceCodeAttr = hotClientXml.Attribute("SourceCode");
            if (sourceCodeAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "SourceCode"));
            }

            hotClientRequest.SourceCode = sourceCodeAttr.Value;

            // UserId
            var userIdAttr = hotClientXml.Attribute("UserCode");
            if (userIdAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "UserCode"));
            }

            hotClientRequest.UserCode = userIdAttr.Value;

            // UserName
            var userNameAttr = hotClientXml.Attribute("UserName");
            if (userNameAttr == null)
            {   
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "UserName"));
            }

            hotClientRequest.UserName = userNameAttr.Value;

            // CreationDate
            var dateAttr = hotClientXml.Attribute("CreationDate");
            if (dateAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "CreationDate"));
            }

            hotClientRequest.CreationDate = DateTime.Parse(dateAttr.Value);

            // ContactName
            var contactNameAttr = hotClientXml.Attribute("ContactName");
            if (contactNameAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "ContactName"));
            }

            hotClientRequest.ContactName = contactNameAttr.Value;

            // ContactPhone
            var contactPhoneAttr = hotClientXml.Attribute("ContactPhone");
            if (contactPhoneAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "ContactPhone"));
            }

            hotClientRequest.ContactPhone = contactPhoneAttr.Value;

            // Description
            var descriptionAttr = hotClientXml.Attribute("Description");
            if (descriptionAttr != null)
            {
                hotClientRequest.Description = descriptionAttr.Value;
            }

            // CardCode
            var cardCodeElement = hotClientXml.Element("CardCode");
            if (cardCodeElement != null)
            {
                hotClientRequest.CardCode = (long)cardCodeElement.Attribute("CardCode");
            }

            // BranchCode
            var branchCodeElement = hotClientXml.Element("BranchCode");
            if (branchCodeElement != null)
            {
                hotClientRequest.BranchCode = (long)branchCodeElement.Attribute("BranchCode");
            }

            if (branchCodeElement == null && cardCodeElement == null)
            {
                throw new BusinessLogicException(BLResources.ImportHotClientCardCodeAndBranchCodeAreNull);
            }

            return hotClientRequest;
        }

        private void ProcessPackage(IEnumerable<string> dataObjects)
        {
            var requests = new List<HotClientRequest>();

            var xmlRoots = dataObjects.Select(XDocument.Parse)
                                      .Where(document => document.Root != null)
                                      .Select(document => document.Root);

            foreach (var root in xmlRoots)
            {
                switch (root.Name.LocalName)
                {
                    case "HotClient":
                        requests.Add(ParseHotClientXml(root));
                        break;
                }
            }

            using (var operationScope = _operationScopeFactory.CreateNonCoupled<ImportFlowStickersIdentity>())
            {
                foreach (var request in requests)
                {
                    _hotClientRequestService.CreateOrUpdate(request);
                    operationScope.Added<HotClientRequest>(request.Id);
                }

                operationScope.Complete();
            }
        }
    }
}
