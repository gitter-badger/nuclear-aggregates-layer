using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Xml;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class WriteMessageToServiceBusHandler : RequestHandler<WriteMessageToServiceBusRequest, ExportResponse>
    {
        private readonly ITracer _logger;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;

        public WriteMessageToServiceBusHandler(ITracer logger, IIntegrationSettings integrationSettings, IClientProxyFactory clientProxyFactory)
        {
            _logger = logger;
            _integrationSettings = integrationSettings;
            _clientProxyFactory = clientProxyFactory;
        }

        protected override ExportResponse Handle(WriteMessageToServiceBusRequest request)
        {
            var respоnse = new ExportResponse();

            var stream = request.MessageStream;
            string data;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                data = reader.ReadToEnd();
            }

            string error;
            var xsd = Properties.Resources.ResourceManager.GetString(StaticReflection.GetMemberName(request.XsdSchemaResourceExpression));
            var isValidXml = data.ValidateXml(xsd, out error);
            if (!isValidXml)
            {
                _logger.Fatal(error);
                throw new BusinessLogicException(string.Format(BLResources.XSDValidationError, error));
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                {
                    WriteObjectsToServiceBus(new[] { data }, request.FlowName);
                }

                respоnse.Messages = new[] { string.Format("Сообщение успешно выгружено в шину") };
                transaction.Complete();
            }

            return respоnse;
        }

        private void WriteObjectsToServiceBus(IEnumerable<string> exportObjects, string flowName)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiSender>("NetTcpBinding_IBrokerApiSender");

            clientProxy.Execute(brokerApiSender =>
            {
                brokerApiSender.BeginSending(_integrationSettings.IntegrationApplicationName, flowName);

                foreach (var serializedObject in exportObjects)
                {
                    try
                    {
                        brokerApiSender.SendDataObject(serializedObject);
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFormat(e, "Ошибка при записи объекта в шину интеграции (поток {0})", flowName);
                        throw;
                    }
                }

                brokerApiSender.Commit();
                brokerApiSender.EndSending();
            });
        }
    }
}